namespace Sitecore.LiveTesting.Tests.Initialization.Arguments
{
  using System;
  using System.Collections;
  using System.Linq;
  using System.Reflection;

  using NSubstitute;

  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;
  using Sitecore.LiveTesting.Initialization.Arguments;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="ArgumentProvider"/>.
  /// </summary>
  public class ArgumentProviderTest
  {
    /// <summary>
    /// The provider.
    /// </summary>
    private readonly TestArgumentProvider provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArgumentProviderTest"/> class.
    /// </summary>
    public ArgumentProviderTest()
    {
      this.provider = Substitute.For<TestArgumentProvider>();
    }

    /// <summary>
    /// Should throw not supported exception for other contexts than test initialization context.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionForOtherContextsThanTestInitializationContext()
    {
      Assert.ThrowsDelegate action = () => this.provider.SetInitializationContext(new TestApplicationInitializationContext(new TestApplication()));

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Should enumerate and substitute values for appropriate parameters.
    /// </summary>
    [Fact]
    public void ShouldEnumerateAndSubstituteValuesForAppropriateParameters()
    {
      Action<IComparable, IEnumerable, IDisposable> testDelegate = this.TestMethod;
      object[] intactParameter = new object[0];
      object[] arguments = { null, intactParameter, null };
      TestInitializationContext context = new TestInitializationContext(testDelegate.Target, testDelegate.Method, arguments);

      this.provider.SetInitializationContext(context);

      Assert.Equal(5, this.provider.ReceivedCalls().Count());

      this.provider.Received().TestNeedToProvideValue(Arg.Is<object>(obj => obj == null), Arg.Is<ParameterInfo>(parameter => parameter.Position == 0));
      this.provider.Received().TestNeedToProvideValue(Arg.Is<object[]>(obj => obj.Length == 0), Arg.Is<ParameterInfo>(parameter => parameter.Position == 1));
      this.provider.Received().TestNeedToProvideValue(Arg.Is<object>(obj => obj == null), Arg.Is<ParameterInfo>(parameter => parameter.Position == 2));

      this.provider.Received().TestResolveValue(Arg.Is<object>(obj => obj == null), Arg.Is<ParameterInfo>(parameter => parameter.Position == 0));
      this.provider.DidNotReceive().TestResolveValue(Arg.Is<object[]>(obj => obj.Length == 0), Arg.Is<ParameterInfo>(parameter => parameter.Position == 1));
      this.provider.Received().TestResolveValue(Arg.Is<object>(obj => obj == null), Arg.Is<ParameterInfo>(parameter => parameter.Position == 2));
    }

    /// <summary>
    /// The test method.
    /// </summary>
    /// <param name="c">The c.</param>
    /// <param name="e">The e.</param>
    /// <param name="d">The d.</param>
    private void TestMethod(IComparable c, IEnumerable e, IDisposable d)
    {
    }

    /// <summary>
    /// Defines the sample argument provider.
    /// </summary>
    public abstract class TestArgumentProvider : ArgumentProvider
    {
      /// <summary>
      /// Determines if other value than the specified one should be provided.
      /// </summary>
      /// <param name="value">The initial value.</param>
      /// <param name="parameter">The parameter to which new value will be passed.</param>
      /// <returns><value>true</value> if other than initial value needs to be specified, otherwise <value>false</value>.</returns>
      public abstract bool TestNeedToProvideValue(object value, ParameterInfo parameter);

      /// <summary>
      /// Resolves the value for the parameter.
      /// </summary>
      /// <param name="value">The initial value.</param>
      /// <param name="parameter">The parameter.</param>
      /// <returns>The new value.</returns>
      public abstract object TestResolveValue(object value, ParameterInfo parameter);

      /// <summary>
      /// Determines if other value than the specified one should be provided.
      /// </summary>
      /// <param name="value">The initial value.</param>
      /// <param name="parameter">The parameter to which new value will be passed.</param>
      /// <returns><value>true</value> if other than initial value needs to be specified, otherwise <value>false</value>.</returns>
      protected sealed override bool NeedToProvideValue(object value, ParameterInfo parameter)
      {
        this.TestNeedToProvideValue(value, parameter);
        
        return base.NeedToProvideValue(value, parameter);
      }

      /// <summary>
      /// Resolves the value for the parameter.
      /// </summary>
      /// <param name="value">The initial value.</param>
      /// <param name="parameter">The parameter.</param>
      /// <returns>The new value.</returns>
      protected sealed override object ResolveValue(object value, ParameterInfo parameter)
      {
        return this.TestResolveValue(value, parameter);
      }
    }
  }
}
