namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the tag mapping.
  /// </summary>
  public class TagMapping : ConfigurationElement
  {
    /// <summary>
    /// The tag attribute name.
    /// </summary>
    private const string TagAttributeName = "tag";

    /// <summary>
    /// The attributes element name.
    /// </summary>
    private const string AttributesElementName = "attributes";

    /// <summary>
    /// Gets the tag name.
    /// </summary>
    [ConfigurationProperty(TagAttributeName, IsKey = true, IsRequired = true)]
    public string Tag
    {
      get { return (string)base[TagAttributeName]; }
    }

    /// <summary>
    /// Gets the attributes associated with the tag.
    /// </summary>
    [ConfigurationProperty(AttributesElementName, IsDefaultCollection = true)]
    public TagAttributeCollection Attributes
    {
      get { return (TagAttributeCollection)base[AttributesElementName]; }
    }
  }
}
