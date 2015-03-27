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

      object[] initializerInfo = action.State as object[];

      if ((initializerInfo == null) || (initializerInfo.Length < 2))
      {
        throw new ArgumentException("Action in not in a proper state. It's 'State' property should have reference to an array with at least 2 elements: initializer type and array of its arguments.");
      }

      Type initializerType = initializerInfo[0] as Type;

      if (initializerType == null)
      {
        throw new ArgumentException("Action in not in a proper state. First element of array must be a type of initializer.");
      }

      object[] initializerArguments = initializerInfo[1] as object[];

      if (initializerArguments == null)
      {
        throw new ArgumentException("Action in not in a proper state. Second element of array must be an System.Object[] array of initializer arguments.");
      }

      Type[] argumentTypes = new Type[initializerArguments.Length];

      for (int index = 0; index < initializerArguments.Length; ++index)
      {
        argumentTypes[index] = initializerArguments[index].GetType();
      }

      ConstructorInfo constructor = initializerType.GetConstructor(argumentTypes);

      if (constructor != null)
      {
        action.State = constructor.Invoke(initializerArguments);
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
