namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using System.IO;
  using System.Threading;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HostedWebCore"/>.
  /// </summary>
  public class HostedWebCoreTest : SequentialTest
  {
    /// <summary>
    /// The host config template file name.
    /// </summary>
    private const string HostConfigTemplateFileName = "..\\..\\applicationHost.config";

    /// <summary>
    /// The default app pool name.
    /// </summary>
    private const string DefaultInstanceName = "DefaultInstance";

    /// <summary>
    /// The IIS bin folder.
    /// </summary>
    private readonly string iisBinFolder;

    /// <summary>
    /// The hosted web core library path.
    /// </summary>
    private readonly string hostedWebCoreLibraryPath;

    /// <summary>
    /// The host config path.
    /// </summary>
    private readonly string hostConfigPath;

    /// <summary>
    /// The root config path.
    /// </summary>
    private readonly string rootConfigPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedWebCoreTest"/> class.
    /// </summary>
    public HostedWebCoreTest()
    {
      this.iisBinFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express");
      this.hostedWebCoreLibraryPath = Path.Combine(this.iisBinFolder, "hwebcore.dll");
      this.hostConfigPath = Path.GetFullPath("applicationHostWithExpandedVariables.config");
      this.rootConfigPath = Path.Combine(Path.GetDirectoryName(System.Configuration.ConfigurationManager.OpenMachineConfiguration().FilePath), "web.config");

      File.WriteAllText(this.hostConfigPath, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", this.iisBinFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));
    }

    /// <summary>
    /// Should start and then stop hosted web core.
    /// </summary>
    [Fact]
    public void ShouldStartAndThenStopHostedWebCore()
    {
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);

      using (new HostedWebCore(this.hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultInstanceName))
      {
        Assert.Equal(this.hostConfigPath, HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
        Assert.Equal(this.hostedWebCoreLibraryPath, HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
        Assert.Equal(DefaultInstanceName, HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
        Assert.Equal(this.rootConfigPath, HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);
      }

      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);
    }

    /// <summary>
    /// The should retrieve started hosted web core from another app domain.
    /// </summary>
    [Fact]
    public void ShouldRetrieveStartedHostedWebCoreFromAnotherAppDomain()
    {
      using (new HostedWebCore(this.hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultInstanceName))
      {
        AppDomain appDomain = AppDomain.CreateDomain("HostedWebCoreTestDomain", null, AppDomain.CurrentDomain.SetupInformation);
        
        try
        {
          appDomain.SetData("hostedWebCoreLibraryPath", this.hostedWebCoreLibraryPath);
          appDomain.SetData("hostConfigPath", this.hostConfigPath);
          appDomain.SetData("rootConfigPath", this.rootConfigPath);
          appDomain.SetData("instanceName", DefaultInstanceName);

          appDomain.DoCallBack(GetAlreadyHostedWebCore);

          Assert.Equal(this.hostedWebCoreLibraryPath, HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
          Assert.Equal(this.hostConfigPath, HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
          Assert.Equal(this.rootConfigPath, HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);
          Assert.Equal(DefaultInstanceName, HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
        }
        finally
        {
          AppDomain.Unload(appDomain);
        }
      }

      Assert.Empty(HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
      Assert.Empty(HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
      Assert.Empty(HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);
      Assert.Empty(HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
    }

    /// <summary>
    /// Should not start two or more hosted web cores in same process.
    /// </summary>
    [Fact]
    public void ShouldNotStartTwoOrMoreHostedWebCoresInSameProcess()
    {
      using (new HostedWebCore(this.hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultInstanceName))
      {
        AppDomain appDomain = AppDomain.CreateDomain("HostedWebCoreTestDomain", null, AppDomain.CurrentDomain.SetupInformation);

        try
        {
          appDomain.SetData("hostedWebCoreLibraryPath", this.hostedWebCoreLibraryPath);
          appDomain.SetData("hostConfigPath", this.hostConfigPath);
          appDomain.SetData("rootConfigPath", this.rootConfigPath);
          appDomain.SetData("instanceName", "NewInstance");

          Assert.ThrowsDelegate action = () => appDomain.DoCallBack(GetAlreadyHostedWebCore);
          Assert.Throws<ArgumentException>(action);
        }
        finally
        {
          AppDomain.Unload(appDomain);
        }
      }
    }

    /// <summary>
    /// Should throw invalid operation exception if hosted web core library cannot be loaded.
    /// </summary>
    [Fact]
    public void ShouldThrowInvalidOperationExceptionIfHostedWebCoreLibraryCannotBeLoaded()
    {
      string invalidHostedWebCoreLibraryPath = Path.Combine(this.iisBinFolder, "crabcore.dll");

      Assert.ThrowsDelegate action = () => new HostedWebCore(invalidHostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultInstanceName);
      
      Assert.Throws<InvalidOperationException>(action);
    }

    /// <summary>
    /// Should throw invalid operation exception if host config is not valid.
    /// </summary>
    [Fact]
    public void ShouldThrowInvalidOperationExceptionIfHostConfigIsNotValid()
    {
      Assert.ThrowsDelegate action = () => new HostedWebCore(this.hostedWebCoreLibraryPath, Path.GetFullPath(HostConfigTemplateFileName), this.rootConfigPath, DefaultInstanceName);

      Assert.Throws<InvalidOperationException>(action);      
    }

    /// <summary>
    /// Should guarantee thread safety on hosted web core consruction and destruction.
    /// </summary>
    [Fact]
    public void ShouldGuaranteeThreadSafetyOnHostedWebCoreConstructionAndDestruction()
    {
      AppDomain appDomain = AppDomain.CreateDomain("HostedWebCoreTestDomain", null, AppDomain.CurrentDomain.SetupInformation);

      try
      {
        appDomain.SetData("hostedWebCoreLibraryPath", this.hostedWebCoreLibraryPath);
        appDomain.SetData("hostConfigPath", this.hostConfigPath);
        appDomain.SetData("rootConfigPath", this.rootConfigPath);
        appDomain.SetData("instanceName", "NewInstance");

        AppDomain.CurrentDomain.SetData("hostedWebCoreLibraryPath", this.hostedWebCoreLibraryPath);
        AppDomain.CurrentDomain.SetData("hostConfigPath", this.hostConfigPath);
        AppDomain.CurrentDomain.SetData("rootConfigPath", this.rootConfigPath);
        AppDomain.CurrentDomain.SetData("instanceName", "NewInstance");

        Thread appDomainThread = new Thread(() => appDomain.DoCallBack(RunConcurrentHostedWebCoreConstructionAndDeconstruction));
        appDomainThread.Start();
        RunConcurrentHostedWebCoreConstructionAndDeconstruction();
        appDomainThread.Join();
      }
      finally
      {
        AppDomain.Unload(appDomain);
      }
    }

    /// <summary>
    /// Gets already hosted web core.
    /// </summary>
    private static void GetAlreadyHostedWebCore()
    {
      (new HostedWebCore(AppDomain.CurrentDomain.GetData("hostedWebCoreLibraryPath").ToString(), AppDomain.CurrentDomain.GetData("hostConfigPath").ToString(), AppDomain.CurrentDomain.GetData("rootConfigPath").ToString(), AppDomain.CurrentDomain.GetData("instanceName").ToString())).Dispose();
      Assert.Equal(AppDomain.CurrentDomain.GetData("hostedWebCoreLibraryPath").ToString(), HostedWebCore.CurrentHostedWebCoreSetup.HostedWebCoreLibraryPath);
      Assert.Equal(AppDomain.CurrentDomain.GetData("hostConfigPath").ToString(), HostedWebCore.CurrentHostedWebCoreSetup.HostConfig);
      Assert.Equal(AppDomain.CurrentDomain.GetData("rootConfigPath").ToString(), HostedWebCore.CurrentHostedWebCoreSetup.RootConfig);
      Assert.Equal(AppDomain.CurrentDomain.GetData("instanceName").ToString(), HostedWebCore.CurrentHostedWebCoreSetup.InstanceName);
    }

    /// <summary>
    /// Runs concurrent hosted web core construction/deconstruction.
    /// </summary>
    private static void RunConcurrentHostedWebCoreConstructionAndDeconstruction()
    {
      Thread[] threads = new Thread[7];

      for (int index = 0; index < threads.Length; ++index)
      {
        threads[index] = new Thread(() => (new HostedWebCore(AppDomain.CurrentDomain.GetData("hostedWebCoreLibraryPath").ToString(), AppDomain.CurrentDomain.GetData("hostConfigPath").ToString(), AppDomain.CurrentDomain.GetData("rootConfigPath").ToString(), AppDomain.CurrentDomain.GetData("instanceName").ToString())).Dispose());
        threads[index].Start();
      }

      foreach (Thread thread in threads)
      {
        thread.Join();
      }
    }
  }
}
