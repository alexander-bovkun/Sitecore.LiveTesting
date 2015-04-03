namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the class that creates and manages applications from <see cref="TestApplicationHost"/>.
  /// </summary>
  /// <typeparam name="T">Type of the application object to create remotely.</typeparam>
  public class TestApplicationManager<T> where T : TestApplication, new()
  {
    /// <summary>
    /// The global testing event handler type name.
    /// </summary>
    private const string GlobalInitializationHandlerTypeName = "SitecoreLiveTestingInitializationHandler";

    /// <summary>
    /// The value indicating whether search for global initialization handler was initiated or not.
    /// </summary>
    private static int subscribedForInitializationFlag;

    /// <summary>
    /// The global initialization handler.
    /// </summary>
    private static object globalInitializationHandler;

    /// <summary>
    /// The application manager.
    /// </summary>
    private readonly ApplicationManager applicationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationManager{T}"/> class.
    /// </summary>
    public TestApplicationManager() : this(ApplicationManager.GetApplicationManager())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationManager{T}"/> class.
    /// </summary>
    /// <param name="applicationManager">The application manager.</param>
    public TestApplicationManager(ApplicationManager applicationManager)
    {
      if (applicationManager == null)
      {
        throw new ArgumentNullException("applicationManager");
      }

      this.applicationManager = applicationManager;
    }

    /// <summary>
    /// Gets the application manager.
    /// </summary>
    protected ApplicationManager ApplicationManager
    {
      get { return this.applicationManager; }
    }

    /// <summary>
    /// Starts application from the application host definition.
    /// </summary>
    /// <param name="applicationHost">The application host.</param>
    /// <returns>Instance of test application.</returns>
    public virtual T StartApplication(TestApplicationHost applicationHost)
    {
      if (applicationHost == null)
      {
        throw new ArgumentNullException("applicationHost");
      }

      this.EnsureGlobalInitializationIsPerformed();

      return (T)this.ApplicationManager.CreateObject(applicationHost.ApplicationId, typeof(T), applicationHost.VirtualPath, Path.GetFullPath(applicationHost.PhysicalPath), false, true);
    }

    /// <summary>
    /// Gets the running application that corresponds to the provided application host.
    /// </summary>
    /// <param name="applicationHost">The application host.</param>
    /// <returns>Matching application instance.</returns>
    public virtual TestApplication GetRunningApplication(TestApplicationHost applicationHost)
    {
      if (applicationHost == null)
      {
        throw new ArgumentNullException("applicationHost");
      }

      return (T)this.ApplicationManager.GetObject(applicationHost.ApplicationId, typeof(T));
    }

    /// <summary>
    /// Stops the application by it's application host definition.
    /// </summary>
    /// <param name="application">The test application.</param>
    public virtual void StopApplication(TestApplication application)
    {
      if (application == null)
      {
        throw new ArgumentNullException("application");
      }

      this.ApplicationManager.ShutdownApplication(application.Id);
    }

    /// <summary>
    /// Gets the list of running applications.
    /// </summary>
    /// <returns>The list of running applications.</returns>
    public virtual IEnumerable<TestApplication> GetRunningApplications()
    {
      List<TestApplication> result = new List<TestApplication>();

      foreach (ApplicationInfo application in this.applicationManager.GetRunningApplications())
      {
        TestApplication candidate = this.applicationManager.GetObject(application.ID, typeof(T)) as TestApplication;

        if (candidate != null)
        {
          result.Add(candidate);
        }
      }

      return result;
    }

    /// <summary>
    /// Ensures that the global initialization is performed.
    /// </summary>
    protected virtual void EnsureGlobalInitializationIsPerformed()
    {
      if ((!HostingEnvironment.IsHosted) && (subscribedForInitializationFlag == 0) && (Interlocked.Increment(ref subscribedForInitializationFlag) == 1))
      {
        Type globalInitializationHandlerType = null;

        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
          foreach (Type type in assembly.GetTypes())
          {
            if (type.Name == GlobalInitializationHandlerTypeName)
            {
              if (globalInitializationHandlerType != null)
              {
                throw new InvalidOperationException(string.Format("Only one global initialization handler with the name '{0}' if permitted per application domain.", GlobalInitializationHandlerTypeName));
              }

              globalInitializationHandlerType = type;
            }
          }
        }

        if (globalInitializationHandlerType != null)
        {
          globalInitializationHandler = Activator.CreateInstance(globalInitializationHandlerType);
        }

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

    /// <summary>
    /// Handles test domain unload.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The event arguments.</param>
    private static void TestDomainOnDomainUnload(object sender, EventArgs e)
    {
      ApplicationManager manager = ApplicationManager.GetApplicationManager();
      
      foreach (ApplicationInfo application in manager.GetRunningApplications())
      {
        manager.ShutdownApplication(application.ID);
      }

      IDisposable disposableCandidate = globalInitializationHandler as IDisposable;

      if (disposableCandidate != null)
      {
        disposableCandidate.Dispose();
      }
    }
  }
}
