namespace Sitecore.LiveTesting.SpecFlow.Tests
{
  using System;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the sample base class for all live tests.
  /// </summary>
  public class SampleLiveTestBase : LiveTestWithInitialization
  {
    /// <summary>
    /// Instantiates the test class.
    /// </summary>
    /// <param name="testType">Type of the test class.</param>
    /// <returns>Instance of the test class.</returns>
    public static new LiveTestWithInitialization Instantiate(Type testType)
    {
      return Intercept((LiveTestWithInitialization)Activator.CreateInstance(testType), testType);
    }

    /// <summary>
    /// The get default application host.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns>The <see cref="ApplicationHost"/>.</returns>
    public static new ApplicationHost GetDefaultApplicationHost(Type type)
    {
      return null;
    }
  }
}
