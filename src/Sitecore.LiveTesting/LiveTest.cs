namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the base class for tests.
  /// </summary>
  [DynamicConstruction]
  public class LiveTest : ContextBoundObject, IRegisteredObject
  {
    /// <summary>
    /// The default application id.
    /// </summary>
    private const string DefaultApplicationId = "Sitecore.LiveTesting.Default";

    /// <summary>
    /// The website path setting name.
    /// </summary>
    private const string WebsitePathSettingName = "Sitecore.LiveTest.WebsitePath";

    /// <summary>
    /// The name of method that gets default application host.
    /// </summary>
    private const string GetDefaultApplicationHostName = "GetDefaultApplicationHost";

    /// <summary>
    /// The global testing event handler type name.
    /// </summary>
    private const string GlobalInitializationHandlerTypeName = "GlobalInitializationHandler";

    /// <summary>
    /// The value indicating whether search for global initialization handler was initiated or not.
    /// </summary>
    private static int subscribedForInitializationFlag;

    /// <summary>
    /// The value indicating whether infrastructure was subscribed for termination of current test <see cref="AppDomain"/>.
    /// </summary>
    private static int subscribedForTerminationFlag;

    /// <summary>
    /// The global initialization handlers.
    /// </summary>
    private static IEnumerable<object> globalInitializationHandlers;

    /// <summary>
    /// Gets default application host for the specified test type.
    /// </summary>
    /// <param name="testType">Type of the test.</param>
    /// <returns>The default application host.</returns>
    public static ApplicationHost GetDefaultApplicationHost(Type testType)
    {
      return new ApplicationHost(DefaultApplicationId, "/", ConfigurationManager.AppSettings.Get(WebsitePathSettingName) ?? Directory.GetParent(Environment.CurrentDirectory).FullName);
    }

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <returns>Instance of the class.</returns>
    public static LiveTest Instantiate(Type testType)
    {
      MethodInfo getDefaultApplicationHostMethod = Utility.GetInheritedMethod(testType, GetDefaultApplicationHostName, new[] { typeof(Type) });

      if (getDefaultApplicationHostMethod == null)
      {
        throw new InvalidOperationException(string.Format("Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", testType.FullName, GetDefaultApplicationHostName, typeof(LiveTest).FullName));
      }

      ApplicationHost host = (ApplicationHost)getDefaultApplicationHostMethod.Invoke(null, new object[] { testType });

      if (host == null)
      {
        throw new InvalidOperationException(string.Format("Failed to get an instance of '{0}'.", typeof(ApplicationHost).FullName));
      }

      if (!HostingEnvironment.IsHosted)
      {
        if ((subscribedForInitializationFlag == 0) && (Interlocked.Increment(ref subscribedForInitializationFlag) == 1))
        {
          globalInitializationHandlers = GetDiscoveredGlobalInitializationHandlers();
        }

        if ((subscribedForTerminationFlag == 0) && (Interlocked.Increment(ref subscribedForTerminationFlag) == 1))
        {
          if (AppDomain.CurrentDomain.IsDefaultAppDomain())
          {
            AppDomain.CurrentDomain.ProcessExit += TestDomainOnDomainUnload;
          }
          else
          {
            AppDomain.CurrentDomain.DomainUnload += TestDomainOnDomainUnload;
          }
        }
      }
      else
      {
        var hostingEnvironment = typeof(HostingEnvironment).GetField("_theHostingEnvironment", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null);
        var eventHandler = (EventHandler)typeof(HostingEnvironment).GetField("_onAppDomainUnload", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(hostingEnvironment);

        Thread.GetDomain().DomainUnload -= eventHandler;
      }

      return (LiveTest)ApplicationManager.GetApplicationManager().CreateObject(host.ApplicationId, testType, host.VirtualPath, Path.GetFullPath(host.PhysicalPath), false, true);
    }

    /// <summary>
    /// Stops the instance of the test.
    /// </summary>
    /// <param name="immediate"><value>true</value> if must be stopped immediately, otherwise <value>false</value>.</param>
    public virtual void Stop(bool immediate)
    {
      HostingEnvironment.UnregisterObject(this);
    }

    /// <summary>
    /// Gets global initialization handler instance.
    /// </summary>
    /// <returns>Global initialization handler instances.</returns>
    private static IEnumerable<object> GetDiscoveredGlobalInitializationHandlers()
    {
      List<object> result = new List<object>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (type.Name == GlobalInitializationHandlerTypeName)
          {
            ConstructorInfo constructorInfo = type.GetConstructor(new Type[0]);

            if (constructorInfo != null)
            {
              result.Add(constructorInfo.Invoke(new object[0]));
            }
          }
        }
      }

      return result;
    }

    /// <summary>
    /// Defines the handler for the test domain unload event.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="eventArgs">The arguments.</param>
    private static void TestDomainOnDomainUnload(object sender, EventArgs eventArgs)
    {
      ApplicationManager manager = ApplicationManager.GetApplicationManager();

      foreach (ApplicationInfo appInfo in manager.GetRunningApplications())
      {
        manager.ShutdownApplication(appInfo.ID);
      }

      Stack<object> globalInitializationHandlersInOrder = new Stack<object>(globalInitializationHandlers);

      while (globalInitializationHandlersInOrder.Count > 0)
      {
        IDisposable disposableCandidate = globalInitializationHandlersInOrder.Pop() as IDisposable;

        if (disposableCandidate != null)
        {
          disposableCandidate.Dispose();
        }
      }
    }
  }
}
