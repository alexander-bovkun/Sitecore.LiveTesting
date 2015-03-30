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
    /// The initialization action discoverer.
    /// </summary>
    private readonly InitializationActionDiscoverer initializationActionDiscoverer;

    /// <summary>
    /// The initialization action executor.
    /// </summary>
    private readonly InitializationActionExecutor initializationActionExecutor;

    /// <summary>
    /// The actions.
    /// </summary>
    private readonly IDictionary<int, IList<InitializationAction>> actions;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationManager"/> class.
    /// </summary>
    /// <param name="initializationActionDiscoverer">The initialization Action discoverer.</param>
    /// <param name="initializationActionExecutor">The initialization Action executor.</param>
    /// <param name="actions">The action container.</param>
    public InitializationManager(InitializationActionDiscoverer initializationActionDiscoverer, InitializationActionExecutor initializationActionExecutor, IDictionary<int, IList<InitializationAction>> actions)
    {
      if (initializationActionDiscoverer == null)
      {
        throw new ArgumentNullException("initializationActionDiscoverer");
      }

      if (initializationActionExecutor == null)
      {
        throw new ArgumentNullException("initializationActionExecutor");
      }

      if (actions == null)
      {
        throw new ArgumentNullException("actions");
      }

      this.initializationActionDiscoverer = initializationActionDiscoverer;
      this.initializationActionExecutor = initializationActionExecutor;
      this.actions = actions;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationManager"/> class.
    /// </summary>
    /// <param name="initializationActionDiscoverer">The initialization Action discoverer.</param>
    /// <param name="initializationActionExecutor">The initialization Action executor.</param>
    public InitializationManager(InitializationActionDiscoverer initializationActionDiscoverer, InitializationActionExecutor initializationActionExecutor) : this(initializationActionDiscoverer, initializationActionExecutor, new Dictionary<int, IList<InitializationAction>>())
    {
    }

    /// <summary>
    /// Performs initialization.
    /// </summary>
    /// <param name="id">The id of initialization attempt.</param>
    /// <param name="context">The initialization context.</param>
    public virtual void Initialize(int id, object context)
    {
      IEnumerable<InitializationAction> actionsToExecute = this.initializationActionDiscoverer.GetInitializationActions(context);

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
        this.initializationActionExecutor.ExecuteInitializationForAction(action);
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
        this.initializationActionExecutor.ExecuteCleanupForAction(action);
      }
    }
  }
}
