namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;

  /// <summary>
  /// Defines the class responsible for initialization before and after each test runs.
  /// </summary>
  public class InitializationManager
  {
    /// <summary>
    /// The initialization attribute discoverer.
    /// </summary>
    private readonly InitializationActionDiscoverer initializationAttributeDiscoverer;

    /// <summary>
    /// The initialization attribute executor.
    /// </summary>
    private readonly InitializationActionExecutor initializationAttributeExecutor;

    /// <summary>
    /// The actions.
    /// </summary>
    private readonly IDictionary<int, IList<InitializationAction>> actions;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationManager"/> class.
    /// </summary>
    /// <param name="initializationAttributeDiscoverer">The initialization attribute discoverer.</param>
    /// <param name="initializationAttributeExecutor">The initialization attribute executor.</param>
    /// <param name="actions">The action container.</param>
    public InitializationManager(InitializationActionDiscoverer initializationAttributeDiscoverer, InitializationActionExecutor initializationAttributeExecutor, IDictionary<int, IList<InitializationAction>> actions)
    {
      if (initializationAttributeDiscoverer == null)
      {
        throw new ArgumentNullException("initializationAttributeDiscoverer");
      }

      if (initializationAttributeExecutor == null)
      {
        throw new ArgumentNullException("initializationAttributeExecutor");
      }

      if (actions == null)
      {
        throw new ArgumentNullException("actions");
      }

      this.initializationAttributeDiscoverer = initializationAttributeDiscoverer;
      this.initializationAttributeExecutor = initializationAttributeExecutor;
      this.actions = actions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationManager"/> class.
    /// </summary>
    /// <param name="initializationAttributeDiscoverer">The initialization attribute discoverer.</param>
    /// <param name="initializationAttributeExecutor">The initialization attribute executor.</param>
    public InitializationManager(InitializationActionDiscoverer initializationAttributeDiscoverer, InitializationActionExecutor initializationAttributeExecutor) : this(initializationAttributeDiscoverer, initializationAttributeExecutor, new Dictionary<int, IList<InitializationAction>>())
    {
    }

    /// <summary>
    /// Performs initialization.
    /// </summary>
    /// <param name="testInstance">Test instance.</param>
    /// <param name="methodCallId">Method call id.</param>
    /// <param name="testMethod">Test method.</param>
    /// <param name="arguments">The arguments.</param>
    public virtual void Initialize(LiveTestWithInitialization testInstance, int methodCallId, MethodBase testMethod, object[] arguments)
    {
      IEnumerable<InitializationAction> actionsToExecute = this.initializationAttributeDiscoverer.GetInitializationActions(testInstance, testMethod, arguments);

      lock (this.actions)
      {
        if (this.actions.ContainsKey(methodCallId))
        {
          throw new InvalidOperationException("Concurrency problem occured. Initialize method has been called twice or more for the same method call id.");
        }

        this.actions.Add(methodCallId, Utility.ToList(actionsToExecute));
      }

      foreach (InitializationAction action in this.actions[methodCallId])
      {
        this.initializationAttributeExecutor.ExecuteInitializationForAction(action);
      }
    }

    /// <summary>
    /// Performs cleanup.
    /// </summary>
    /// <param name="testInstance">Test instance.</param>
    /// <param name="methodCallId">Method call id.</param>
    /// <param name="testMethod">Test method.</param>
    /// <param name="arguments">The arguments.</param>
    public virtual void Cleanup(LiveTestWithInitialization testInstance, int methodCallId, MethodBase testMethod, object[] arguments)
    {
      IEnumerable<InitializationAction> actionsInOriginalOrder;

      lock (this.actions)
      {
        if (!this.actions.ContainsKey(methodCallId))
        {
          throw new InvalidOperationException("Possible concurrency problem occured. Seems that Initialize method was not called before clean up.");
        }

        actionsInOriginalOrder = this.actions[methodCallId];
      }

      Stack<InitializationAction> actionsToExecute = new Stack<InitializationAction>(actionsInOriginalOrder);

      while (actionsToExecute.Count > 0)
      {
        InitializationAction action = actionsToExecute.Pop();
        this.initializationAttributeExecutor.ExecuteCleanupForAction(action);
      }
    }
  }
}
