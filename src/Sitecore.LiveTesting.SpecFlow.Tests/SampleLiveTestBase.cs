namespace Sitecore.LiveTesting.SpecFlow.Tests
{
  using System;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the sample base class for all live tests.
  /// </summary>
  public class SampleLiveTestBase : LiveTest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="SampleLiveTestBase"/> class.
    /// </summary>
    public SampleLiveTestBase() : base(new InitializationManager(new TestInitializationActionDiscoverer(), new InitializationActionExecutor()))
    {
    }

    /// <summary>
    /// Instantiates the test class.
    /// </summary>
    /// <param name="testType">Type of the test class.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the test class.</returns>
    public static new LiveTest Instantiate(Type testType, params object[] arguments)
    {
      return LiveTest.Intercept((LiveTest)Activator.CreateInstance(testType), testType);
    }

    /// <summary>
    /// The get default application host.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The <see cref="TestApplicationHost"/>.</returns>
    public static new TestApplicationHost GetDefaultApplicationHost(Type type, params object[] arguments)
    {
      return null;
    }
  }
}
