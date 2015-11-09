namespace Sitecore.LiveTesting.IIS.Tests.Configuration
{
  using System.Configuration;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Xml.Linq;
  using System.Xml.XPath;
  using Sitecore.LiveTesting.IIS.Configuration;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HostedWebCoreConfigProvider"/>.
  /// </summary>
  public class HostedWebCoreConfigProviderTest : SequentialTest
  {
    /// <summary>
    /// The host config template file name.
    /// </summary>
    private const string HostConfigTemplateFileName = "..\\..\\applicationHost.config";

    /// <summary>
    /// The host config file name.
    /// </summary>
    private readonly string hostConfigFileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="HostedWebCoreConfigProviderTest"/> class.
    /// </summary>
    public HostedWebCoreConfigProviderTest()
    {
      this.hostConfigFileName = Path.GetTempFileName();

      File.Copy(HostConfigTemplateFileName, this.hostConfigFileName, true);
    }

    /// <summary>
    /// Should create site pool.
    /// </summary>
    [Fact]
    public void ShouldCreateSitePool()
    {
      string processedHostConfigFileName = (new HostedWebCoreConfigProvider(this.hostConfigFileName, ConfigurationManager.OpenMachineConfiguration().FilePath)).GetProcessedHostConfig();

      XDocument processedFile = XDocument.Load(processedHostConfigFileName);
      XElement[] sites = processedFile.XPathSelectElements("/configuration/system.applicationHost/sites/site").ToArray();

      Assert.Equal("Sitecore.LiveTesting.IIS.ApplicationHost.config", Path.GetFileName(processedHostConfigFileName));
      Assert.Equal(5, sites.Length);

      for (int index = 0; index < sites.Length; ++index)
      {
        Assert.Equal((index + 1).ToString(CultureInfo.InvariantCulture), sites[index].Attribute("id").Value);
        Assert.Equal((index + 1).ToString(CultureInfo.InvariantCulture), sites[index].Attribute("name").Value);

        XElement[] bindings = sites[index].XPathSelectElements("bindings/binding").ToArray();

        Assert.Equal(1, bindings.Length);
        Assert.Equal("http", bindings.Single().Attribute("protocol").Value);
        Assert.True(bindings.Single().Attribute("bindingInformation").Value.StartsWith("*:"));
        Assert.True(bindings.Single().Attribute("bindingInformation").Value.EndsWith(":localhost"));

        XElement[] applications = sites[index].Elements("application").ToArray();

        Assert.Equal(1, applications.Length);
        Assert.Equal("/", applications.Single().Attribute("path").Value);
        Assert.Equal("Sitecore.LiveTesting", applications.Single().Attribute("applicationPool").Value);

        XElement[] virtualDirectories = applications.Single().XPathSelectElements("virtualDirectory").ToArray();

        Assert.Equal(1, virtualDirectories.Length);
        Assert.Equal("/", virtualDirectories.Single().Attribute("path").Value);
        Assert.Empty(virtualDirectories.Single().Attribute("physicalPath").Value);
      }
    }

    /// <summary>
    /// Should create single app pool
    /// </summary>
    [Fact]
    public void ShouldCreateSingleAppPool()
    {
      string processedHostConfigFileName = (new HostedWebCoreConfigProvider(this.hostConfigFileName, ConfigurationManager.OpenMachineConfiguration().FilePath)).GetProcessedHostConfig();

      XDocument processedFile = XDocument.Load(processedHostConfigFileName);
      XElement[] addedAppPools = processedFile.XPathSelectElements("/configuration/system.applicationHost/applicationPools/add").ToArray();

      Assert.Equal(1, addedAppPools.Length);
      Assert.Equal("Sitecore.LiveTesting", addedAppPools.Single().Attribute("name").Value);
      Assert.Equal("v4.0", addedAppPools.Single().Attribute("managedRuntimeVersion").Value);
      Assert.Equal("Integrated", addedAppPools.Single().Attribute("managedPipelineMode").Value);
    }

    /// <summary>
    /// Should return original root config.
    /// </summary>
    [Fact]
    public void ShouldReturnOriginalRootConfig()
    {
      string rootConfigFileName = Path.GetTempFileName();
      
      string result = (new HostedWebCoreConfigProvider(this.hostConfigFileName, rootConfigFileName)).GetProcessedRootConfig();

      Assert.Equal(rootConfigFileName, result);
    }
  }
}
