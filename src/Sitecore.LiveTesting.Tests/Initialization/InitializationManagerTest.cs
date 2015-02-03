namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using NSubstitute;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="InitializationManager"/>
  /// </summary>  
  public class TestInitializationManagerTest
  {
    /// <summary>
    /// The action discoverer.
    /// </summary>
    private readonly InitializationActionDiscoverer actionDiscoverer;

    /// <summary>
    /// The action executor.
    /// </summary>
    private readonly InitializationActionExecutor actionExecutor;

    /// <summary>
    /// The manager.
    /// </summary>
    private InitializationManager manager;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInitializationManagerTest"/> class.
    /// </summary>
    public TestInitializationManagerTest()
    {
      this.actionDiscoverer = Substitute.For<InitializationActionDiscoverer>();
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

      MethodInfo testMethod = typeof(Test).GetMethod("TestMethod");
      Test test = new Test();

      this.actionDiscoverer.GetInitializationActions(test, testMethod, Arg.Is<object[]>(args => args.Length == 0)).Returns(new[] { new InitializationAction("Action1"), new InitializationAction("Action2") });

      this.manager.Initialize(test, MethodCallId, testMethod, new object[0]);

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

      MethodInfo testMethod = typeof(Test).GetMethod("TestMethod");
      Test test = new Test();

      this.actionDiscoverer.GetInitializationActions(test, testMethod, Arg.Is<object[]>(args => args.Length == 0)).Returns(new[] { new InitializationAction("Action1"), new InitializationAction("Action2") });

      this.manager.Initialize(test, MethodCallId, testMethod, new object[0]);
      Assert.ThrowsDelegate action = () => this.manager.Initialize(test, MethodCallId, testMethod, new object[0]);

      Assert.Throws<InvalidOperationException>(action);
    }

    /// <summary>
    /// Should execute initializers in reverse order on cleanup.
    /// </summary>
    [Fact]
    public void ShouldExecuteInitializersInReverseOrderOnCleanup()
    {
      const int MethodCallId = 123;

      MethodInfo testMethod = typeof(Test).GetMethod("TestMethod");
      Test test = new Test();

      this.manager = new InitializationManager(this.actionDiscoverer, this.actionExecutor, new Dictionary<int, IList<InitializationAction>>() { { MethodCallId, new InitializationAction[] { new InitializationAction("Action1"), new InitializationAction("Action2") } } });

      this.manager.Cleanup(test, MethodCallId, testMethod, new object[0]);

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

      MethodInfo testMethod = typeof(Test).GetMethod("TestMethod");
      Test test = new Test();

      Assert.ThrowsDelegate action = () => this.manager.Cleanup(test, MethodCallId, testMethod, new object[0]);

      Assert.Throws<InvalidOperationException>(action);
    }

    /// <summary>
    /// Defines a typical test example.
    /// </summary>
    public class Test : LiveTestWithInitialization
    {
      /// <summary>
      /// Instantiates the test class.
      /// </summary>
      /// <param name="testType">Type of the test class.</param>
      /// <returns>Instance of the test class.</returns>
      public static new LiveTestWithInitialization Instantiate(Type testType)
      {
        return new Test();
      }

      /// <summary>
      /// Sample test method.
      /// </summary>
      public void TestMethod()
      {
      }
    }
  }
}
