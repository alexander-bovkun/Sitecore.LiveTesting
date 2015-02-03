namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Reflection;

  /// <summary>
  /// Defines the class that executes initialization actions.
  /// </summary>
  public class InitializationActionExecutor
  {
    /// <summary>
    /// Executes initialization for the action.
    /// </summary>
    /// <param name="action">The action.</param>
    public virtual void ExecuteInitializationForAction(InitializationAction action)
    {
      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      Type type = action.State as Type;

      if (type == null)
      {
        throw new ArgumentException("Action in not in a proper state. It's 'State' property should have reference to the actual initializer type.");
      }

      ConstructorInfo constructor = type.GetConstructor(new Type[0]);

      if (constructor != null)
      {
        action.State = constructor.Invoke(new object[0]);
      }
    }

    /// <summary>
    /// Executes cleanup operation for the action.
    /// </summary>
    /// <param name="action">The action.</param>
    public virtual void ExecuteCleanupForAction(InitializationAction action)
    {
      if (action == null)
      {
        throw new ArgumentNullException("action");
      }

      IDisposable disposableCandidate = action.State as IDisposable;

      if (disposableCandidate != null)
      {
        disposableCandidate.Dispose();
      }
    }
  }
}
