namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the tag mapping collection.
  /// </summary>
  [ConfigurationCollection(typeof(TagMapping), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
  public class TagMappingCollection : ConfigurationElementCollection
  {
    /// <summary>
    /// Create new configuration element.
    /// </summary>
    /// <returns>The <see cref="ConfigurationElement"/>.</returns>
    protected override ConfigurationElement CreateNewElement()
    {
      return new TagMapping();
    }

    /// <summary>
    /// Gets configuration element key.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The key.</returns>
    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((TagMapping)element).Tag;
    }
  }
}
