namespace Sitecore.LiveTesting.Applications
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Threading;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the class that creates and manages applications from <see cref="TestApplicationHost"/>.
  /// </summary>
  public class TestApplicationManager
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
    /// The test application type.
    /// </summary>
    private readonly Type testApplicationType;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationManager"/> class.
    /// </summary>
    public TestApplicationManager() : this(ApplicationManager.GetApplicationManager(), typeof(TestApplication))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationManager"/> class.
    /// </summary>
    /// <param name="applicationManager">The application manager.</param>
    /// <param name="testApplicationType">The test application type.</param>
    public TestApplicationManager(ApplicationManager applicationManager, Type testApplicationType)
    {
      if (applicationManager == null)
      {
        throw new ArgumentNullException("applicationManager");
      }

      if (testApplicationType == null)
      {
        throw new ArgumentNullException("testApplicationType");
      }

      if (!typeof(TestApplication).IsAssignableFrom(testApplicationType))
      {
        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Only applications derived from '{0}' are supported.", typeof(TestApplication).AssemblyQualifiedName));
      }

      this.applicationManager = applicationManager;
      this.testApplicationType = testApplicationType;
    }

    /// <summary>
    /// Gets the application manager.
    /// </summary>
    protected ApplicationManager ApplicationManager
    {
      get { return this.applicationManager; }
    }

    /// <summary>
    /// Gets the test application type.
    /// </summary>
    protected Type TestApplicationType
    {
      get { return this.testApplicationType; }
    }

    /// <summary>
    /// Starts application from the application host definition.
    /// </summary>
    /// <param name="applicationHost">The application host.</param>
    /// <returns>Instance of test application.</returns>
    public virtual TestApplication StartApplication(TestApplicationHost applicationHost)
    {
      if (applicationHost == null)
      {
        throw new ArgumentNullException("applicationHost");
      }

      this.EnsureGlobalInitializationIsPerformed();

      return (TestApplication)this.ApplicationManager.CreateObject(applicationHost.ApplicationId, this.TestApplicationType, applicationHost.VirtualPath, Path.GetFullPath(applicationHost.PhysicalPath), false, true);
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

      return (TestApplication)this.ApplicationManager.GetObject(applicationHost.ApplicationId, this.TestApplicationType);
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

      // The following try block helps resolve AppDomain unload deadlock issue in some environments so that AppDomain.DomainUnload event handlers will be processed shortly after Assembly.GetSatelliteAssembly call.
      try
      {
        typeof(ApplicationManager).Assembly.GetSatelliteAssembly(CultureInfo.InvariantCulture);
      }
      catch (FileNotFoundException)
      {
      }

      this.ApplicationManager.ShutdownApplication(application.Id);
      this.WaitUntilApplicationUnloaded(application);
    }

    /// <summary>
    /// Gets the list of running applications.
    /// </summary>
    /// <returns>The list of running applications.</returns>
    public virtual IEnumerable<TestApplication> GetRunningApplications()
    {
      List<TestApplication> result = new List<TestApplication>();

      foreach (ApplicationInfo application in this.ApplicationManager.GetRunningApplications())
      {
        TestApplication candidate = this.ApplicationManager.GetObject(application.ID, this.TestApplicationType) as TestApplication;

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
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.Name == GlobalInitializationHandlerTypeName)
                    {
                        if (globalInitializationHandlerType != null)
                        {
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture,
                                    "Only one global initialization handler with the name '{0}' is permitted per application domain.",
                                    GlobalInitializationHandlerTypeName));
                        }

                        globalInitializationHandlerType = type;
                    }
                }
            }
            catch (ReflectionTypeLoadException)
            {
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
    /// Waits until application is unloaded.
    /// </summary>
    /// <param name="application">The application.</param>
    protected virtual void WaitUntilApplicationUnloaded(TestApplication application)
    {
      try
      {
        while (application != null)
        {
          string applicationId = application.Id;
          
          if (string.IsNullOrEmpty(applicationId))
          {
            break;
          }
          else
          {
            Thread.Sleep(50);
          }
        }
      }
      catch (AppDomainUnloadedException)
      {
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
