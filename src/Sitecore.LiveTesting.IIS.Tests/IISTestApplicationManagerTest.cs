namespace Sitecore.LiveTesting.IIS.Tests
{
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.IIS.Applications;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="IISTestApplicationManager"/>.
  /// </summary>
  public class IISTestApplicationManagerTest
  {
    /// <summary>
    /// Should start and then stop website.
    /// </summary>
    [Fact]
    public void ShouldStartAndThenStopWebsite()
    {
      IISTestApplicationManager applicationManager = new IISTestApplicationManager();
      TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
      TestApplication application = applicationManager.StartApplication(testApplicationHost);
    }
  }
}
