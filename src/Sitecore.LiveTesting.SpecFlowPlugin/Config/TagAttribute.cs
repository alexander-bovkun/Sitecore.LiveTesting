namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the tag attribute.
  /// </summary>
  public class TagAttribute : ConfigurationElement
  {
    /// <summary>
    /// The id attribute name.
    /// </summary>
    private const string IdAttributeName = "id";

    /// <summary>
    /// The name attribute name.
    /// </summary>
    private const string TypeAttributeName = "type";

    /// <summary>
    /// The arguments element name.
    /// </summary>
    private const string ArgumentsElementName = "arguments";

    /// <summary>
    /// Gets the identifier.
    /// </summary>
    [ConfigurationProperty(IdAttributeName, IsKey = true, IsRequired = true)]
    public string Id
    {
      get { return (string)base[IdAttributeName]; }
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    [ConfigurationProperty(TypeAttributeName, IsRequired = true)]
    public string Type
    {
      get { return (string)base[TypeAttributeName]; }
    }

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    [ConfigurationProperty(ArgumentsElementName, IsDefaultCollection = true)]
    public TagAttributeArgumentCollection Arguments
    {
      get { return (TagAttributeArgumentCollection)base[ArgumentsElementName]; }
    }
  }
}
