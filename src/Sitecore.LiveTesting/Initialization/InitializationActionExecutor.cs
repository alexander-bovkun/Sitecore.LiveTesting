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

      InitializationHandler initializationHandler = action.State as InitializationHandler;

      if (initializationHandler == null)
      {
        throw new ArgumentException(string.Format("Action in not in a proper state. It's 'State' property should be of type '{0}' and should not be null.", typeof(InitializationHandler)));
      }

      Type[] argumentTypes = new Type[initializationHandler.Arguments.Length];

      for (int index = 0; index < initializationHandler.Arguments.Length; ++index)
      {
        argumentTypes[index] = initializationHandler.Arguments[index].GetType();
      }

      ConstructorInfo constructor = initializationHandler.Type.GetConstructor(argumentTypes);

      if (constructor == null)
      {
        constructor = initializationHandler.Type.GetConstructor(new[] { typeof(object[]) });

        if (constructor == null)
        {
          throw new InvalidOperationException(string.Format("Failed to create an instance of '{0}' type. No constructor found that matches the list of parameters.", initializationHandler.Type.AssemblyQualifiedName));
        }
        
        action.State = constructor.Invoke(new object[] { initializationHandler.Arguments });
      }
      else
      {
        action.State = constructor.Invoke(initializationHandler.Arguments);
      }

      IInitializationContextAware contextAwareInitializer = action.State as IInitializationContextAware;

      if (contextAwareInitializer != null)
      {
        contextAwareInitializer.SetInitializationContext(action.Context);
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
