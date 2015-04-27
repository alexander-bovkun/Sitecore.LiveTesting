namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.Remoting;
  using NSubstitute;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="LiveTestWithInitializationTest"/>
  /// </summary>
  public class LiveTestWithInitializationTest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitializationTest"/> class.
    /// </summary>
    public LiveTestWithInitializationTest()
    {
      Test.Manager = Substitute.For<InitializationManager>(Substitute.For<TestInitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>());
    }

    /// <summary>
    /// Should initialize and cleanup before and after method is called.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldInitialzeAndCleanupBeforeAndAfterMethodIsCalled()
    {
      Test test = new Test();
      
      test.TestSomething();

      Assert.Equal(Test.RealTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
      int methodCallId = (int)Test.Manager.ReceivedCalls().First().GetArguments()[0];
      Test.Manager.Received().Initialize(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == Test.RealTest) && (c.Method == typeof(Test).GetMethod("TestSomething") && (c.Arguments.Length == 0))));
      Test.Manager.Received().Cleanup(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == Test.RealTest) && (c.Method == typeof(Test).GetMethod("TestSomething") && (c.Arguments.Length == 0))));
    }

    /// <summary>
    /// Should perform cleanup even after exception.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldPerformCleanupEvenAfterException()
    {
      Test test = new Test();

      Assert.ThrowsDelegate action = test.FailingTest;

      Assert.Throws<Exception>(action);
      Assert.Equal(Test.RealTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
      int methodCallId = (int)Test.Manager.ReceivedCalls().First().GetArguments()[0];
      Test.Manager.Received().Initialize(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == Test.RealTest) && (c.Method == typeof(Test).GetMethod("FailingTest")) && (c.Arguments.Length == 0)));
      Test.Manager.Received().Cleanup(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == Test.RealTest) && (c.Method == typeof(Test).GetMethod("FailingTest")) && (c.Arguments.Length == 0)));
    }

    /// <summary>
    /// Should crash if test initialization crashes.
    /// </summary>
    [Fact]
    public void ShouldCrashIfTestInitializationCrashes()
    {
      Test.Manager.WhenForAnyArgs(manager => manager.Initialize(0, null)).Throw<Exception>();
      Test test = new Test();

      Assert.ThrowsDelegate action = test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Should crash if test cleanup crashes.
    /// </summary>
    [Fact]
    public void ShouldCrashIfTestCleanupCrashes()
    {
      Test.Manager.WhenForAnyArgs(manager => manager.Cleanup(0, null)).Throw<Exception>();
      Test test = new Test();

      Assert.ThrowsDelegate action = test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Defines a typical test class example.
    /// </summary>
    public class Test : LiveTestWithInitialization
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="Test"/> class.
      /// </summary>
      public Test() : base(Manager)
      {
      }

      /// <summary>
      /// Gets or sets test initialization manager.
      /// </summary>
      public static InitializationManager Manager { get; set; }

      /// <summary>
      /// Gets or sets real test.
      /// </summary>
      public static LiveTestWithInitialization RealTest { get; set; }

      /// <summary>
      /// Creates an instance of corresponding class.
      /// </summary>
      /// <param name="testType">Type of the test to instantiate.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>Instance of the class.</returns>
      public static new LiveTestWithInitialization Instantiate(Type testType, params object[] arguments)
      {
        LiveTestWithInitialization test = (LiveTestWithInitialization)Activator.CreateInstance(testType, arguments);
        RealTest = test;
        return LiveTestWithInitialization.Intercept(test, testType);
      }

      /// <summary>
      /// Typical test method example.
      /// </summary>
      public void TestSomething()
      {
      }

      /// <summary>
      /// Typical failing test.
      /// </summary>
      public void FailingTest()
      {
        throw new Exception();
      }
    }
  }
}
