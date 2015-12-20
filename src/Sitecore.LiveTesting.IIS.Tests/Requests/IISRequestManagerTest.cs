namespace Sitecore.LiveTesting.IIS.Tests.Requests
{
  using Sitecore.LiveTesting.IIS.Requests;
  using Sitecore.LiveTesting.Requests;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="IISRequestManager"/>.
  /// </summary>
  public class IISRequestManagerTest : IISLiveTest
  {
    /// <summary>
    /// Should execute request.
    /// </summary>
    [Fact]
    public void ShouldExecuteRequest()
    {
      IISRequestManager requestManager = new IISRequestManager();

      Response result = requestManager.ExecuteRequest(new Request());
    }
  }
}
