namespace Sitecore.LiveTesting.Tests
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Runtime.Remoting;
  using System.Runtime.Remoting.Messaging;
  using System.Web;

  using NSubstitute;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="LiveTest"/>
  /// </summary>
  public class LiveTestTest
  {
    /// <summary>
    /// The initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;    

    /// <summary>
    /// The test application.
    /// </summary>
    private readonly TestApplication testApplication;

    /// <summary>
    /// The real test.
    /// </summary>
    private readonly LiveTest realTest;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestTest"/> class.
    /// </summary>
    public LiveTestTest()
    {
      this.testApplication = Substitute.For<TestApplication>();
      this.initializationManager = Substitute.For<InitializationManager>(Substitute.For<InitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>());

      LiveTestBase.TestApplicationManager.StartApplication(Arg.Is<TestApplicationHost>(host => (host.ApplicationId == "Sitecore.LiveTesting.Default") && (host.VirtualPath == "/") && (host.PhysicalPath == "..\\Website"))).Returns(this.testApplication);

      this.realTest = new TestWithCustomInstantiation(this.initializationManager);
    }

    /// <summary>
    /// Should recreate class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldCreateTestClass()
    {
      this.testApplication.CreateObject(typeof(LiveTestBase), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      LiveTestBase test = new LiveTestBase(this.initializationManager);

      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
    }

    /// <summary>
    /// Should use custom instantiation procedure.
    /// </summary>
    [Fact]
    public void ShouldUseCustomInstantiationProcedure()
    {
      LiveTestBase test = new TestWithCustomInstantiation(this.initializationManager);

      Assert.NotEqual(this.realTest, test);
      Assert.IsType<LiveTestBase>(test);
      Assert.IsNotType<TestWithCustomInstantiation>(test);
    }

    /// <summary>
    /// Should use instantiation from the base class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldUseInstantiationFromTheBaseClass()
    {
      this.testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      Test test = new Test(this.initializationManager);

      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
    }

    /// <summary>
    /// Should initialize and cleanup before and after method is called.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "It's ok to use it from tests")]
    [Fact]
    public void ShouldInitialzeAndCleanupBeforeAndAfterMethodIsCalled()
    {
      this.testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      Test test = new Test(this.initializationManager);

      test.TestSomething();

      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
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
      this.testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      Test test = new Test(this.initializationManager);

      Assert.ThrowsDelegate action = test.FailingTest;

      Assert.Throws<Exception>(action);
      Assert.Equal(this.realTest, RemotingServices.GetRealProxy(test).GetType().GetField("target", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(RemotingServices.GetRealProxy(test)));
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
      this.testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      Test test = new Test(this.initializationManager);

      this.initializationManager.WhenForAnyArgs(manager => manager.Initialize(0, null)).Throw<Exception>();

      Assert.ThrowsDelegate action = test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Should crash if test cleanup crashes.
    /// </summary>
    [Fact]
    public void ShouldCrashIfTestCleanupCrashes()
    {
      this.testApplication.CreateObject(typeof(Test), Arg.Is<object[]>(args => (args != null) && (args.Length == 1) && (args[0] == this.initializationManager))).Returns(this.realTest);

      Test test = new Test(this.initializationManager);

      this.initializationManager.WhenForAnyArgs(manager => manager.Cleanup(0, null)).Throw<Exception>();

      Assert.ThrowsDelegate action = test.TestSomething;

      Assert.Throws<Exception>(action);
    }

    /// <summary>
    /// Constructor should be called only once by infrastructure.
    /// </summary>
    [Fact]
    public void ConstructorShouldBeCalledOnlyOnceByInfrastructure()
    {
      LiveTestWithConstructorCallCount.ResetCounter();

      LiveTestWithConstructorCallCount test = new LiveTestWithConstructorCallCount();

      Assert.Equal(1, test.ConstructorCallCount);
    }

    /// <summary>
    /// Should be able to create uninitialized test instance.
    /// </summary>
    [Fact]
    public void ShouldBeAbleToCreateUninitializedTestInstance()
    {
      Test uninitializedTest = (Test)Test.CreateUninitializedTestInstance(typeof(Test));
      Assert.ThrowsDelegate action = uninitializedTest.TestSomething; 

      Assert.NotNull(uninitializedTest);
      Assert.Throws<RemotingException>(action);
    }

    /// <summary>
    /// Should create uninitialized test instances only for classes derived from <see cref="LiveTest"/>.
    /// </summary>
    [Fact]
    public void ShouldCreateUninitializedTestInstancesOnlyForClassesDerivedFromLiveTest()
    {
      Assert.ThrowsDelegate action = () => Test.CreateUninitializedTestInstance(typeof(string));

      Assert.Throws<ArgumentException>(action);
    }

    /// <summary>
    /// Should support argument substitution.
    /// </summary>
    [Fact]
    public void ShouldSupportArgumentSubstitution()
    {
      LiveTestWithArgumentSubstitution test = new LiveTestWithArgumentSubstitution();

      test.TestMethod("arg");
    }

    /// <summary>
    /// Defines the fake live test version.
    /// </summary>
    public class LiveTestBase : LiveTest
    {
      /// <summary>
      /// Initializes static members of the <see cref="LiveTestBase"/> class.
      /// </summary>
      static LiveTestBase()
      {
        TestApplicationManager = Substitute.For<TestApplicationManager>();
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="LiveTestBase"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public LiveTestBase(InitializationManager initializationManager) : base(initializationManager)
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
    }

    /// <summary>
    /// Defines the test with custom instantiation.
    /// </summary>
    public class TestWithCustomInstantiation : LiveTestBase
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="TestWithCustomInstantiation"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public TestWithCustomInstantiation(InitializationManager initializationManager) : base(initializationManager)
      {
      }

      /// <summary>
      /// Creates an instance of the test class.
      /// </summary>
      /// <param name="type">Type of the test to instantiate.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>An instance of test class.</returns>
      public static new LiveTest Instantiate(Type type, params object[] arguments)
      {
        return new LiveTestBase(Substitute.For<InitializationManager>(Substitute.For<InitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>()));
      }
    }

    /// <summary>
    /// Defines the sample test class.
    /// </summary>
    public class Test : LiveTestBase
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="Test"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public Test(InitializationManager initializationManager) : base(initializationManager)
      {
      }

      /// <summary>
      /// Creates uninitialized test class instance.
      /// </summary>
      /// <param name="testType">The type of the test.</param>
      /// <returns>The test class instance.</returns>
      public static LiveTest CreateUninitializedTestInstance(Type testType)
      {
        return LiveTest.CreateUninitializedInstance(testType);
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
    /// Defines live test with constructor call count.
    /// </summary>
    public class LiveTestWithConstructorCallCount : LiveTest
    {
      /// <summary>
      /// The counter.
      /// </summary>
      private static int counter;

      /// <summary>
      /// Initializes a new instance of the <see cref="LiveTestWithConstructorCallCount"/> class.
      /// </summary>
      public LiveTestWithConstructorCallCount()
      {
        ++counter;
      }

      /// <summary>
      /// Gets the number of constructor calls.
      /// </summary>
      public int ConstructorCallCount
      {
        get { return counter; }
      }

      /// <summary>
      /// Sets the counter to its initial value of 0.
      /// </summary>
      public static void ResetCounter()
      {
        counter = 0;
      }
    }

    /// <summary>
    /// Defines sample test class that substitutes its test method arguments.
    /// </summary>
    public class LiveTestWithArgumentSubstitution : LiveTest
    {
      /// <summary>
      /// Creates an instance of the test class.
      /// </summary>
      /// <param name="type">Type of the test to instantiate.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>An instance of test class.</returns>
      public static new LiveTest Instantiate(Type type, params object[] arguments)
      {
        return LiveTest.Intercept(type, new LiveTestWithArgumentSubstitution());
      }

      /// <summary>
      /// The test method.
      /// </summary>
      /// <param name="argument">The argument.</param>
      public void TestMethod(string argument)
      {
        Assert.Equal("arg*", argument);
      }

      /// <summary>
      /// The routine called before test method call.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The arguments.</param>
      protected override void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
      {
        args.Arguments[0] = args.Arguments[0].ToString() + '*';
      }
    }

    /// <summary>
    /// Defines the test class with scenarios related to CallContext.
    /// </summary>
    public class CallContextTest : LiveTest
    {
      /// <summary>
      /// Should share call context information between calls
      /// </summary>
      [Fact]
      public void ShouldShareCallContextInformationBetweenCalls()
      {
        Assert.Equal("testValue", CallContext.LogicalGetData("test1"));
        CallContext.LogicalSetData("test2", "testValue");
      }

      /// <summary>
      /// The routine called before test method call.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The arguments.</param>
      protected override void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
      {
        CallContext.LogicalSetData("test1", "testValue");
      }

      /// <summary>
      /// The routine called after test method call.
      /// </summary>
      /// <param name="sender">The sender.</param>
      /// <param name="args">The arguments.</param>
      protected override void OnAfterMethodCall(object sender, MethodCallEventArgs args)
      {
        Assert.Equal("testValue", CallContext.LogicalGetData("test1"));
        Assert.Equal("testValue", CallContext.LogicalGetData("test2"));
      }
    }
  }
}
