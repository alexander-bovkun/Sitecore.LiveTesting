namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="TestInitializationActionDiscoverer"/>
  /// </summary>
  public class TestInitializationActionDiscovererTest
  {
    /// <summary>
    /// Should discover actions by initialization handler attributes.
    /// </summary>
    [Fact]
    public void ShouldDiscoverActionsByInitializationHandlerAttributes()
    {
      const string Argument = "argument";

      TestInitializationActionDiscoverer actionDiscoverer = new TestInitializationActionDiscoverer();
      Test test = new Test();

      IEnumerable<InitializationAction> actions = actionDiscoverer.GetInitializationActions(new TestInitializationContext(test, typeof(Test).GetMethod("TestMethod"), new object[] { Argument })).ToArray();

      Assert.Equal(1, actions.Count());

      TestInitializationContext expectedInitializationContext = new TestInitializationContext(test, typeof(Test).GetMethod("TestMethod"), new object[] { Argument });
      
      Assert.Equal(typeof(InitializationHandler1).AssemblyQualifiedName, actions.Single().Id);
      Assert.IsType<InitializationHandler>(actions.Single().State);
      Assert.Equal(typeof(InitializationHandler1), ((InitializationHandler)actions.Single().State).Type);
      Assert.Equal(new object[] { "parameter" }, ((InitializationHandler)actions.Single().State).Arguments);
      Assert.Equal(expectedInitializationContext.Instance, ((TestInitializationContext)actions.Single().Context).Instance);
      Assert.Equal(expectedInitializationContext.Method, ((TestInitializationContext)actions.Single().Context).Method);
      Assert.Equal(expectedInitializationContext.Arguments, ((TestInitializationContext)actions.Single().Context).Arguments);
    }

    /// <summary>
    /// Should take into the account the priority.
    /// </summary>
    [Fact]
    public void ShouldTakeIntoTheAccoutThePriority()
    {
      TestInitializationActionDiscoverer actionDiscoverer = new TestInitializationActionDiscoverer();
      Test test = new Test();

      IEnumerable<InitializationAction> actions = actionDiscoverer.GetInitializationActions(new TestInitializationContext(test, typeof(Test).GetMethod("TestMethodWithPrioritizedInitializationHandler"), new object[0])).ToArray();

      Assert.Equal(3, actions.Count());
      Assert.Equal(typeof(InitializationHandler2).AssemblyQualifiedName, actions.First().Id);
      Assert.Equal(typeof(InitializationHandler2), ((InitializationHandler)actions.First().State).Type);
      Assert.Equal(typeof(InitializationHandler1).AssemblyQualifiedName, actions.ElementAt(1).Id);
      Assert.Equal(typeof(InitializationHandler1), ((InitializationHandler)actions.ElementAt(1).State).Type);
      Assert.Equal(typeof(InitializationHandler2).AssemblyQualifiedName, actions.ElementAt(2).Id);
      Assert.Equal(typeof(InitializationHandler2), ((InitializationHandler)actions.ElementAt(2).State).Type);
    }

    /// <summary>
    /// Should throw not supported exception if called with other than initialization context type of context.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionIfCalledWithOtherThanInitializationContextTypeOfContext()
    {
      TestInitializationActionDiscoverer actionDiscoverer = new TestInitializationActionDiscoverer();

      Assert.ThrowsDelegate action = () => actionDiscoverer.GetInitializationActions(new object());

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Defines typical test example.
    /// </summary>
    [InitializationHandler(typeof(InitializationHandler1), "parameter")]
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

      /// <summary>
      /// Another sample test method
      /// </summary>
      [InitializationHandler(typeof(InitializationHandler2), Priority = 100)]
      [InitializationHandler(typeof(InitializationHandler2), Priority = -100)]
      public void TestMethodWithPrioritizedInitializationHandler()
      {
      }
    }

    /// <summary>
    /// Defines sample initialization handler.
    /// </summary>
    public class InitializationHandler1
    {
    }

    /// <summary>
    /// Defines sample initialization handler.
    /// </summary>
    public class InitializationHandler2
    {
    }
  }
}
