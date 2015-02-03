namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Reflection;
  using System.Runtime.Serialization;
  using System.Security.Permissions;

  /// <summary>
  /// Defines the event arguments for method call interception.
  /// </summary>
  [Serializable]
  public class MethodCallEventArgs : EventArgs, ISerializable
  {
    /// <summary>
    /// The method call id.
    /// </summary>
    private readonly int methodCallId;

    /// <summary>
    /// The method.
    /// </summary>
    private readonly MethodBase method;

    /// <summary>
    /// The arguments.
    /// </summary>
    private readonly object[] arguments;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCallEventArgs"/> class.
    /// </summary>
    /// <param name="methodCallId">The method call id.</param>
    /// <param name="method">The method.</param>
    /// <param name="arguments">The arguments.</param>
    public MethodCallEventArgs(int methodCallId, MethodBase method, object[] arguments)
    {
      if (method == null)
      {
        throw new ArgumentNullException("method");
      }

      if (arguments == null)
      {
        throw new ArgumentNullException("arguments");
      }

      this.methodCallId = methodCallId;
      this.method = method;
      this.arguments = arguments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodCallEventArgs"/> class.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The serialization context.</param>
    protected MethodCallEventArgs(SerializationInfo info, StreamingContext context)
    {
      this.methodCallId = info.GetInt32("MethodCallId");
      this.method = (MethodBase)info.GetValue("Method", typeof(MethodBase));
      
      if (this.method == null)
      {
        throw new ArgumentException("SerializationInfo is not properly formed: it is missing the value for the 'Method' key.");
      }

      this.arguments = (object[])info.GetValue("Arguments", typeof(object[]));

      if (this.arguments == null)
      {
        throw new ArgumentException("SerializationInfo is not properly formed: it is missing the value for the 'Arguments' key.");
      }
    }

    /// <summary>
    /// Gets the method call id.
    /// </summary>
    public int MethodCallId
    {
      get { return this.methodCallId; }
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

    /// <summary>
    /// Gets object data.
    /// </summary>
    /// <param name="info">The serialization info.</param>
    /// <param name="context">The serialization context.</param>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      info.AddValue("MethodCallId", this.MethodCallId);
      info.AddValue("Method", this.Method);
      info.AddValue("Arguments", this.Arguments);
    }
  }
}
