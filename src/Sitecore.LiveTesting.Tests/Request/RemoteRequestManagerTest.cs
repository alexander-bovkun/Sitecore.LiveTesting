namespace Sitecore.LiveTesting.Tests.Request
{
  using System.Web.Hosting;
  using Sitecore.LiveTesting.Request;
  using Xunit;
  using ApplicationHost = Sitecore.LiveTesting.ApplicationHost;

  /// <summary>
  /// Defines the test class for <see cref="RemoteRequestManager"/>.
  /// </summary>
  public class RemoteRequestManagerTest : LiveTest
  {
    /// <summary>
    /// Should execute request in custom application host.
    /// </summary>
    [Fact]
    public void ShouldExecuteRequestInCustomApplicationHost()
    {
      RemoteRequestManager manager = new RemoteRequestManager();
      Request request = new Request { Path = "TestPage.aspx" };

      Response response = manager.ExecuteRemoteRequest(request, new ApplicationHost("CustomApplication", HostingEnvironment.ApplicationVirtualPath, HostingEnvironment.ApplicationPhysicalPath));

      Assert.Equal(200, response.StatusCode);
      Assert.Equal("Test page", response.Content);
    }
  }
}
