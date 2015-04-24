namespace Sitecore.LiveTesting.Extensions.Initialization.Configuration
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Xml.XPath;
  using Sitecore.Diagnostics;
  using Sitecore.LiveTesting.Extensions.Configuration;

  /// <summary>
  /// Defines the initialization handler that merges configuration from the provided file.
  /// </summary>
  public class MergeConfiguration : IDisposable
  {
    /// <summary>
    /// The Sitecore configuration root XPath.
    /// </summary>
    private const string SitecoreConfigurationRootXPath = "/sitecore";

    /// <summary>
    /// The Sitecore configuration switcher.
    /// </summary>
    private readonly SitecoreConfigurationSwitcher sitecoreConfigurationSwitcher;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    /// <param name="fileName">The file Name.</param>
    public MergeConfiguration(string fileName) : this(new SitecoreConfigurationSwitcher(), fileName, SitecoreConfigurationRootXPath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="xpath">The XPath of the element to merge configuration to.</param>
    public MergeConfiguration(string fileName, string xpath) : this(new SitecoreConfigurationSwitcher(), fileName, xpath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    /// <param name="sitecoreConfigurationSwitcher">The Sitecore configuration switcher.</param>
    /// <param name="fileName">The file Name.</param>
    public MergeConfiguration(SitecoreConfigurationSwitcher sitecoreConfigurationSwitcher, string fileName) : this(sitecoreConfigurationSwitcher, fileName, SitecoreConfigurationRootXPath)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    /// <param name="sitecoreConfigurationSwitcher">The Sitecore configuration switcher.</param>
    /// <param name="fileName">The file name.</param>
    /// <param name="xpath">The XPath of the element to merge configuration to.</param>
    public MergeConfiguration(SitecoreConfigurationSwitcher sitecoreConfigurationSwitcher, string fileName, string xpath) : this(sitecoreConfigurationSwitcher)
    {
      const string FileNameDoesNotRepresentAFileMessage = "File name does not represent a file.";

      Assert.ArgumentNotNullOrEmpty(fileName, "fileName");
      Assert.ArgumentNotNullOrEmpty(xpath, "xpath");

      string fileDirectory = Path.GetDirectoryName(fileName);
      string filePattern = Path.GetFileName(fileName);

      Assert.ArgumentCondition(fileDirectory != null, "fileName", FileNameDoesNotRepresentAFileMessage);
      Assert.ArgumentCondition(!string.IsNullOrEmpty(filePattern), "fileName", FileNameDoesNotRepresentAFileMessage);

      XPathNodeIterator iterator = this.SitecoreConfigurationSwitcher.GetNodeIterator(xpath);
      LinkedList<XPathNodeIterator> elements = new LinkedList<XPathNodeIterator>();

      while (iterator.MoveNext())
      {
        elements.AddLast(iterator.Clone());
      }

      fileDirectory = string.IsNullOrEmpty(fileDirectory) ? "." : fileDirectory;

      foreach (string file in Directory.GetFiles(fileDirectory, filePattern))
      {
        foreach (XPathNodeIterator element in elements)
        {
          element.MergeContent(file);
        }
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    /// <param name="sitecoreConfigurationSwitcher">The Sitecore configuration switcher.</param>
    protected MergeConfiguration(SitecoreConfigurationSwitcher sitecoreConfigurationSwitcher)
    {
      Assert.ArgumentNotNull(sitecoreConfigurationSwitcher, "sitecoreConfigurationSwitcher");
      
      this.sitecoreConfigurationSwitcher = sitecoreConfigurationSwitcher;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="MergeConfiguration"/> class.
    /// </summary>
    ~MergeConfiguration()
    {
      this.Dispose(false);
    }

    /// <summary>
    /// Gets the Sitecore configuration switcher.
    /// </summary>
    protected SitecoreConfigurationSwitcher SitecoreConfigurationSwitcher
    {
      get { return this.sitecoreConfigurationSwitcher; }
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
        this.SitecoreConfigurationSwitcher.Dispose();
      }

      this.disposed = true;
    }
  }
}
