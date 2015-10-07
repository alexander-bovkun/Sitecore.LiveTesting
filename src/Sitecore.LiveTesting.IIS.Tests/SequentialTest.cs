namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using System.Threading;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the base class for all of the tests that should be executed sequentially.
  /// </summary>
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

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected override void OnAfterMethodCall(object sender, MethodCallEventArgs args)
    {
      base.OnAfterMethodCall(sender, args);
      Monitor.Exit(typeof(SequentialTest));
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected override void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
    {
      Monitor.Enter(typeof(SequentialTest));
      base.OnBeforeMethodCall(sender, args);
    }
  }
}
