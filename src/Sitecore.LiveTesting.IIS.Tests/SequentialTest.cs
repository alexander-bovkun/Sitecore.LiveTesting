namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the base class for all of the tests that should be executed sequentially.
  /// </summary>
  [InitializationHandler(typeof(SequentialInitializationHandler), Priority = int.MinValue)]
  public class SequentialTest : LiveTest
  {
    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static new LiveTest Instantiate(Type testType, params object[] arguments)
    {
      if (LiveTest.InstantiatedByProxy(testType, arguments))
      {
        return LiveTest.Intercept(testType, null);
      }

      return (LiveTest)Activator.CreateInstance(testType, arguments);
    }
  }
}
