namespace Sitecore.LiveTesting.Extensions.Tests
{
  using System.Xml;
  using System.Xml.XPath;
  using Sitecore.LiveTesting.Extensions.Configuration;

  /// <summary>
  /// Defines the fake configuration switcher to simplify unit testing.
  /// </summary>
  public abstract class FakeConfigurationSwitcher : SitecoreConfigurationSwitcher
  {
    /// <summary>
    /// Gets the configuration backup.
    /// </summary>
    /// <returns>The <see cref="XmlDocument"/>.</returns>
    public abstract XmlDocument FakeBackupConfiguration();

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <returns>The <see cref="XmlDocument"/>.</returns>
    public abstract XmlDocument FakeGetConfiguration();

    /// <summary>
    /// Restores the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    public abstract void FakeRestoreConfiguration(XmlDocument configuration);

    /// <summary>
    /// Gets the configuration backup.
    /// </summary>
    /// <returns>The <see cref="XmlDocument"/>.</returns>
    protected sealed override XmlDocument BackupConfiguration()
    {
      return this.FakeBackupConfiguration();
    }

    /// <summary>
    /// Disposes the object.
    /// </summary>
    /// <param name="disposing">The disposing.</param>
    protected sealed override void Dispose(bool disposing)
    {
      base.Dispose(disposing);
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <returns>The <see cref="XmlDocument"/>.</returns>
    protected sealed override XmlDocument GetConfiguration()
    {
      return this.FakeGetConfiguration();
    }

    /// <summary>
    /// Get XPath node iterator.
    /// </summary>
    /// <param name="config">The config.</param>
    /// <param name="path">The path.</param>
    /// <returns>The <see cref="XPathNodeIterator"/>.</returns>
    protected sealed override XPathNodeIterator GetXPathNodeIterator(XmlDocument config, string path)
    {
      return base.GetXPathNodeIterator(config, path);
    }

    /// <summary>
    /// Restores the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    protected sealed override void RestoreConfiguration(XmlDocument configuration)
    {
      this.FakeRestoreConfiguration(configuration);
    }
  }
}
