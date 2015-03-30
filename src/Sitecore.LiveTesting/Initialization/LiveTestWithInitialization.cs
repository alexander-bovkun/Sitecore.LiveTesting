namespace Sitecore.LiveTesting.Initialization
{
  using System;
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
    private InitializationManager initializationManager;

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <returns>Instance of the class.</returns>    
    public static new LiveTestWithInitialization Instantiate(Type testType)
    {
      LiveTestWithInitialization test = (LiveTestWithInitialization)LiveTest.Instantiate(testType);

      if (!HostingEnvironment.IsHosted)
      {
        test = Intercept(test, testType);        
      }

      return test;
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
    /// Gets an instance of <see cref="InitializationManager"/>.
    /// </summary>
    /// <returns>An instance of <see cref="InitializationManager"/>.</returns>
    protected virtual InitializationManager GetTestInitializationManager()
    {
      return this.initializationManager = this.initializationManager ?? new InitializationManager(new TestInitializationActionDiscoverer(), new InitializationActionExecutor());
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected virtual void OnAfterMethodCall(object sender, MethodCallEventArgs args)
    {
      this.GetTestInitializationManager().Cleanup(args.MethodCallId, new TestInitializationContext(this, args.Method, args.Arguments));
    }

    /// <summary>
    /// Event handler executed before each method call.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    protected virtual void OnBeforeMethodCall(object sender, MethodCallEventArgs args)
    {
      this.GetTestInitializationManager().Initialize(args.MethodCallId, new TestInitializationContext(this, args.Method, args.Arguments));
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
      private readonly LiveTestWithInitialization target;

      /// <summary>
      /// Initializes a new instance of the <see cref="MethodCallInterceptor"/> class.
      /// </summary>
      /// <param name="target">The target object to intercept calls to.</param>
      /// <param name="testType">Type of the test.</param>
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
      public MethodCallInterceptor(LiveTestWithInitialization target, Type testType) : base(testType)
      {
        if (target == null)
        {
          throw new ArgumentNullException("target");
        }

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
