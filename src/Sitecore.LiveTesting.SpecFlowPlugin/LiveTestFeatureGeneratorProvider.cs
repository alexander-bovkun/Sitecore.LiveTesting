namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using TechTalk.SpecFlow.Generator;
  using TechTalk.SpecFlow.Generator.UnitTestConverter;
  using TechTalk.SpecFlow.Parser.SyntaxElements;

  /// <summary>
  /// Defines the LiveTestFeatureGeneratorProvider class.
  /// </summary>
  public class LiveTestFeatureGeneratorProvider : IFeatureGeneratorProvider
  {
    /// <summary>
    /// The feature generator.
    /// </summary>
    private readonly IFeatureGenerator baseFeatureGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestFeatureGeneratorProvider" /> class.
    /// </summary>
    /// <param name="baseFeatureGenerator">The feature generator.</param>
    public LiveTestFeatureGeneratorProvider(UnitTestFeatureGenerator baseFeatureGenerator)
    {
      this.baseFeatureGenerator = baseFeatureGenerator;
    }

    /// <summary>
    /// Gets the priority.
    /// </summary>
    /// <value>
    /// The priority.
    /// </value>
    public int Priority
    {
      get
      {
        return int.MaxValue;
      }
    }

    /// <summary>
    /// Determines whether this instance can generate the specified feature.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns>
    ///   <c>true</c> if this instance can generate the specified feature; otherwise, <c>false</c>.
    /// </returns>
    public bool CanGenerate(Feature feature)
    {
      return true;
    }

    /// <summary>
    /// Creates the generator.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns>FeatureGenerator instance.</returns>
    public IFeatureGenerator CreateGenerator(Feature feature)
    {
      return new LiveTestFeatureGenerator(this.baseFeatureGenerator);
    }
  }
}
