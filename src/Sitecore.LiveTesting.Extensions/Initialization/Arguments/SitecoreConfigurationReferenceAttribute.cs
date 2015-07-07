namespace Sitecore.LiveTesting.Extensions.Initialization.Arguments
{
  using System;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class used to define the set of Sitecore configuratoin parts used to resolve argument values.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
  public sealed class SitecoreConfigurationReferenceAttribute : Attribute
  {
    /// <summary>
    /// The configuration node.
    /// </summary>
    private readonly string configurationNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreConfigurationReferenceAttribute"/> class.
    /// </summary>
    /// <param name="configurationNode">The configuration node.</param>
    public SitecoreConfigurationReferenceAttribute([NotNull] string configurationNode)
    {
      Assert.ArgumentNotNullOrEmpty(configurationNode, "configurationNode");

      this.configurationNode = configurationNode;
    }

    /// <summary>
    /// Gets the configuration node.
    /// </summary>
    [NotNull]
    public string ConfigurationNode
    {
      get { return this.configurationNode; }
    }
  }
}
