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
    /// Should get application instance from another AppDomain.
    /// </summary>
    [Fact]
    public void ShouldGetApplicationInstanceFromAnotherAppDomain()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
        TestApplication application = applicationManager.StartApplication(testApplicationHost);
        AppDomain domain = AppDomain.CreateDomain("TestDomain", null, AppDomain.CurrentDomain.SetupInformation);

        application.ExecuteAction(new Action<string>(InitializationAction), "AppTokenFromAnotherDomain");

        try
        {
          domain.DoCallBack(GetRemoteApplicationAndCheckVariables);
        }
        finally
        {
          AppDomain.Unload(domain);
        }

        applicationManager.StopApplication(application);
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

    /// <summary>
    /// Gets the value of test environment variable.
    /// </summary>
    /// <returns></returns>
    private static string GetTestEnvironmentVariable()
    {
      return TestEnvironmentVariable;
    }

    /// <summary>
    /// Gets remote application and checks variables.
    /// </summary>
    private static void GetRemoteApplicationAndCheckVariables()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/", "..\\Website");
        TestApplication application = applicationManager.StartApplication(testApplicationHost);

        string result = (string)application.ExecuteAction(new Func<string>(GetTestEnvironmentVariable));

        Assert.Equal("AppTokenFromAnotherDomain", result);
      }
    }
  }
}
