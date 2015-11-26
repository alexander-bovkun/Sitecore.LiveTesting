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
    /// The host config template file name.
    /// </summary>
    private const string HostConfigTemplateFileName = "..\\..\\applicationHost.config";

    /// <summary>
    /// The root config relative path.
    /// </summary>
    private const string RootConfigRelativePath = "..\\web.config";

    /// <summary>
    /// The default app pool name.
    /// </summary>
    private const string DefaultInstanceName = "DefaultInstance";

    /// <summary>
    /// The test environment variable.
    /// </summary>
    public static string TestEnvironmentVariable;

    /// <summary>
    /// Should use already created hosted web core instance.
    /// </summary>
    [Fact]
    public void ShouldUseAlreadyCreatedHostedWebCoreInstance()
    {
      const string HostConfigWithExpandedVariablesPath = "applicationHostWithExpandedVariables.config";

      string iisBinFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express");
      string hostConfigPath = Path.GetFullPath(HostConfigWithExpandedVariablesPath);

      File.WriteAllText(hostConfigPath, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", iisBinFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

      using (HostedWebCore hostedWebCore = new HostedWebCore(hostConfigPath, Path.Combine(System.Configuration.ConfigurationManager.OpenMachineConfiguration().FilePath, RootConfigRelativePath), DefaultInstanceName))
      {
        using (TestIISTestApplicationManager applicationManager = new TestIISTestApplicationManager())
        {
          Assert.NotEqual(hostedWebCore, applicationManager.WebCore);
        }
        
        Assert.Equal(HostConfigWithExpandedVariablesPath, Path.GetFileName(HostedWebCore.CurrentHostedWebCoreSetup.HostConfig));
      }

      Assert.Empty(HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
    }

    /// <summary>
    /// Should throw invalid operation exception if no sites are available.
    /// </summary>
    [Fact]
    public void ShouldThrowInvalidOperationExceptionIfNoSitesAreAvailable()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        for (int index = 0; index < 5; ++index)
        {
          TestApplicationHost testApplicationHost = new TestApplicationHost(string.Format("MyApplication{0}", index), "/", "..\\Website");
          applicationManager.StartApplication(testApplicationHost);          
        }

        Assert.ThrowsDelegate action = () => applicationManager.StartApplication(new TestApplicationHost("MyApplication_", "/", "..\\Website"));
        
        Assert.Throws<InvalidOperationException>(action);
      }      
    }

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
    /// Should start website with custom virtual path.
    /// </summary>
    [Fact]
    public void ShouldStartWebsiteWithCustomVirtualPath()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplicationHost testApplicationHost = new TestApplicationHost("MyApplication", "/virtualPath", "..\\Website");
        TestApplication application = applicationManager.StartApplication(testApplicationHost);

        HttpWebRequest request = WebRequest.CreateHttp(string.Format("http://localhost:{0}/virtualPath/TestPage.aspx", IISEnvironmentInfo.GetApplicationInfo(application).Port));
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
          Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
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

    /// <summary>
    /// The extended version of <see cref="IISTestApplicationManager"/> which exposes protected properties for testing purposes.
    /// </summary>
    private class TestIISTestApplicationManager : IISTestApplicationManager
    {
      /// <summary>
      /// Gets instance of <see cref="HostedWebCore"/>.
      /// </summary>
      internal HostedWebCore WebCore
      {
        get
        {
          return this.HostedWebCore;
        }
      }
    }
  }
}
