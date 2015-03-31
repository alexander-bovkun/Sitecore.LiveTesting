namespace Sitecore.LiveTesting.Request
{
  using System;
  using System.Web.Hosting;
  using ApplicationHost = Sitecore.LiveTesting.ApplicationHost;

  /// <summary>
  /// Defines the version of request manager that can execute requests for other applications.
  /// </summary>
  public class RemoteRequestManager
  {
    /// <summary>
    /// Executes remote request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="host">The host.</param>
    /// <returns>The <see cref="Response"/>.</returns>
    public Response ExecuteRemoteRequest(Request request, ApplicationHost host)
    {
      if (request == null)
      {
        throw new ArgumentNullException("request");
      }

      if (host == null)
      {
        throw new ArgumentNullException("host");
      }

      return this.GetRemoteRequestManager(host).ExecuteRequest(request);
    }

    /// <summary>
    /// Warms the application up for upcoming request.
    /// </summary>
    /// <param name="host">The host.</param>
    public void WarmupApplicationForRequest(ApplicationHost host)
    {
      this.GetRemoteRequestManager(host);
    }

    /// <summary>
    /// Gets remote request manager for the host.
    /// </summary>
    /// <param name="host">The host.</param>
    /// <returns>The remote version of request manager.</returns>
    protected virtual RequestManager GetRemoteRequestManager(ApplicationHost host)
    {
      if (HostingEnvironment.ApplicationID == host.ApplicationId)
      {
        return new RequestManager();
      }

      return (RequestManager)ApplicationManager.GetApplicationManager().CreateObject(host.ApplicationId, typeof(RequestManager), host.VirtualPath, host.PhysicalPath, false);
    }
  }
}
