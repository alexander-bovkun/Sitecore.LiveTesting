// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SitecoreConfigurationSwitcher.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the class that allows to modify Sitecore configuration at runtime.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.LiveTesting.Extensions.Configuration
{
  using System;
  using System.Reflection;
  using System.Xml;
  using System.Xml.XPath;
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class that allows to modify Sitecore configuration at runtime.
  /// </summary>
  public class SitecoreConfigurationSwitcher : IDisposable
  {
    /// <summary>
    /// Defines "Instance of the object has been already disposed." phrase.
    /// </summary>
    public const string InstanceOfObjectHasBeenAlreadyDisposed = "Instance of the object has been already disposed.";

    /// <summary>
    /// The configuration field name.
    /// </summary>
    private const string ConfigurationFieldName = "configuration";

    /// <summary>
    /// The configuration configuration.
    /// </summary>
    private XmlDocument configurationBackup;

    /// <summary>
    /// The current configuration.
    /// </summary>
    private XmlDocument currentConfiguration;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Finalizes an instance of the <see cref="SitecoreConfigurationSwitcher"/> class.
    /// </summary>
    ~SitecoreConfigurationSwitcher()
    {
      this.Dispose(false);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Modifies the specified path.
    /// </summary>
    /// <param name="path">The path.</param>
    /// <returns>Current instance of ConfigSwitcher.</returns>
    [NotNull]
    public XPathNodeIterator GetNodeIterator([NotNull] string path)
    {
      Assert.ArgumentNotNullOrEmpty(path, "path");
      Assert.IsFalse(this.disposed, InstanceOfObjectHasBeenAlreadyDisposed);

      if (this.configurationBackup == null)
      {
        this.configurationBackup = this.BackupConfiguration();
      }

      if (this.currentConfiguration == null)
      {
        this.currentConfiguration = this.GetConfiguration();
      }

      return this.GetXPathNodeIterator(this.currentConfiguration, path);
    }

    /// <summary>
    /// Restores the working configuration. Should only be used after Factory.Reset() call.
    /// </summary>
    public void RestoreWorkingConfiguration()
    {
      Assert.IsFalse(this.disposed, InstanceOfObjectHasBeenAlreadyDisposed);

      if (this.currentConfiguration != null)
      {
        this.RestoreConfiguration(this.currentConfiguration);
      }
    }

    /// <summary>
    /// Backups the configuration.
    /// </summary>
    /// <returns>Backed up configuration document.</returns>
    [NotNull]
    protected virtual XmlDocument BackupConfiguration()
    {
      return (XmlDocument)Factory.GetConfiguration().Clone();
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <returns>Current configuration.</returns>
    [NotNull]
    protected virtual XmlDocument GetConfiguration()
    {
      return Factory.GetConfiguration();
    }

    /// <summary>
    /// Restores the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    protected virtual void RestoreConfiguration([NotNull] XmlDocument configuration)
    {
      FieldInfo configurationFieldInfo = typeof(Factory).GetField(ConfigurationFieldName, BindingFlags.Static | BindingFlags.NonPublic);

      if (configurationFieldInfo == null)
      {
        Factory.GetConfiguration().RemoveAll();
        if (configuration.DocumentElement != null)
        {
          Factory.GetConfiguration()
            .AppendChild(Factory.GetConfiguration().ImportNode(configuration.DocumentElement, true));
        }
      }
      else
      {
        configurationFieldInfo.SetValue(null, configuration);
      }
    }

    /// <summary>
    /// Modifies the XML node.
    /// </summary>
    /// <param name="config">The config.</param>
    /// <param name="path">The path.</param>
    /// <returns>XPath iterator corresponding to the specified path.</returns>
    [NotNull]
    protected virtual XPathNodeIterator GetXPathNodeIterator([NotNull] XmlDocument config, [NotNull] string path)
    {
      return config.CreateNavigator().Select(path);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (disposing)
      {
        if (this.configurationBackup != null)
        {
          this.RestoreConfiguration(this.configurationBackup);
        }
      }

      this.disposed = true;
    }
  }
}
