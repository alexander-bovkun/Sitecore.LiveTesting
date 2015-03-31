namespace Sitecore.LiveTesting.Initialization
{
  using System;

  /// <summary>
  /// Defines the attribute that points to initialization handler types.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
  public class InitializationHandlerAttribute : Attribute
  {
    /// <summary>
    /// The initialization handler.
    /// </summary>
    private readonly InitializationHandler initializationHandler;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationHandlerAttribute"/> class.
    /// </summary>
    /// <param name="initializationHandlerType">The initialization handler type. The initialization handler should perform initialization in its constructor and cleanup in its <see cref="System.IDisposable.Dispose"/> method implementation.</param>
    /// <param name="arguments">The arguments to provide to initialization handler.</param>
    public InitializationHandlerAttribute(Type initializationHandlerType, params object[] arguments)
    {
      this.initializationHandler = new InitializationHandler(initializationHandlerType, arguments);
    }

    /// <summary>
    /// Gets the initialization handler.
    /// </summary>
    public InitializationHandler InitializationHandler
    {
      get { return this.initializationHandler; }
    }

    /// <summary>
    /// Gets or sets the priority of initialization handler.
    /// </summary>
    public int Priority { get; set; }
  }
}
