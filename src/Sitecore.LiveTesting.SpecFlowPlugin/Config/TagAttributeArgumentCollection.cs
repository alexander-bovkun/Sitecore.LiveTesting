namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines tag attribute arguments collection.
  /// </summary>
  [ConfigurationCollection(typeof(TagAttributeArgument), AddItemName = "argument", CollectionType = ConfigurationElementCollectionType.BasicMap)]
  public class TagAttributeArgumentCollection : ConfigurationElementCollection
  {
    /// <summary>
    /// Create new configuration element.
    /// </summary>
    /// <returns>The <see cref="ConfigurationElement"/>.</returns>
    protected override ConfigurationElement CreateNewElement()
    {
      return new TagAttributeArgument();
    }

    /// <summary>
    /// Gets configuration element key.
    /// </summary>
    /// <param name="element">The element.</param>
    /// <returns>The key.</returns>
    protected override object GetElementKey(ConfigurationElement element)
    {
      return ((TagAttributeArgument)element).Id;
    }
  }
}
