namespace Sitecore.LiveTesting.Extensions.Tests.Initialization.Configuration
{
  using System.IO;
  using System.Xml;
  using NSubstitute;
  using Sitecore.LiveTesting.Extensions.Initialization.Configuration;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="MergeConfiguration"/>.
  /// </summary>
  public class MergeConfigurationTest
  {
    /// <summary>
    /// The configuration switcher.
    /// </summary>
    private readonly FakeConfigurationSwitcher configurationSwitcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfigurationTest"/> class.
    /// </summary>
    public MergeConfigurationTest()
    {
      this.configurationSwitcher = Substitute.For<FakeConfigurationSwitcher>();
    }

    /// <summary>
    /// Should merge configuration on initialization handler creation and restore it on disposal.
    /// </summary>
    [Fact]
    public void ShouldMergeConfigurationOnInitializationHandlerCreationAndRestoreItOnDisposal()
    {
      const string InitialConfiguration = "<sitecore><unknownConfiguration/></sitecore>";

      XmlDocument configuration = new XmlDocument();
      XmlDocument configurationBackup = new XmlDocument();
      string fileName = Path.GetFullPath("FullConfiguration.config");

      configuration.LoadXml(InitialConfiguration);
      this.configurationSwitcher.FakeGetConfiguration().Returns(configuration);
      this.configurationSwitcher.FakeBackupConfiguration().Returns(configurationBackup);

      using (StreamWriter writer = new StreamWriter(fileName))
      {
        writer.Write("<configuration xmlns:patch=\"http://www.sitecore.net/xmlconfig/\"><sitecore><unknownConfiguration><patch:delete/></unknownConfiguration></sitecore></configuration>");
      }

      try
      {
        using (new MergeConfiguration(this.configurationSwitcher, fileName))
        {
          Assert.Equal("<sitecore></sitecore>", configuration.OuterXml);
        }

        this.configurationSwitcher.Received().FakeRestoreConfiguration(configurationBackup);
      }
      finally 
      {
        File.Delete(fileName);
      }
    }

    /// <summary>
    /// Should merge configuration at the specified XPath.
    /// </summary>
    [Fact]
    public void ShouldMergeConfigurationAtTheSpecifiedXPath()
    {
      const string InitialConfiguration = "<sitecore><indexes><index><config/></index><index><config/></index></indexes></sitecore>";

      XmlDocument configuration = new XmlDocument();
      XmlDocument configurationBackup = new XmlDocument();
      string fileName = Path.GetFullPath("PartOfConfiguration.config");

      configuration.LoadXml(InitialConfiguration);
      this.configurationSwitcher.FakeGetConfiguration().Returns(configuration);
      this.configurationSwitcher.FakeBackupConfiguration().Returns(configurationBackup);

      using (StreamWriter writer = new StreamWriter(fileName))
      {
        writer.Write("<configuration xmlns:patch=\"http://www.sitecore.net/xmlconfig/\"><index><config><patch:delete/></config></index></configuration>");
      }

      try
      {
        using (new MergeConfiguration(this.configurationSwitcher, fileName, "/sitecore/indexes/index"))
        {
          Assert.Equal("<sitecore><indexes><index></index><index></index></indexes></sitecore>", configuration.OuterXml);
        }

        this.configurationSwitcher.Received().FakeRestoreConfiguration(configurationBackup);
      }
      finally
      {
        File.Delete(fileName);
      }
    }
  }
}
