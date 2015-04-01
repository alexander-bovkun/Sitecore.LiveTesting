namespace Sitecore.LiveTesting
{
  using System;
  using System.Reflection;
  using System.Web.Hosting;

  /// <summary>
  /// The test application.
  /// </summary>
  public class TestApplication : MarshalByRefObject, IRegisteredObject
  {
    /// <summary>
    /// Gets the application id.
    /// </summary>
    public string Id
    {
      get { return HostingEnvironment.ApplicationID; }
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
    }
  }
}
