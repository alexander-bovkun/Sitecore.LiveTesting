namespace Sitecore.LiveTesting.Tests
{
  using System;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="LiveTest"/>
  /// </summary>
  public class LiveTestTest
  {
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
      LiveTest test = new TestWithCustomInstantiation();

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
    /// Should execute test in specially prepared application domain.
    /// </summary>
    [Fact]
    public void ShouldExecuteTestInSpeciallyPreparedApplicationDomain()
    {
      new RealLiveTest();
    }

    /// <summary>
    /// Defines the fake live test version.
    /// </summary>
    public class LiveTestBase : LiveTest
    {
      /// <summary>
      /// Instantiates the test class.
      /// </summary>
      /// <param name="type">Test type.</param>
      /// <returns>Instance of the test.</returns>
      public static new LiveTest Instantiate(Type type)
      {
        return (LiveTest)Activator.CreateInstance(type);
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

    /// <summary>
    /// The real live test.
    /// </summary>
    public class RealLiveTest : LiveTest
    {
    }
  }
}
