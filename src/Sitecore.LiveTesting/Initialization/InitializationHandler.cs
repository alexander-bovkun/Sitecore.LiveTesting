namespace Sitecore.LiveTesting.Initialization
{
  using System;

  /// <summary>
  /// The request initialization handler.
  /// </summary>
  [Serializable]
  public class InitializationHandler
  {
    /// <summary>
    /// Initialization handler type.
    /// </summary>
    private readonly Type type;

    /// <summary>
    /// The arguments.
    /// </summary>
    private readonly object[] arguments;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationHandler"/> class.
    /// </summary>
    /// <param name="type">The initialization handler type.</param>
    /// <param name="arguments">The arguments.</param>
    public InitializationHandler(Type type, params object[] arguments)
    {
      if (type == null)
      {
        throw new ArgumentNullException("type");
      }

      if (arguments == null)
      {
        throw new ArgumentNullException("arguments");
      }

      this.type = type;
      this.arguments = arguments;
    }

    /// <summary>
    /// Gets the type of initialization handler.
    /// </summary>
    public Type Type
    {
      get { return this.type; }
    }

    /// <summary>
    /// Gets the arguments to be provided to initialization handler.
    /// </summary>
    public object[] Arguments
    {
      get { return this.arguments; }
    }    
  }
}
