namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the tag attribute collection.
  /// </summary>
  [ConfigurationCollection(typeof(TagAttribute), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName = "attribute")]
  public class TagAttributeCollection : ConfigurationElementCollection
  {
    /// <summary>
    /// Create new configuration element.
    /// </summary>
    /// <returns>The <see cref="ConfigurationElement"/>.</returns>
    protected override ConfigurationElement CreateNewElement()
    {
      return new TagAttribute();
    }

    /// <summary>
    /// Gets configuration element key.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The key.</returns>
    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((TagAttribute)element).Id;
    }
  }
}
