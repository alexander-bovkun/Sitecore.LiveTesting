namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using System.IO;
  using System.Threading;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HostedWebCore"/>.
  /// </summary>
  public class HostedWebCoreTest : LiveTest
  {
    /// <summary>
    /// Host config template file name.
    /// </summary>
    private const string HostConfigTemplateFileName = "..\\..\\applicationHost.config";

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static LiveTest Instantiate(Type testType, params object[] arguments)
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
      const string AppPoolName = "IISExpressAppPool";
      const string HostConfigName = "applicationHostWithExpandedVariables.config";

      string binFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "IIS Express");
      string hostedWebCoreLibraryPath = Path.Combine(binFolder, "hwebcore.dll");
      string hostConfigFullPath = Path.GetFullPath(HostConfigName);
      string rootConfig = Path.Combine(Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\ASP.NET\\4.0.30319.0").GetValue("Path").ToString(), "Config\\web.config");

      File.WriteAllText(HostConfigName, File.ReadAllText(HostConfigTemplateFileName).Replace("%IIS_BIN%", binFolder).Replace("%windir%", Environment.GetFolderPath(Environment.SpecialFolder.Windows)));

      Assert.Equal(string.Empty, HostedWebCore.CurrentHostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentInstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentRootConfig);

      using (new HostedWebCore(hostedWebCoreLibraryPath, hostConfigFullPath, rootConfig, AppPoolName))
      {
        Assert.Equal(hostConfigFullPath, HostedWebCore.CurrentHostConfig);
        Assert.Equal(hostedWebCoreLibraryPath, HostedWebCore.CurrentHostedWebCoreLibraryPath);
        Assert.Equal(AppPoolName, HostedWebCore.CurrentInstanceName);
        Assert.Equal(rootConfig, HostedWebCore.CurrentRootConfig);
      }
      
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostConfig);
      Assert.Equal(string.Empty, HostedWebCore.CurrentHostedWebCoreLibraryPath);
      Assert.Equal(string.Empty, HostedWebCore.CurrentInstanceName);
      Assert.Equal(string.Empty, HostedWebCore.CurrentRootConfig);
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
  }
}
