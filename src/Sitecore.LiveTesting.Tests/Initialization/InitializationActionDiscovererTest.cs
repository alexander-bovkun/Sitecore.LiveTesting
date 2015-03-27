namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="InitializationActionDiscoverer"/>
  /// </summary>
  public class InitializationActionDiscovererTest
  {
    /// <summary>
    /// Should discover actions by initialization handler attributes.
    /// </summary>
    [Fact]
    public void ShouldDiscoverActionsByInitializationHandlerAttributes()
    {
      InitializationActionDiscoverer actionDiscoverer = new InitializationActionDiscoverer();
      Test test = new Test();

      IEnumerable<InitializationAction> actions = actionDiscoverer.GetInitializationActions(test, typeof(Test).GetMethod("TestMethod"), new object[0]).ToArray();

      Assert.Equal(2, actions.Count());
      Assert.Equal(typeof(InitializationHandler2).AssemblyQualifiedName, actions.First().Id);
      Assert.IsType<object[]>(actions.First().State);
      Assert.Equal(typeof(InitializationHandler2), ((object[])actions.First().State)[0]);
      Assert.Empty((object[])((object[])actions.First().State)[1]);
      Assert.Equal(typeof(InitializationHandler1).AssemblyQualifiedName, actions.ElementAt(1).Id);
      Assert.IsType<object[]>(actions.ElementAt(1).State);
      Assert.Equal(typeof(InitializationHandler1), ((object[])actions.ElementAt(1).State)[0]);
      Assert.Equal(new object[] { "parameter" }, ((object[])actions.ElementAt(1).State)[1]);
    }

    /// <summary>
    /// Should take into the account the priority.
    /// </summary>
    [Fact]
    public void ShouldTakeIntoTheAccoutThePriority()
    {
      InitializationActionDiscoverer actionDiscoverer = new InitializationActionDiscoverer();
      Test test = new Test();

      IEnumerable<InitializationAction> actions = actionDiscoverer.GetInitializationActions(test, typeof(Test).GetMethod("TestMethodWithPrioritizedInitializationHandler"), new object[0]).ToArray();

      Assert.Equal(3, actions.Count());
      Assert.Equal(typeof(InitializationHandler2).AssemblyQualifiedName, actions.First().Id);
      Assert.Equal(typeof(InitializationHandler2), ((object[])actions.First().State)[0]);
      Assert.Equal(typeof(InitializationHandler1).AssemblyQualifiedName, actions.ElementAt(1).Id);
      Assert.Equal(typeof(InitializationHandler1), ((object[])actions.ElementAt(1).State)[0]);
      Assert.Equal(typeof(InitializationHandler2).AssemblyQualifiedName, actions.ElementAt(2).Id);
      Assert.Equal(typeof(InitializationHandler2), ((object[])actions.ElementAt(2).State)[0]);
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
      [InitializationHandler(typeof(InitializationHandler2))]
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
