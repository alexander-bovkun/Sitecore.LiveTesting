namespace Sitecore.LiveTesting.SpecFlowPlugin.Config
{
  using System.Configuration;

  /// <summary>
  /// Defines the tag attribute argument.
  /// </summary>
  public class TagAttributeArgument : ConfigurationElement
  {
    /// <summary>
    /// The id attribute name.
    /// </summary>
    private const string IdAttributeName = "id";

    /// <summary>
    /// The name attribute name.
    /// </summary>
    private const string NameAttributeName = "name";

    /// <summary>
    /// The code snippet attribute name.
    /// </summary>
    private const string CodeSnippetAttributeName = "codeSnippet";

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
    [ConfigurationProperty(NameAttributeName)]
    public string Name
    {
      get { return (string)base[NameAttributeName]; }
    }

    /// <summary>
    /// Gets the code snippet.
    /// </summary>
    [ConfigurationProperty(CodeSnippetAttributeName, IsRequired = true)]
    public string CodeSnippet
    {
      get { return (string)base[CodeSnippetAttributeName]; }
    }
  }
}
