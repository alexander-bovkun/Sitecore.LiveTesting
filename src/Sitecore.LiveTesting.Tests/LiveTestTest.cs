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

      LiveTestBase.TestApplicationManager.StartApplication(Arg.Is<TestApplicationHost>(host => (host.ApplicationId == "Sitecore.LiveTesting.Default") && (host.VirtualPath == "/") && (host.PhysicalPath == "..\\Website"))).Returns(this.testApplication);

      this.realTest = new TestWithCustomInstantiation();
    }

    /// <summary>
    /// Should recreate class.
    /// </summary>
    [Fact]
    public void ShouldCreateTestClass()
    {
      this.testApplication.CreateObject(typeof(LiveTestBase), Arg.Any<object[]>()).Returns(this.realTest);

      Assert.Equal(this.realTest, new LiveTestBase());
    }

    /// <summary>
    /// Should use custom instantiation procedure.
    /// </summary>
    [Fact]
    public void ShouldUseCustomInstantiationProcedure()
    {
      LiveTestBase test = new TestWithCustomInstantiation();

      Assert.NotEqual(this.realTest, test);
      Assert.IsType<LiveTestBase>(test);
      Assert.IsNotType<TestWithCustomInstantiation>(test);
    }

    /// <summary>
    /// Should use instantiation from the base class.
    /// </summary>
    [Fact]
    public void ShouldUseInstantiationFromTheBaseClass()
    {
      this.testApplication.CreateObject(typeof(Test), Arg.Any<object[]>()).Returns(this.realTest);

      Assert.Equal(this.realTest, new Test());
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
      /// Creates an instance of the test class.
      /// </summary>
      /// <param name="type">Type of the test to instantiate.</param>
      /// <param name="arguments">The arguments.</param>
      /// <returns>An instance of test class.</returns>
      public static new LiveTest Instantiate(Type type, params object[] arguments)
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
