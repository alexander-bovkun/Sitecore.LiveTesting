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
    /// Initialization handler type.
    /// </summary>
    private readonly Type initializationHandlerType;

    /// <summary>
    /// The arguments.
    /// </summary>
    private readonly object[] arguments;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationHandlerAttribute"/> class.
    /// </summary>
    /// <param name="initializationHandlerType">The initialization handler type. The initialization handler should perform initialization in its constructor and cleanup in its <see cref="System.IDisposable.Dispose"/> method implementation.</param>
    /// <param name="arguments">The arguments to provide to initialization handler.</param>
    public InitializationHandlerAttribute(Type initializationHandlerType, params object[] arguments)
    {
      if (initializationHandlerType == null)
      {
        throw new ArgumentNullException("initializationHandlerType");
      }

      if (arguments == null)
      {
        throw new ArgumentNullException("arguments");
      }

      this.initializationHandlerType = initializationHandlerType;
      this.arguments = arguments;
    }

    /// <summary>
    /// Gets the type of initialization handler.
    /// </summary>
    public Type InitializationHandlerType
    {
      get { return this.initializationHandlerType; } 
    }

    /// <summary>
    /// Gets the arguments to be provided to initialization handler.
    /// </summary>
    public object[] Arguments
    {
      get { return this.arguments; }
    }

    /// <summary>
    /// Gets or sets the priority of initialization handler.
    /// </summary>
    public int Priority { get; set; }
  }
}
