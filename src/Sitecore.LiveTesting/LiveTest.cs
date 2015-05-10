namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.Configuration;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using System.Runtime.Remoting;
  using System.Runtime.Remoting.Activation;
  using System.Runtime.Remoting.Messaging;
  using System.Runtime.Remoting.Proxies;
  using System.Runtime.Remoting.Services;
  using System.Security.Permissions;
  using System.Threading;
  using System.Web.Hosting;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the base class for tests.
  /// </summary>
  [DynamicConstruction]
  public class LiveTest : ContextBoundObject
  {
    /// <summary>
    /// The default application id.
    /// </summary>
    private const string DefaultApplicationId = "Sitecore.LiveTesting.Default";

    /// <summary>
    /// The website path setting name.
    /// </summary>
    private const string WebsitePathSettingName = "Sitecore.LiveTesting.WebsitePath";

    /// <summary>
    /// The name of method that gets default application manager.
    /// </summary>
    private const string GetDefaultTestApplicationManagerName = "GetDefaultTestApplicationManager";

    /// <summary>
    /// The name of method that gets default application host.
    /// </summary>
    private const string GetDefaultApplicationHostName = "GetDefaultApplicationHost";

    /// <summary>
    /// The default test application manager.
    /// </summary>
    private static readonly TestApplicationManager DefaultTestApplicationManager = new TestApplicationManager();

    /// <summary>
    /// The test initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTest"/> class.
    /// </summary>
    public LiveTest() : this(HostingEnvironment.IsHosted ? new InitializationManager(new TestInitializationActionDiscoverer(), new InitializationActionExecutor()) : null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTest"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization manager.</param>
    protected LiveTest(InitializationManager initializationManager)
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
    /// Gets default test application manager for the specified test type.
    /// </summary>
    /// <param name="testType">The test type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>An instance of <see cref="DefaultTestApplicationManager"/>.</returns>
    public static TestApplicationManager GetDefaultTestApplicationManager(Type testType, params object[] arguments)
    {
      return DefaultTestApplicationManager;
    }

    /// <summary>
    /// Gets default application host for the specified test type.
    /// </summary>
    /// <param name="testType">Type of the test.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The default application host.</returns>
    public static TestApplicationHost GetDefaultApplicationHost(Type testType, params object[] arguments)
    {
      return new TestApplicationHost(DefaultApplicationId, "/", ConfigurationManager.AppSettings.Get(WebsitePathSettingName) ?? Directory.GetParent(Environment.CurrentDirectory).FullName);
    }

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static LiveTest Instantiate(Type testType, params object[] arguments)
    {
      if (HostingEnvironment.IsHosted)
      {
        return CreateUninitializedInstance(testType);
      }

      if (InstantiatedByProxy(testType, arguments))
      {
        return Intercept(testType, null);
      }

      MethodInfo getDefaultTestApplicationManagerMethod = Utility.GetInheritedMethod(testType, GetDefaultTestApplicationManagerName, new[] { typeof(Type), typeof(object[]) });
      MethodInfo getDefaultApplicationHostMethod = Utility.GetInheritedMethod(testType, GetDefaultApplicationHostName, new[] { typeof(Type), typeof(object[]) });

      if (getDefaultTestApplicationManagerMethod == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", testType.FullName, GetDefaultTestApplicationManagerName, typeof(LiveTest).FullName));
      }

      if (getDefaultApplicationHostMethod == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", testType.FullName, GetDefaultApplicationHostName, typeof(LiveTest).FullName));
      }

      object[] allArguments = { testType, arguments };

      TestApplicationManager testApplicationManager = (TestApplicationManager)getDefaultTestApplicationManagerMethod.Invoke(null, allArguments);

      if (testApplicationManager == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to get an instance of '{0}'.", typeof(TestApplicationManager).FullName));
      }

      TestApplicationHost host = (TestApplicationHost)getDefaultApplicationHostMethod.Invoke(null, allArguments);

      if (host == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to get an instance of '{0}'.", typeof(TestApplicationHost).FullName));
      }

      TestApplication testApplication = testApplicationManager.StartApplication(host);

      if (testApplication == null)
      {
        throw new InvalidOperationException("Failed to get application to execute tests in.");
      }

      return (LiveTest)testApplication.CreateObject(testType, arguments);
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
    /// Creates uninitialized test class instance.
    /// </summary>
    /// <param name="testType">The type of the test.</param>
    /// <returns>The test class instance.</returns>
    protected static LiveTest CreateUninitializedInstance(Type testType)
    {
      if (testType == null)
      {
        throw new ArgumentNullException("testType");
      }

      if (!typeof(LiveTest).IsAssignableFrom(testType))
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Only types derived from '{0}' are supported", typeof(LiveTest).FullName));
      }

      DynamicConstructionAttribute dynamicConstructionAttribute = (DynamicConstructionAttribute)testType.GetCustomAttributes(typeof(DynamicConstructionAttribute), true)[0];

      return (LiveTest)dynamicConstructionAttribute.CreateUninitializedInstance(testType);
    }

    /// <summary>
    /// Returns interceptor for the provided test type, and optionally, instance.
    /// </summary>
    /// <param name="testType">Type of the test.</param>
    /// <param name="test">The test instance.</param>
    /// <returns>The interceptor.</returns>
    protected static LiveTest Intercept(Type testType, LiveTest test)
    {
      (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand();
      return (LiveTest)(new MethodCallInterceptor(testType, test)).GetTransparentProxy();
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
    /// The attribute that enforces live execution of the test.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    private sealed class DynamicConstructionAttribute : ProxyAttribute
    {
      /// <summary>
      /// The name of instantiation method.
      /// </summary>
      internal const string InstantiateMethodName = "Instantiate";

      /// <summary>
      /// The arguments marker.
      /// </summary>
      internal static readonly object[] ArgumentsMarker = new object[0];

      /// <summary>
      /// The set of active threads.
      /// </summary>
      private static readonly IDictionary<int, bool> ActiveThreads = new Dictionary<int, bool>();

      /// <summary>
      /// Creates proxy instance.
      /// </summary>
      /// <param name="serverType">The type.</param>
      /// <returns>The proxy.</returns>
      public override MarshalByRefObject CreateInstance(Type serverType)
      {
        if (serverType == null)
        {
          throw new ArgumentNullException("serverType");
        }

        MethodInfo factoryMethodInfo = Utility.GetInheritedMethod(serverType, InstantiateMethodName, new[] { typeof(Type), typeof(object[]) });

        if (factoryMethodInfo == null)
        {
          throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because no static method '{1}' can be found in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", serverType.FullName, InstantiateMethodName, typeof(LiveTest).FullName));
        }

        if (!EnterInstantiationPhase())
        {
          return this.CreateUninitializedInstance(serverType);
        }
        
        MarshalByRefObject result;
        
        try
        {
          result = (MarshalByRefObject)factoryMethodInfo.Invoke(null, new object[] { serverType, ArgumentsMarker });
        }
        finally
        {
          LeaveInstantiationPhase();
        }

        if (result == null)
        {
          throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "The result of execution of factory method '{0}' from '{1}' must not be null", factoryMethodInfo.Name, factoryMethodInfo.DeclaringType.FullName));
        }

        return result;
      }

      /// <summary>
      /// Enters active instantiation phase.
      /// </summary>
      /// <returns>The value indicating whether it was succcessful attempt to enter initialization phase or not.</returns>
      internal static bool EnterInstantiationPhase()
      {
        lock (ActiveThreads)
        {
          if (ActiveThreads.ContainsKey(Thread.CurrentThread.ManagedThreadId))
          {
            return false;
          }

          ActiveThreads.Add(Thread.CurrentThread.ManagedThreadId, true);
          
          return true;
        }
      }

      /// <summary>
      /// Leaves active instantiation phase.
      /// </summary>
      internal static void LeaveInstantiationPhase()
      {
        lock (ActiveThreads)
        {
          ActiveThreads.Remove(Thread.CurrentThread.ManagedThreadId);
        }
      }

      /// <summary>
      /// Creates uninitialized proxy instance.
      /// </summary>
      /// <param name="serverType">The type.</param>
      /// <returns>The uninitialized proxy.</returns>      
      internal MarshalByRefObject CreateUninitializedInstance(Type serverType)
      {
        (new SecurityPermission(SecurityPermissionFlag.UnmanagedCode)).Demand();
        return base.CreateInstance(serverType);
      }
    }

    /// <summary>
    /// Defines the proxy class which intercepts all method calls.
    /// </summary>
    private sealed class MethodCallInterceptor : RealProxy
    {
      /// <summary>
      /// The method call id.
      /// </summary>
      private static int methodCallId;

      /// <summary>
      /// The target test instance.
      /// </summary>
      private LiveTest target;

      /// <summary>
      /// Initializes a new instance of the <see cref="MethodCallInterceptor"/> class.
      /// </summary>
      /// <param name="testType">Type of the test.</param>
      /// <param name="target">Target test instance.</param>
      [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
      internal MethodCallInterceptor(Type testType, LiveTest target) : base(testType)
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

            if (!DynamicConstructionAttribute.EnterInstantiationPhase())
            {
              throw new InvalidOperationException("Error in instantiation workflow. Object is already in the middle of instantiation when another instance of the object is created.");
            }

            try
            {
              this.target = (LiveTest)factoryMethodInfo.Invoke(null, new object[] { constructorCall.ActivationType, constructorCall.Args });
            }
            finally
            {
              DynamicConstructionAttribute.LeaveInstantiationPhase();
            }
          }

          return EnterpriseServicesHelper.CreateConstructionReturnMessage(constructorCall, (MarshalByRefObject)this.GetTransparentProxy());
        }

        IMethodCallMessage methodCall = msg as IMethodCallMessage;

        if (methodCall != null)
        {
          MethodCallEventArgs eventArgs = typeof(LiveTest).IsAssignableFrom(methodCall.MethodBase.DeclaringType) ? new MethodCallEventArgs(Interlocked.Increment(ref methodCallId), methodCall.MethodBase, methodCall.Args) : null;

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
