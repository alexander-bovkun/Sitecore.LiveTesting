namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Defines the class responsible for initialization before and after each test runs.
  /// </summary>
  public class InitializationManager
  {
    /// <summary>
    /// The initialization attribute discoverer.
    /// </summary>
    private readonly TestInitializationActionDiscoverer initializationAttributeDiscoverer;

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
    public InitializationManager(TestInitializationActionDiscoverer initializationAttributeDiscoverer, InitializationActionExecutor initializationAttributeExecutor, IDictionary<int, IList<InitializationAction>> actions)
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
    public InitializationManager(TestInitializationActionDiscoverer initializationAttributeDiscoverer, InitializationActionExecutor initializationAttributeExecutor) : this(initializationAttributeDiscoverer, initializationAttributeExecutor, new Dictionary<int, IList<InitializationAction>>())
    {
    }

    /// <summary>
    /// Performs initialization.
    /// </summary>
    /// <param name="id">The id of initialization attempt.</param>
    /// <param name="context">The initialization context.</param>
    public virtual void Initialize(int id, object context)
    {
      IEnumerable<InitializationAction> actionsToExecute = this.initializationAttributeDiscoverer.GetInitializationActions(context);

      lock (this.actions)
      {
        if (this.actions.ContainsKey(id))
        {
          throw new InvalidOperationException("Concurrency problem occured. Initialize method has been called twice or more for the same method call id.");
        }

        this.actions.Add(id, Utility.ToList(actionsToExecute));
      }

      foreach (InitializationAction action in this.actions[id])
      {
        this.initializationAttributeExecutor.ExecuteInitializationForAction(action);
      }
    }

    /// <summary>
    /// Performs cleanup.
    /// </summary>
    /// <param name="id">The id of initialization attempt.</param>
    /// <param name="context">The initialization context.</param>
    public virtual void Cleanup(int id, object context)
    {
      IEnumerable<InitializationAction> actionsInOriginalOrder;

      lock (this.actions)
      {
        if (!this.actions.ContainsKey(id))
        {
          throw new InvalidOperationException("Possible concurrency problem occured. Seems that Initialize method was not called before clean up.");
        }

        actionsInOriginalOrder = this.actions[id];
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
