namespace Sitecore.LiveTesting.Tests
{
  using System;
  using NSubstitute;
  using Sitecore.LiveTesting.Applications;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="LiveTest"/>
  /// </summary>
  public class LiveTestTest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestTest"/> class.
    /// </summary>
    public LiveTestTest()
    {
      TestApplication testApplication = Substitute.For<TestApplication>();
      
      testApplication.CreateObject(null, null).ReturnsForAnyArgs(callInfo => Activator.CreateInstance(callInfo.Arg<Type>(), callInfo.Arg<object[]>()));
      LiveTestBase.TestApplicationManager.StartApplication(Arg.Any<TestApplicationHost>()).Returns(testApplication);
    }

    /// <summary>
    /// Should recreate class.
    /// </summary>
    [Fact]
    public void ShouldCreateTestClass()
    {
      Assert.IsType<LiveTestBase>(new LiveTestBase());
    }

    /// <summary>
    /// Should use custom instantiation procedure.
    /// </summary>
    [Fact]
    public void ShouldUseCustomInstantiationProcedure()
    {
      LiveTestBase test = new TestWithCustomInstantiation();

      Assert.IsType<LiveTestBase>(test);
      Assert.IsNotType<TestWithCustomInstantiation>(test);
    }

    /// <summary>
    /// Should use instantiation from the base class.
    /// </summary>
    [Fact]
    public void ShouldUseInstantiationFromTheBaseClass()
    {
      Assert.IsType<Test>(new Test());
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
      /// Gets the test application manager.
      /// </summary>
      public static TestApplicationManager TestApplicationManager { get; private set; }

      /// <summary>
      /// Gets the default test application manager.
      /// </summary>
      /// <param name="type">Test type.</param>
      /// <returns>Instance of the test application manager.</returns>
      public static new TestApplicationManager GetDefaultTestApplicationManager(Type type)
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
      /// Creates an instance of the test class.
      /// </summary>
      /// <param name="type">Type of the test to instantiate.</param>
      /// <returns>An instance of test class.</returns>
      public static new LiveTest Instantiate(Type type)
      {
        return new LiveTestBase();
      }
    }

    /// <summary>
    /// Defines the sample test class.
    /// </summary>
    public class Test : LiveTestBase
    {
    }
  }
}
