namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Threading;
  using Microsoft.Win32;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HostedWebCore"/>.
  /// </summary>
  public class HostedWebCoreTest : LiveTest
  {
    /// <summary>
    /// The host config template file name.
    /// </summary>
    private const string HostConfigTemplateFileName = "..\\..\\applicationHost.config";

    /// <summary>
    /// The default app pool name.
    /// </summary>
    private const string DefaultAppPoolName = "IISExpressAppPool";

    /// <summary>
    /// The IIS bin folder.
    /// </summary>
    private readonly string iisBinFolder;

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
      RegistryKey key = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ASP.NET");

      Assert.NotNull(key);

      this.iisBinFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express");
      this.hostConfigPath = Path.GetFullPath("applicationHostWithExpandedVariables.config");
      this.rootConfigPath = Path.Combine(key.OpenSubKey(key.GetSubKeyNames().First(n => n.StartsWith("4.0"))).GetValue("Path").ToString(), "Config\\web.config");      
    }

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static new LiveTest Instantiate(Type testType, params object[] arguments)
    {
      if (LiveTest.InstantiatedByProxy(testType, arguments))
      {
        return LiveTest.Intercept(testType, null);
      }

      return (LiveTest)Activator.CreateInstance(testType, arguments);
    }

    /// <summary>
    /// Should start and then stop hosted web core.
    /// </summary>
    [Fact]
    public void ShouldStartAndThenStopHostedWebCore()
    {
      string hostedWebCoreLibraryPath = Path.Combine(this.iisBinFolder, "hwebcore.dll");

      File.WriteAllText(this.hostConfigPath, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", this.iisBinFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

      Assert.Equal(string.Empty, HostedWebCore.CurrentHostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentInstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentRootConfig);

      using (new HostedWebCore(hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultAppPoolName))
      {
        Assert.Equal(this.hostConfigPath, HostedWebCore.CurrentHostConfig);
        Assert.Equal(hostedWebCoreLibraryPath, HostedWebCore.CurrentHostedWebCoreLibraryPath);
        Assert.Equal(DefaultAppPoolName, HostedWebCore.CurrentInstanceName);
        Assert.Equal(this.rootConfigPath, HostedWebCore.CurrentRootConfig);
      }
      
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentInstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentRootConfig);
    }

    /// <summary>
    /// The should retrieve started hosted web core from another app domain.
    /// </summary>
    [Fact]
    public void ShouldRetrieveStartedHostedWebCoreFromAnotherAppDomain()
    {
      string hostedWebCoreLibraryPath = Path.Combine(this.iisBinFolder, "hwebcore.dll");

      File.WriteAllText(this.hostConfigPath, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", this.iisBinFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

      using (new HostedWebCore(hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultAppPoolName))
      {
        AppDomain appDomain = AppDomain.CreateDomain("HostedWebCoreTestDomain", null, AppDomain.CurrentDomain.SetupInformation);
        
        appDomain.SetData("hostedWebCoreLibraryPath", hostedWebCoreLibraryPath);
        appDomain.SetData("hostConfigPath", this.hostConfigPath);
        appDomain.SetData("rootConfigPath", this.rootConfigPath);

        appDomain.DoCallBack(GetAlreadyHostedWebCore);
      }
    }

    /// <summary>
    /// Should not start two or more hosted web cores in same process.
    /// </summary>
    [Fact]
    public void ShouldNotStartTwoOrMoreHostedWebCoresInSameProcess()
    {
      string hostedWebCoreLibraryPath = Path.Combine(this.iisBinFolder, "hwebcore.dll");

      File.WriteAllText(this.hostConfigPath, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", this.iisBinFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

      using (new HostedWebCore(hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, DefaultAppPoolName))
      {
        Assert.ThrowsDelegate action = () => new HostedWebCore(hostedWebCoreLibraryPath, this.hostConfigPath, this.rootConfigPath, "NewPoolName");
        Assert.Throws<ArgumentException>(action);
      }
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected override void OnAfterMethodCall(object sender, MethodCallEventArgs args)
    {
      base.OnAfterMethodCall(sender, args);
      Monitor.Exit(typeof(HostedWebCore));
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected override void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
    {
      Monitor.Enter(typeof(HostedWebCore));
      base.OnBeforeMethodCall(sender, args);
    }

    /// <summary>
    /// Gets already hosted web core.
    /// </summary>
    private static void GetAlreadyHostedWebCore()
    {
      (new HostedWebCore(AppDomain.CurrentDomain.GetData("hostedWebCoreLibraryPath").ToString(), AppDomain.CurrentDomain.GetData("hostConfigPath").ToString(), AppDomain.CurrentDomain.GetData("rootConfigPath").ToString(), DefaultAppPoolName)).Dispose();
    }
  }
}
