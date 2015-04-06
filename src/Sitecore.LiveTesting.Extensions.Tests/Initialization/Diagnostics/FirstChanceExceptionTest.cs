namespace Sitecore.LiveTesting.Extensions.Tests.Initialization.Diagnostics
{
  using System;
  using System.Reflection;
  using System.Runtime.ExceptionServices;
  using Sitecore.LiveTesting.Extensions.Initialization.Diagnostics;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="FirstChanceException"/>.
  /// </summary>
  public class FirstChanceExceptionTest
  {
    /// <summary>
    /// Gets the first chance exception subscribers.
    /// </summary>
    private static EventHandler<FirstChanceExceptionEventArgs> FirstChanceExceptionSubscribers
    {
      get { return (EventHandler<FirstChanceExceptionEventArgs>)typeof(AppDomain).GetField("_firstChanceException", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(AppDomain.CurrentDomain); }
    }

    /// <summary>
    /// Should subscribe to first chance exception on construction and unsubscribe on disposal.
    /// </summary>
    [Fact]
    public void ShouldSubscribeToFirstChanceExceptionOnConstructionAndUnsubscribeOnDisposal()
    {
      FirstChanceException initializationHandler = new TestFirstChanceException();

      using (initializationHandler)
      {
        Assert.Equal(initializationHandler, FirstChanceExceptionSubscribers.Target);
      }

      Assert.Null(FirstChanceExceptionSubscribers);
    }

    /// <summary>
    /// The test first chance exception.
    /// </summary>
    private class TestFirstChanceException : FirstChanceException
    {
      /// <summary>
      /// On first chance exception event handler.
      /// </summary>
      /// <param name="firstChanceExceptionEventArgs">The first chance exception event args.</param>
      protected override void OnFirstChanceException(FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
      {
      }
    }
  }
}
