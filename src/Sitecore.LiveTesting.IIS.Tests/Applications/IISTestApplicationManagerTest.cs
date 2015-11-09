namespace Sitecore.LiveTesting.IIS.Tests.Applications
{
  using System;
  using System.IO;
  using System.Net;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.IIS.Applications;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="IISTestApplicationManager"/>.
  /// </summary>
  public class IISTestApplicationManagerTest : SequentialTest
  {
    /// <summary>
    /// The test environment variable.
    /// </summary>
    public static string TestEnvironmentVariable;

    /// <summary>
    /// Should start, initialize, execute request on and then stop website.
    /// </summary>
    [Fact]
    public void ShouldStartInitializeExecuteRequestOnAndThenStopWebsite()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
        TestApplication application = applicationManager.StartApplication(testApplicationHost);
        string initializationToken = string.Format("Sitecore.LiveTesting.{0}.Test", new Random().Next());

        application.ExecuteAction(new Action<string>(InitializationAction), initializationToken);

        HttpWebRequest request = WebRequest.CreateHttp(string.Format("http://localhost:{0}/TestPage.aspx", IISEnvironmentInfo.GetApplicationInfo(application).Port));
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          Assert.Equal(HttpStatusCode.OK, response.StatusCode);
          using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
          {
            Assert.Contains(initializationToken, streamReader.ReadToEnd());
          }
        }

        applicationManager.StopApplication(application);
        Assert.Null(applicationManager.GetRunningApplication(testApplicationHost));
      }
    }

    /// <summary>
    /// Should be operable after restart.
    /// </summary>
    [Fact]
    public void ShouldBeOperableAfterRestart()
    {
      for (int index = 0; index < 2; ++index)
      {
        using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
        {
          TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
          TestApplication application = applicationManager.StartApplication(testApplicationHost);

          HttpWebRequest request = WebRequest.CreateHttp(string.Format("http://localhost:{0}/TestPage.aspx", IISEnvironmentInfo.GetApplicationInfo(application).Port));
          request.GetResponse().Dispose();

          applicationManager.StopApplication(application);
        }
      }
    }

    /// <summary>
    /// Initialization action that will be executed on the side of hosted application.
    /// </summary>
    /// <param name="initializationToken">Random initialization token which presense will be checked.</param>
    private static void InitializationAction(string initializationToken)
    {
      TestEnvironmentVariable = initializationToken;
    }
  }
}
