namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using NSubstitute;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="InitializationManager"/>
  /// </summary>  
  public class InitializationManagerTest
  {
    /// <summary>
    /// The action discoverer.
    /// </summary>
    private readonly TestInitializationActionDiscoverer actionDiscoverer;

    /// <summary>
    /// The action executor.
    /// </summary>
    private readonly InitializationActionExecutor actionExecutor;

    /// <summary>
    /// The manager.
    /// </summary>
    private InitializationManager manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationManagerTest"/> class.
    /// </summary>
    public InitializationManagerTest()
    {
      this.actionDiscoverer = Substitute.For<TestInitializationActionDiscoverer>();
      this.actionExecutor = Substitute.For<InitializationActionExecutor>();
      this.manager = new InitializationManager(this.actionDiscoverer, this.actionExecutor);
    }

    /// <summary>
    /// Should discover and execute initializers in order on initialization.
    /// </summary>
    [Fact]
    public void ShouldDiscoverAndExecuteInitializersInOrderOnInitialization()
    {
      const int MethodCallId = 123;

      TestInitializationContext context = new TestInitializationContext(null, typeof(string).GetMethod("Intern"), new object[0]);

      this.actionDiscoverer.GetInitializationActions(context).Returns(new[] { new InitializationAction("Action1"), new InitializationAction("Action2") });

      this.manager.Initialize(MethodCallId, context);

      Received.InOrder(
        () =>
          {
            this.actionExecutor.ExecuteInitializationForAction(Arg.Is<InitializationAction>(action => action.Id == "Action1"));
            this.actionExecutor.ExecuteInitializationForAction(Arg.Is<InitializationAction>(action => action.Id == "Action2"));
          });
    }

    /// <summary>
    /// Should throw exception on initialization if called with the same method call id.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionOnInitializationIfCalledWithTheSameMethodCallId()
    {
      const int MethodCallId = 123;

      TestInitializationContext context = new TestInitializationContext(null, typeof(string).GetMethod("Intern"), new object[0]);

      this.actionDiscoverer.GetInitializationActions(context).Returns(new[] { new InitializationAction("Action1"), new InitializationAction("Action2") });

      this.manager.Initialize(MethodCallId, context);
      Assert.ThrowsDelegate action = () => this.manager.Initialize(MethodCallId, context);

      Assert.Throws<InvalidOperationException>(action);
    }

    /// <summary>
    /// Should execute initializers in reverse order on cleanup.
    /// </summary>
    [Fact]
    public void ShouldExecuteInitializersInReverseOrderOnCleanup()
    {
      const int MethodCallId = 123;

      this.manager = new InitializationManager(this.actionDiscoverer, this.actionExecutor, new Dictionary<int, IList<InitializationAction>> { { MethodCallId, new[] { new InitializationAction("Action1"), new InitializationAction("Action2") } } });

      this.manager.Cleanup(MethodCallId, null);

      Received.InOrder(
        () =>
        {
          this.actionExecutor.ExecuteCleanupForAction(Arg.Is<InitializationAction>(action => action.Id == "Action2"));
          this.actionExecutor.ExecuteCleanupForAction(Arg.Is<InitializationAction>(action => action.Id == "Action1"));
        });
    }

    /// <summary>
    /// Should throw invalid operation exception on cleanup if actions collection is empty.
    /// </summary>
    [Fact]
    public void ShouldThrowInvalidOperationExceptionOnCleanupIfActionsCollectionIsEmpty()
    {
      const int MethodCallId = 123;

      Assert.ThrowsDelegate action = () => this.manager.Cleanup(MethodCallId, null);

      Assert.Throws<InvalidOperationException>(action);
    }
  }
}
