namespace Sitecore.LiveTesting.Extensions.Tests.Initialization.Diagnostics
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.ExceptionServices;
  using NSubstitute;
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
    /// Should subscribe to first chance exception on construction, call abstract method on event and unsubscribe on disposal.
    /// </summary>
    [Fact]
    public void ShouldSubscribeToFirstChanceExceptionOnConstructionCallAbstractMethodOnEventAndUnsubscribeOnDisposal()
    {
      FirstChanceException initializationHandler = Substitute.ForPartsOf<FirstChanceException>();

      using (initializationHandler)
      {
        Assert.Equal(initializationHandler, FirstChanceExceptionSubscribers.Target);

        Exception exception = new Exception();

        FirstChanceExceptionSubscribers(AppDomain.CurrentDomain, new FirstChanceExceptionEventArgs(exception));

        Assert.Equal(1, initializationHandler.ReceivedCalls().Count());
        Assert.Equal(initializationHandler, initializationHandler.ReceivedCalls().Single().Target());
        Assert.Equal(1, initializationHandler.ReceivedCalls().Single().GetArguments().Length);
        Assert.IsType<FirstChanceExceptionEventArgs>(initializationHandler.ReceivedCalls().Single().GetArguments()[0]);
        Assert.Equal(exception, ((FirstChanceExceptionEventArgs)initializationHandler.ReceivedCalls().Single().GetArguments()[0]).Exception);
      }

      Assert.Null(FirstChanceExceptionSubscribers);
    }
  }
}
