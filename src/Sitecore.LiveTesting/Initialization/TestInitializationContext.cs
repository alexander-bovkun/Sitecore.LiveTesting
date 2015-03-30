namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Reflection;

  /// <summary>
  /// Defines the initialization context which provides additional information about the test being executed to initialization handlers.
  /// </summary>
  public class TestInitializationContext
  {
    /// <summary>
    /// The instance.
    /// </summary>
    private readonly object instance;

    /// <summary>
    /// The method.
    /// </summary>
    private readonly MethodBase method;

    /// <summary>
    /// The arguments.
    /// </summary>
    private readonly object[] arguments;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestInitializationContext"/> class.
    /// </summary>
    /// <param name="instance">The instance.</param>
    /// <param name="method">The method.</param>
    /// <param name="arguments">The arguments.</param>
    public TestInitializationContext(object instance, MethodBase method, object[] arguments)
    {
      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      this.instance = instance;
      this.method = method;
      this.arguments = arguments;
    }

    /// <summary>
    /// Gets the instance.
    /// </summary>
    public object Instance
    {
      get { return this.instance; }
    }

    /// <summary>
    /// Gets the method.
    /// </summary>
    public MethodBase Method
    {
      get { return this.method; }
    }

    /// <summary>
    /// Gets the arguments.
    /// </summary>
    public object[] Arguments
    {
      get { return this.arguments; }
    }
  }
}
