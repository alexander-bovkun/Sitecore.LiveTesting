namespace Sitecore.LiveTesting.Extensions.Initialization.Arguments
{
  using System;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class that describes how types are mapped to Sitecore configuration.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter, AllowMultiple = true)]
  public sealed class SitecoreConfigurationMapAttribute : Attribute
  {
    /// <summary>
    /// The argument type.
    /// </summary>
    private readonly Type argumentType;

    /// <summary>
    /// The configuration node.
    /// </summary>
    private readonly string configurationNode;

    /// <summary>
    /// Initializes a new instance of the <see cref="SitecoreConfigurationMapAttribute"/> class.
    /// </summary>
    /// <param name="argumentType">The argument type.</param>
    /// <param name="configurationNode">The configuration node.</param>
    public SitecoreConfigurationMapAttribute([NotNull] Type argumentType, [NotNull] string configurationNode)
    {
      Assert.ArgumentNotNull(argumentType, "argumentType");
      Assert.ArgumentNotNullOrEmpty(configurationNode, "configurationNode");

      this.argumentType = argumentType;
      this.configurationNode = configurationNode;
    }

    /// <summary>
    /// Gets the argument type.
    /// </summary>
    public Type ArgumentType
    {
      get { return this.argumentType; }
    }

    /// <summary>
    /// Gets the configuration node.
    /// </summary>
    public string ConfigurationNode
    {
      get { return this.configurationNode; }
    }
  }
}
