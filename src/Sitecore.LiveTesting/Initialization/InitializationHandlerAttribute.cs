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
    /// Initializes a new instance of the <see cref="InitializationHandlerAttribute"/> class.
    /// </summary>
    /// <param name="initializationHandlerType">The initialization handler type. The initialization handler should perform initialization in its constructor and cleanup in its <see cref="System.IDisposable.Dispose"/> method implementation.</param>
    public InitializationHandlerAttribute(Type initializationHandlerType)
    {
      if (initializationHandlerType == null)
      {
        throw new ArgumentNullException("initializationHandlerType");
      }

      this.initializationHandlerType = initializationHandlerType;
    }

    /// <summary>
    /// Gets the type of initialization handler.
    /// </summary>
    public Type InitializationHandlerType
    {
      get { return this.initializationHandlerType; } 
    }

    /// <summary>
    /// Gets or sets the priority of initialization handler.
    /// </summary>
    public int Priority { get; set; }
  }
}
