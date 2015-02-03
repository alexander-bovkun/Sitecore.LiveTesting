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
      Test.InitializationManager = Substitute.For<InitializationManager>(Substitute.For<InitializationActionDiscoverer>(), Substitute.For<InitializationActionExecutor>());
    }

    /// <summary>
    /// The interface for sample remote test.
    /// </summary>
    private interface ISampleRemoteTest
    {
      /// <summary>
      /// The sample test method.
      /// </summary>
      /// <param name="argument">The argument.</param>
      void TestSomething(int argument);
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
      int methodCallId = (int)Test.InitializationManager.ReceivedCalls().First().GetArguments()[1];
      Test.InitializationManager.Received().Initialize(test, methodCallId, typeof(Test).GetMethod("TestSomething"), Arg.Is<object[]>(array => array.Length == 0));
      Test.InitializationManager.Received().Cleanup(test, methodCallId, typeof(Test).GetMethod("TestSomething"), Arg.Is<object[]>(array => array.Length == 0));
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
      int methodCallId = (int)Test.InitializationManager.ReceivedCalls().First().GetArguments()[1];
      Test.InitializationManager.Received().Initialize(test, methodCallId, typeof(Test).GetMethod("FailingTest"), Arg.Is<object[]>(array => array.Length == 0));
      Test.InitializationManager.Received().Cleanup(test, methodCallId, typeof(Test).GetMethod("FailingTest"), Arg.Is<object[]>(array => array.Length == 0));
    }

    /// <summary>
    /// Should not fail when actual test instance is remote proxy.
    /// </summary>
    [Fact]
    public void ShouldNotFailWhenActualTestInstanceIsRemoteProxy()
    {
      ISampleRemoteTest test = new RemoteTest();

      test.TestSomething(1);
    }

    /// <summary>
    /// Defines a typical test class example.
    /// </summary>
    public class Test : LiveTestWithInitialization
    {
      /// <summary>
      /// Gets or sets test initialization manager.
      /// </summary>
      public static InitializationManager InitializationManager { get; set; }

      /// <summary>
      /// Gets or sets real test.
      /// </summary>
      public static LiveTestWithInitialization RealTest { get; set; }

      /// <summary>
      /// Creates an instance of corresponding class.
      /// </summary>
      /// <param name="testType">Type of the test to instantiate.</param>
      /// <returns>Instance of the class.</returns>    
      public static new LiveTestWithInitialization Instantiate(Type testType)
      {
        LiveTestWithInitialization test = (LiveTestWithInitialization)Activator.CreateInstance(testType);
        RealTest = test;
        return Intercept(test, testType);
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

      /// <summary>
      /// Gets test initialization manager.
      /// </summary>
      /// <returns>Test initialization manager.</returns>
      protected override InitializationManager GetTestInitializationManager()
      {
        return InitializationManager;
      }
    }

    /// <summary>
    /// Defines a typical test class example.
    /// </summary>
    public class RemoteTest : LiveTestWithInitialization, ISampleRemoteTest
    {
      /// <summary>
      /// The test application domain.
      /// </summary>
      private static readonly AppDomain TestAppDomain = AppDomain.CreateDomain("TestAppDomain", null, new AppDomainSetup { ApplicationBase = Environment.CurrentDirectory });

      /// <summary>
      /// Creates an instance of corresponding class.
      /// </summary>
      /// <param name="testType">Type of the test to instantiate.</param>
      /// <returns>Instance of the class.</returns>    
      public static new LiveTestWithInitialization Instantiate(Type testType)
      {
        LiveTestWithInitialization test = (LiveTestWithInitialization)TestAppDomain.CreateInstanceAndUnwrap(typeof(RealLiveTest).Assembly.FullName, typeof(RealLiveTest).FullName);
        return Intercept(test, testType);
      }

      /// <summary>
      /// Typical test method example.
      /// </summary>
      /// <param name="argument">The test argument.</param>
      public void TestSomething(int argument)
      {
      }
    }

    /// <summary>
    /// Defines the real live test class which will be instantiated in order to avoid <see cref="StackOverflowException"/>.
    /// </summary>
    public class RealLiveTest : LiveTestWithInitialization, ISampleRemoteTest
    {
      /// <summary>
      /// Creates an instance of corresponding class.
      /// </summary>
      /// <param name="testType">Type of the test to instantiate.</param>
      /// <returns>Instance of the class.</returns>    
      public static new LiveTestWithInitialization Instantiate(Type testType)
      {
        return (LiveTestWithInitialization)Activator.CreateInstance(testType);
      }

      /// <summary>
      /// Typical test method example.
      /// </summary>
      /// <param name="argument">The test argument.</param>
      public void TestSomething(int argument)
      {
      }
    }
  }
}
