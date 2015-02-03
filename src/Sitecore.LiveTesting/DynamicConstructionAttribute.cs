namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using System.Runtime.Remoting.Proxies;
  using System.Threading;

  /// <summary>
  /// The attribute that enforces live execution of the test.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  internal sealed class DynamicConstructionAttribute : ProxyAttribute
  {
    /// <summary>
    /// The name of instantiation method.
    /// </summary>
    private const string InstantiateMethodName = "Instantiate";

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
      bool instantiating;

      lock (ActiveThreads)
      {
        instantiating = ActiveThreads.ContainsKey(Thread.CurrentThread.ManagedThreadId);
      }

      if (!instantiating)
      {
        MethodInfo factoryMethodInfo = Utility.GetInheritedMethod(serverType, InstantiateMethodName, new[] { typeof(Type) });
        
        if (factoryMethodInfo != null)
        {
          lock (ActiveThreads)
          {
            ActiveThreads.Add(Thread.CurrentThread.ManagedThreadId, true);
          }

          MarshalByRefObject result = (MarshalByRefObject)factoryMethodInfo.Invoke(null, new object[] { serverType });
          
          lock (ActiveThreads)
          {
            ActiveThreads.Remove(Thread.CurrentThread.ManagedThreadId);            
          }

          return result;
        }
      }

      return base.CreateInstance(serverType);
    }
  }
}
