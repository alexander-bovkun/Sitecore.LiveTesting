// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XPathNodeIteratorExtensions.cs" company="Sitecore A/S">
//   Copyright (c) Sitecore A/S. All rights reserved.
// </copyright>
// <summary>
//   Defines the set of extension methods for XPathNodeIterator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Sitecore.LiveTesting.Extensions.Configuration
{
  using System.IO;
  using System.Xml;
  using System.Xml.XPath;
  using Sitecore.Diagnostics;
  using Sitecore.Xml.Patch;

  /// <summary>
  /// Defines the set of extension methods for XPathNodeIterator.
  /// </summary>
  public static class XPathNodeIteratorExtensions
  {
    /// <summary>
    /// The there are no nodes in sequence.
    /// </summary>
    private const string ThereAreNoNodesInSequence = "There are no nodes in sequnce";

    /// <summary>
    /// The configuration section name.
    /// </summary>
    private const string ConfigurationSectionName = "configuration";

    /// <summary>
    /// The Sitecore namespaces.
    /// </summary>
    private static readonly XmlPatchNamespaces SitecoreNamespaces;

    /// <summary>
    /// Initializes static members of the <see cref="XPathNodeIteratorExtensions" /> class.
    /// </summary>
    static XPathNodeIteratorExtensions()
    {
      SitecoreNamespaces = new XmlPatchNamespaces { PatchNamespace = "http://www.sitecore.net/xmlconfig/", SetNamespace = "http://www.sitecore.net/xmlconfig/set/" };
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <param name="nodeIterator">The node iterator.</param>
    /// <returns>Value of the current iterator node.</returns>
    [NotNull]
    public static string GetValue([NotNull] this XPathNodeIterator nodeIterator)
    {
      Assert.ArgumentNotNull(nodeIterator, "nodeIterator");

      EnsureEnumerationIsStarted(nodeIterator);
      return nodeIterator.Current.Value;
    }

    /// <summary>
    /// Sets the value.
    /// </summary>
    /// <param name="nodeIterator">The node iterator.</param>
    /// <param name="value">The value.</param>
    public static void SetValue([NotNull] this XPathNodeIterator nodeIterator, [NotNull] string value)
    {
      Assert.ArgumentNotNull(nodeIterator, "nodeIterator");
      Assert.ArgumentNotNull(value, "value");

      EnsureEnumerationIsStarted(nodeIterator);
      nodeIterator.Current.SetValue(value);
    }

    /// <summary>
    /// Merges the content.
    /// </summary>
    /// <param name="nodeIterator">The node iterator.</param>
    /// <param name="patch">The patch.</param>
    public static void MergeContent([NotNull] this XPathNodeIterator nodeIterator, [NotNull] IXmlElement patch)
    {
      Assert.ArgumentNotNull(nodeIterator, "nodeIterator");
      Assert.ArgumentNotNull(patch, "patch");

      EnsureEnumerationIsStarted(nodeIterator);

      XmlDocument document = new XmlDocument();
      document.Load(new StringReader(nodeIterator.Current.OuterXml));
      XmlPatchUtils.MergeNodes(document.DocumentElement, patch, SitecoreNamespaces);

      nodeIterator.Current.ReplaceSelf(document.DocumentElement.OuterXml);
    }

    /// <summary>
    /// Merges the content.
    /// </summary>
    /// <param name="nodeIterator">The node iterator.</param>
    /// <param name="fileName">Name of the file.</param>
    public static void MergeContent([NotNull] this XPathNodeIterator nodeIterator, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(nodeIterator, "nodeIterator");
      Assert.ArgumentNotNullOrEmpty(fileName, "fileName");

      using (XmlTextReader xmlTextReader = new XmlTextReader(fileName))
      {
        xmlTextReader.WhitespaceHandling = WhitespaceHandling.None;
        xmlTextReader.MoveToContent();
        xmlTextReader.ReadStartElement(ConfigurationSectionName);

        nodeIterator.MergeContent(new XmlReaderSource(xmlTextReader, Path.GetFileName(fileName)));

        xmlTextReader.ReadEndElement();
      }
    }

    /// <summary>
    /// Ensures the enumeration is started.
    /// </summary>
    /// <param name="nodeIterator">The node iterator.</param>
    private static void EnsureEnumerationIsStarted(XPathNodeIterator nodeIterator)
    {
      if (nodeIterator.CurrentPosition == 0)
      {
        Assert.IsTrue(nodeIterator.MoveNext(), ThereAreNoNodesInSequence);
      }
    }
  }
}
