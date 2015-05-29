namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the configuration settings related to the base class.
  /// </summary>
  public class BaseClass : ConfigurationElement
  {
    /// <summary>
    /// The type attribute name.
    /// </summary>
    private const string TypeAttributeName = "type";

    /// <summary>
    /// Gets the type of the base class.
    /// </summary>
    [ConfigurationProperty(TypeAttributeName, DefaultValue = "Sitecore.LiveTesting.LiveTest")]
    public string Type
    {
      get { return (string)base[TypeAttributeName]; }
    }
  }
}
