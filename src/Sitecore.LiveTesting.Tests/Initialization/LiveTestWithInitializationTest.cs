namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.Remoting;
  using NSubstitute;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="LiveTestWithInitializationTest"/>
  /// </summary>
  public class LiveTestWithInitializationTest
  {
    /// <summary>
    /// The initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// The real test.
    /// </summary>
    private readonly RealTest realTest;
    
    /// <summary>
    /// The test.
    /// </summary>
    private readonly Test test;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitializationTest"/> class.
    /// </summary>
    public LiveTestWithInitializationTest()
    {
      TestApplication testApplication = Substitute.For<TestApplication>();
      this.initializationManager = Substitute.For<InitializationManager>(Substitute.For<InitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>());

      Test.TestApplicationManager.StartApplication(Arg.Any<TestApplicationHost>()).Returns(testApplication);

      this.realTest = new RealTest(this.initializationManager);
      testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(arguments => (arguments != null) && (arguments.Length == 1))).Returns(this.realTest);

      this.test = new Test(this.initializationManager);
    }

    /// <summary>
    /// Should initialize and cleanup before and after method is called.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldInitialzeAndCleanupBeforeAndAfterMethodIsCalled()
    {
      this.test.TestSomething();

      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(this.test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(this.test)));
      int methodCallId = (int)this.initializationManager.ReceivedCalls().First().GetArguments()[0];
      this.initializationManager.Received().Initialize(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == this.realTest) && (c.Method == typeof(Test).GetMethod("TestSomething") && (c.Arguments.Length == 0))));
      this.initializationManager.Received().Cleanup(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == this.realTest) && (c.Method == typeof(Test).GetMethod("TestSomething") && (c.Arguments.Length == 0))));
    }

    /// <summary>
    /// Should perform cleanup even after exception.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldPerformCleanupEvenAfterException()
    {
      Assert.ThrowsDelegate action = this.test.FailingTest;

      Assert.Throws<Exception>(action);
      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(this.test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(this.test)));
      int methodCallId = (int)this.initializationManager.ReceivedCalls().First().GetArguments()[0];
      this.initializationManager.Received().Initialize(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == this.realTest) && (c.Method == typeof(Test).GetMethod("FailingTest")) && (c.Arguments.Length == 0)));
      this.initializationManager.Received().Cleanup(methodCallId, Arg.Is<TestInitializationContext>(c => (c.Instance == this.realTest) && (c.Method == typeof(Test).GetMethod("FailingTest")) && (c.Arguments.Length == 0)));
    }

    /// <summary>
    /// Should crash if test initialization crashes.
    /// </summary>
    [Fact]
    public void ShouldCrashIfTestInitializationCrashes()
    {
      this.initializationManager.WhenForAnyArgs(manager => manager.Initialize(0, null)).Throw<Exception>();

      Assert.ThrowsDelegate action = this.test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Should crash if test cleanup crashes.
    /// </summary>
    [Fact]
    public void ShouldCrashIfTestCleanupCrashes()
    {
      this.initializationManager.WhenForAnyArgs(manager => manager.Cleanup(0, null)).Throw<Exception>();

      Assert.ThrowsDelegate action = this.test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Defines a typical test class example.
    /// </summary>
    public class Test : LiveTestWithInitialization
    {
      /// <summary>
      /// Initializes static members of the <see cref="LiveTestWithInitializationTest.Test"/> class.
      /// </summary>
      static Test()
      {
        TestApplicationManager = Substitute.For<TestApplicationManager>();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="Test"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public Test(InitializationManager initializationManager) : base(initializationManager)
      {
      }

      /// <summary>
      /// Gets the test application manager.
      /// </summary>
      public static TestApplicationManager TestApplicationManager { get; private set; }

      /// <summary>
      /// Gets the default test application manager.
      /// </summary>
      /// <param name="type">Test type.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>Instance of the test application manager.</returns>
      public static new TestApplicationManager GetDefaultTestApplicationManager(Type type, params object[] arguments)
      {
        return TestApplicationManager;
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

    /// <summary>
    /// Defines the real test.
    /// </summary>
    public class RealTest : Test
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="RealTest"/> class.
      /// </summary>
      public RealTest() : this(Substitute.For<InitializationManager>(Substitute.For<InitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>()))
      {
      }
      
      /// <summary>
      /// Initializes a new instance of the <see cref="RealTest"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public RealTest(InitializationManager initializationManager) : base(initializationManager)
      {
      }

      /// <summary>
      /// Creates an instance of corresponding class.
      /// </summary>
      /// <param name="testType">Type of the test to instantiate.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>Instance of the class.</returns>
      public static new LiveTest Instantiate(Type testType, params object[] arguments)
      {
        return (LiveTest)Activator.CreateInstance(testType, arguments);
      }
    }
  }
}
