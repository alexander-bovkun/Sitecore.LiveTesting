namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;
  using Xunit.Extensions;

  /// <summary>
  /// Defines the test class for <see cref="InitializationActionExecutor"/>
  /// </summary>
  public class InitializationActionExecutorTest
  {
    /// <summary>
    /// Should throw exception on initialization if action state is not an array.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionOnInitializationIfActionStateIsNotAnArray()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction initializationAction = new InitializationAction("System.String,mscorlib") { State = "a" };

      Assert.ThrowsDelegate action = () => executor.ExecuteInitializationForAction(initializationAction);

      Assert.Throws<ArgumentException>(action);
    }

    /// <summary>
    /// Should throw exception on initialization if state array has less than 2 elements.
    /// </summary>
    /// <param name="numberOfElements">The number of elements.</param>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    public void ShouldThrowExceptionOnInitializationIfStateArrayHasLessThan2Elements(int numberOfElements)
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction initializationAction = new InitializationAction("System.String,mscorlib") { State = new object[numberOfElements] };

      Assert.ThrowsDelegate action = () => executor.ExecuteInitializationForAction(initializationAction);

      Assert.Throws<ArgumentException>(action);
    }

    /// <summary>
    /// Should throw exception on initialization if first state element is not a type.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionOnInitializationIfFirstStateElementIsNotAType()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction initializationAction = new InitializationAction("System.String,mscorlib") { State = new object[] { "a", "b" } };

      Assert.ThrowsDelegate action = () => executor.ExecuteInitializationForAction(initializationAction);

      Assert.Throws<ArgumentException>(action);      
    }

    /// <summary>
    /// Should throw exception on initialization if second state element is not an object array.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionOnInitializationIfSecondStateElementIsNotAnObjectArray()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction initializationAction = new InitializationAction("System.String,mscorlib") { State = new object[] { typeof(SimpleInitializer), "a" } };

      Assert.ThrowsDelegate action = () => executor.ExecuteInitializationForAction(initializationAction);

      Assert.Throws<ArgumentException>(action);
    }

    /// <summary>
    /// Should create initializer instance on initialization and save it into actions state.
    /// </summary>
    [Fact]
    public void ShouldCreateInitializerInstanceOnInitializationAndSaveItIntoActionsState()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction action = new InitializationAction("Action") { State = new object[] { typeof(SimpleInitializer), new object[0] } };

      SimpleInitializer.Parameter = null;
      
      executor.ExecuteInitializationForAction(action);

      Assert.NotNull(SimpleInitializer.Parameter);
      Assert.Equal(string.Empty, SimpleInitializer.Parameter);
    }

    /// <summary>
    /// Should create initializer instance using parameters on initialization and save it into actions state.
    /// </summary>
    /// <param name="initializerType">The initializer Type.</param>
    [Theory]
    [InlineData(typeof(SimpleInitializer))]
    [InlineData(typeof(SimpleInitializerWithVariableNumberOfParameters))]
    public void ShouldCreateInitializerInstanceUsingParametersOnInitializationAndSaveItIntoActionsState(Type initializerType)
    {
      const string Parameter = "parameter";

      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction action = new InitializationAction("Action") { State = new object[] { initializerType, new object[] { Parameter } } };

      SimpleInitializer.Parameter = null;

      executor.ExecuteInitializationForAction(action);

      Assert.NotNull(SimpleInitializer.Parameter);
      Assert.Equal(Parameter, SimpleInitializer.Parameter);
    }

    /// <summary>
    /// Should set initialization context if initialization handler implements IInitializationContextAware.
    /// </summary>
    [Fact]
    public void ShouldSetInitializationContextIfInitializationHandlerImplementsIInitializationContextAware()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationContext context = new InitializationContext(null, typeof(string).GetMethod("Intern"), new object[0]);
      InitializationAction action = new InitializationAction("Action") { State = new object[] { typeof(InitializationContextAwareInitializer), new object[0] }, Context = context };

      InitializationContextAwareInitializer.InitializationContext = null;

      executor.ExecuteInitializationForAction(action);

      Assert.Equal(context, InitializationContextAwareInitializer.InitializationContext);
    }

    /// <summary>
    /// Should dispose initializer instance on cleanup.
    /// </summary>
    [Fact]
    public void ShouldDisposeInitializerInstanceOnCleanup()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction action = new InitializationAction("Action") { State = new DisposibleInitializer() };

      DisposibleInitializer.Disposed = false;

      executor.ExecuteCleanupForAction(action);

      Assert.True(DisposibleInitializer.Disposed);
    }

    /// <summary>
    /// Should do nothing on cleanup if initializer instance cannot be disposed.
    /// </summary>
    [Fact]
    public void ShouldDoNothingOnCleanupIfInitializerInstanceCannotBeDisposed()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction action = new InitializationAction("Action") { State = new SimpleInitializer() };

      executor.ExecuteCleanupForAction(action);
    }

    /// <summary>
    /// Should do nothing on cleanup if initializer instance is not set.
    /// </summary>
    [Fact]
    public void ShouldDoNothingOnCleanupIfInitializerInstanceIsNotSet()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction action = new InitializationAction("Action");

      executor.ExecuteCleanupForAction(action);
    }

    /// <summary>
    /// Defines a typical initializer without cleanup support.
    /// </summary>
    public class SimpleInitializer
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="SimpleInitializer"/> class.
      /// </summary>
      public SimpleInitializer()
      {
        Parameter = string.Empty;
      }
      
      /// <summary>
      /// Initializes a new instance of the <see cref="SimpleInitializer"/> class.
      /// </summary>
      /// <param name="parameter">The parameter.</param>
      public SimpleInitializer(string parameter)
      {
        Parameter = parameter;
      }

      /// <summary>
      /// Gets or sets the parameters.
      /// </summary>
      public static string Parameter { get; set; }
    }

    /// <summary>
    /// Defines a typical initializer which receives variable number of parameters in its constructor.
    /// </summary>
    public class SimpleInitializerWithVariableNumberOfParameters : SimpleInitializer
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="SimpleInitializerWithVariableNumberOfParameters"/> class.
      /// </summary>
      /// <param name="parameters">The parameters.</param>
      public SimpleInitializerWithVariableNumberOfParameters(params object[] parameters) : base((string)parameters[0])
      {        
      }
    }

    /// <summary>
    /// Defines a typical initializer with cleanup support.
    /// </summary>
    public sealed class DisposibleInitializer : IDisposable
    {
      /// <summary>
      /// Gets or sets a value indicating whether object was disposed or not.
      /// </summary>
      public static bool Disposed { get; set; }

      /// <summary>
      /// Disposes the instance.
      /// </summary>
      public void Dispose()
      {
        Disposed = true;
      }
    }

    /// <summary>
    /// Defines a typical initialization handler which is aware of its execution context.
    /// </summary>
    public class InitializationContextAwareInitializer : IInitializationContextAware
    {
      /// <summary>
      /// Gets or sets the initialization context.
      /// </summary>
      public static InitializationContext InitializationContext { get; set; }

      /// <summary>
      /// The set initialization context.
      /// </summary>
      /// <param name="context">The context.</param>
      public void SetInitializationContext(InitializationContext context)
      {
        InitializationContext = context;
      }
    }
  }
}
