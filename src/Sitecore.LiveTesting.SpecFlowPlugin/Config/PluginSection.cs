namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// The configuration section for SpecFlow plugin.
  /// </summary>
  public class PluginSection : ConfigurationSection
  {
    /// <summary>
    /// The tag mappings element name.
    /// </summary>
    private const string TagMappingsElementName = "tagmappings";

    /// <summary>
    /// Gets the tag mappings.
    /// </summary>
    [ConfigurationProperty(TagMappingsElementName, IsDefaultCollection = true)]
    public TagMappingCollection TagMappings
    {
      get { return (TagMappingCollection)base[TagMappingsElementName]; }
    }
  }
}
