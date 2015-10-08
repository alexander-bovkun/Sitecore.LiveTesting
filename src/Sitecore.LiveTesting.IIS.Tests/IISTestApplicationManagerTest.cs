namespace Sitecore.LiveTesting.IIS.Tests
{
  using System.Xml.Linq;
  using System.Xml.XPath;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.IIS.Applications;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="IISTestApplicationManager"/>.
  /// </summary>
  public class IISTestApplicationManagerTest : SequentialTest
  {
    /// <summary>
    /// Should start, initialize, execute request on and then stop website.
    /// </summary>
    [Fact]
    public void ShouldStartInitializeExecuteRequestOnAndThenStopWebsite()
    {
      IISTestApplicationManager applicationManager = new IISTestApplicationManager();
      TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
      TestApplication application = applicationManager.StartApplication(testApplicationHost);
      applicationManager.StopApplication(application);
      
      XDocument hostConfiguration = XDocument.Load(HostedWebCore.CurrentHostConfig);
      Assert.Empty(hostConfiguration.XPathSelectElement("/configuration/system.applicationHost/sites").Descendants());
    }
  }
}
