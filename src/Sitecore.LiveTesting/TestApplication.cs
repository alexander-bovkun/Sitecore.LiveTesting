namespace Sitecore.LiveTesting
{
  using System;
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
