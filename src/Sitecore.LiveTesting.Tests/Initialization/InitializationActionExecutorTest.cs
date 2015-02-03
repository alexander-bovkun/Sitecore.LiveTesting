namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="InitializationActionExecutor"/>
  /// </summary>
  public class InitializationActionExecutorTest
  {
    /// <summary>
    /// Should throw exception on initialization if action state is not a type.
    /// </summary>
    [Fact]
    public void ShouldThrowExceptionOnInitializationIfActionStateIsNotAType()
    {
      InitializationActionExecutor executor = new InitializationActionExecutor();
      InitializationAction initializationAction = new InitializationAction("System.String,mscorlib") { State = "a" };

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
      InitializationAction action = new InitializationAction("Action") { State = typeof(SimpleInitializer) };

      SimpleInitializer.Instance = null;
      
      executor.ExecuteInitializationForAction(action);

      Assert.NotNull(SimpleInitializer.Instance);
      Assert.Equal(SimpleInitializer.Instance, action.State);
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
        Instance = this;
      }

      /// <summary>
      /// Gets or sets the instance.
      /// </summary>
      public static SimpleInitializer Instance { get; set; }
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
  }
}
