namespace Sitecore.LiveTesting
{
  using System;
  using System.Reflection;
  using System.Web.Hosting;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// The test application.
  /// </summary>
  public class TestApplication : MarshalByRefObject, IRegisteredObject
  {
    /// <summary>
    /// The initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplication"/> class.
    /// </summary>
    public TestApplication() : this(new InitializationManager(new TestApplicationInitializationActionDiscoverer(), new InitializationActionExecutor()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplication"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization manager.</param>
    public TestApplication(InitializationManager initializationManager)
    {
      if (initializationManager == null)
      {
        throw new ArgumentNullException("initializationManager");
      }

      this.initializationManager = initializationManager;
      this.initializationManager.Initialize(0, new TestApplicationInitializationContext(this));
    }

    /// <summary>
    /// Gets the application id.
    /// </summary>
    public string Id
    {
      get { return HostingEnvironment.ApplicationID; }
    }

    /// <summary>
    /// Gets the initialization manager.
    /// </summary>
    protected InitializationManager InitializationManager
    {
      get { return this.initializationManager; }
    }

    /// <summary>
    /// Creates object in the application context.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The created object.</returns>
    public virtual object CreateObject(Type type, params object[] arguments)
    {
      return Activator.CreateInstance(type, arguments);
    }

    /// <summary>
    /// Execute action in the application context for the specified application host.
    /// </summary>
    /// <param name="targetAction">The target action.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The result of action execution.</returns>
    public virtual object ExecuteAction(MethodBase targetAction, params object[] arguments)
    {
      if (!targetAction.IsStatic)
      {
        throw new NotSupportedException("Instance methods are not supported");
      }

      return targetAction.Invoke(null, arguments);
    }

    /// <summary>
    /// Unregisters this instance.
    /// </summary>
    /// <param name="immediate">Determines if unregistering should occur immediately or not.</param>
    void IRegisteredObject.Stop(bool immediate)
    {
      this.Stop(immediate);
    }

    /// <summary>
    /// Unregisters this instance.
    /// </summary>
    /// <param name="immediate">Determines if unregistering should occur immediately or not.</param>
    protected virtual void Stop(bool immediate)
    {
      HostingEnvironment.UnregisterObject(this);
      
      if (!immediate)
      {
        this.initializationManager.Cleanup(0, new TestApplicationInitializationContext(this));
      }
    }
  }
}
