using Sitecore.LiveTesting.SpecFlowPlugin;
using TechTalk.SpecFlow.Infrastructure;

[assembly: GeneratorPlugin(typeof(LiveTestGeneratorPlugin))]

namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using BoDi;
  using TechTalk.SpecFlow.Generator.Configuration;
  using TechTalk.SpecFlow.Generator.Plugins;
  using TechTalk.SpecFlow.Generator.UnitTestConverter;

  /// <summary>
  /// Defines the LiveTestGeneratorPlugin class.
  /// </summary>
  public class LiveTestGeneratorPlugin : IGeneratorPlugin
  {
    /// <summary>
    /// Registers the dependencies.
    /// </summary>
    /// <param name="container">The container.</param>
    public void RegisterDependencies(ObjectContainer container)
    {
      container.RegisterTypeAs<LiveTestDecorator, ITestClassTagDecorator>("Sitecore.LiveTesting");
      container.RegisterTypeAs<LiveTestDecorator, ITestMethodTagDecorator>("Sitecore.LiveTesting");
      container.RegisterTypeAs<LiveTestFeatureGeneratorProvider, IFeatureGeneratorProvider>("default");
    }

    /// <summary>
    /// Registers the customizations.
    /// </summary>
    /// <param name="container">The container.</param>
    /// <param name="generatorConfiguration">The generator configuration.</param>
    public void RegisterCustomizations(ObjectContainer container, SpecFlowProjectConfiguration generatorConfiguration)
    {
    }

    /// <summary>
    /// Registers the configuration defaults.
    /// </summary>
    /// <param name="specFlowConfiguration">The spec flow configuration.</param>
    public void RegisterConfigurationDefaults(SpecFlowProjectConfiguration specFlowConfiguration)
    {
    }
  }
}
