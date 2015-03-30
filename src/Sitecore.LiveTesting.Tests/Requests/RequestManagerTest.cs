namespace Sitecore.LiveTesting.Tests.Requests
{
  using Sitecore.LiveTesting.Requests;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="RequestManager"/>.
  /// </summary>
  public class RequestManagerTest : LiveTest
  {
    /// <summary>
    /// Should request page.
    /// </summary>
    [Fact]
    public void ShouldRequestPage()
    {
      RequestManager manager = new RequestManager();
      Request request = new Request { Path = "TestPage.aspx" };

      Response response = manager.ExecuteRequest(request);

      Assert.Equal(200, response.StatusCode);
      Assert.Equal("Test page", response.Content);
    }
  }
}
