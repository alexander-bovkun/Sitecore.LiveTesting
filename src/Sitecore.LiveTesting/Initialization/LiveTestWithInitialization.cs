namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Globalization;
  using System.Reflection;
  using System.Runtime.Remoting;
  using System.Runtime.Remoting.Activation;
  using System.Runtime.Remoting.Messaging;
  using System.Runtime.Remoting.Proxies;
  using System.Runtime.Remoting.Services;
  using System.Security.Permissions;
  using System.Threading;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the base class for attribute-aware tests.
  /// </summary>
  public class LiveTestWithInitialization : LiveTest
  {
    /// <summary>
    /// The test initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitialization"/> class.
    /// </summary>
    public LiveTestWithInitialization() : this(HostingEnvironment.IsHosted ? new InitializationManager(new TestInitializationActionDiscoverer(), new InitializationActionExecutor()) : null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitialization"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization manager.</param>
    protected LiveTestWithInitialization(InitializationManager initializationManager)
    {
      this.initializationManager = initializationManager;
    }

    /// <summary>
    /// Gets the initialization manager.
    /// </summary>
    protected InitializationManager InitializationManager
    {
      get { return this.initializationManager; }
    }

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static new LiveTestWithInitialization Instantiate(Type testType, params object[] arguments)
    {
      if (HostingEnvironment.IsHosted)
      {
        return (LiveTestWithInitialization)Activator.CreateInstance(testType, arguments);
      }

      if (InstantiatedByProxy(testType, arguments))
      {
        return Intercept(null, testType);
      }

      return (LiveTestWithInitialization)LiveTest.Instantiate(testType, arguments);
    }

    /// <summary>
    /// Determines if instance of the class is going to be instantiated by construction proxy or by other routines.
    /// </summary>
    /// <param name="testType">The test type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The value determining whether instance of the class is going to be instantiated by construction proxy or by other routines.</returns>
    protected static bool InstantiatedByProxy(Type testType, params object[] arguments)
    {
      return object.ReferenceEquals(DynamicConstructionAttribute.ArgumentsMarker, arguments);
    }

    /// <summary>
    /// Intercepts calls on the specified test.
    /// </summary>
    /// <param name="test">The test.</param>
    /// <param name="testType">Type of the test.</param>
    /// <returns>The test proxy.</returns>
    protected static LiveTestWithInitialization Intercept(LiveTestWithInitialization test, Type testType)
    {
      (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand();
      return (LiveTestWithInitialization)(new MethodCallInterceptor(test, testType)).GetTransparentProxy();
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected virtual void OnAfterMethodCall(object sender, MethodCallEventArgs args)
    {
      if (args == null)
      {
        throw new ArgumentNullException("args");
      }

      if (this.InitializationManager != null)
      {
        this.InitializationManager.Cleanup(args.MethodCallId, new TestInitializationContext(this, args.Method, args.Arguments));        
      }
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected virtual void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
    {
      if (args == null)
      {
        throw new ArgumentNullException("args");
      }

      if (this.InitializationManager != null)
      {
        this.InitializationManager.Initialize(args.MethodCallId, new TestInitializationContext(this, args.Method, args.Arguments));
      }
    }

    /// <summary>
    /// Defines the proxy class which intercepts all method calls.
    /// </summary>
    private class MethodCallInterceptor : RealProxy
    {
      /// <summary>
      /// The method call id.
      /// </summary>
      private static int methodCallId;

      /// <summary>
      /// The target test instance.
      /// </summary>
      private LiveTestWithInitialization target;

      /// <summary>
      /// Initializes a new instance of the <see cref="MethodCallInterceptor"/> class.
      /// </summary>
      /// <param name="target">The target object to intercept calls to.</param>
      /// <param name="testType">Type of the test.</param>
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
      public MethodCallInterceptor(LiveTestWithInitialization target, Type testType) : base(testType)
      {
        this.target = target;
      }

      /// <summary>
      /// Invokes the message.
      /// </summary>
      /// <param name="msg">The message.</param>
      /// <returns>The result of invocation.</returns>
      public override IMessage Invoke(IMessage msg)
      {
        IConstructionCallMessage constructorCall = msg as IConstructionCallMessage;

        if (constructorCall != null)
        {
          if (this.target == null)
          {
            MethodInfo factoryMethodInfo = Utility.GetInheritedMethod(constructorCall.ActivationType, DynamicConstructionAttribute.InstantiateMethodName, new[] { typeof(Type), typeof(object[]) });

            if (factoryMethodInfo == null)
            {
              throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", constructorCall.ActivationType.FullName, DynamicConstructionAttribute.InstantiateMethodName, typeof(LiveTest).FullName));
            }

            this.target = (LiveTestWithInitialization)factoryMethodInfo.Invoke(null, new object[] { constructorCall.ActivationType, constructorCall.Args });
          }

          return EnterpriseServicesHelper.CreateConstructionReturnMessage(constructorCall, (MarshalByRefObject)this.GetTransparentProxy());
        }

        IMethodCallMessage methodCall = msg as IMethodCallMessage;

        if (methodCall != null)
        {
          MethodCallEventArgs eventArgs = typeof(LiveTestWithInitialization).IsAssignableFrom(methodCall.MethodBase.DeclaringType) ? new MethodCallEventArgs(Interlocked.Increment(ref methodCallId), methodCall.MethodBase, methodCall.Args) : null;
          
          if (eventArgs != null)
          {
            this.target.OnBeforeMethodCall(this.target, eventArgs);
          }
          
          IMessage result = RemotingServices.GetRealProxy(this.target).Invoke(msg);

          if (eventArgs != null)
          {
            this.target.OnAfterMethodCall(this.target, eventArgs);
          }

          return result;
        }

        throw new NotSupportedException("Operations other than constructor and method calls are not supported.");
      }
    }
  }
}
