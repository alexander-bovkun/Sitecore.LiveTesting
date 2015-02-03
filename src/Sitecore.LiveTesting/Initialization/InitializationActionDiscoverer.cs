namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;

  /// <summary>
  /// Defines the class that discovers initialization actions.
  /// </summary>
  public class InitializationActionDiscoverer
  {
    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="testInstance">The test instance.</param>
    /// <param name="testMethod">The test method.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>List of discovered initialization actions.</returns>
    public virtual IEnumerable<InitializationAction> GetInitializationActions(LiveTestWithInitialization testInstance, MethodBase testMethod, object[] arguments)
    {
      if (testInstance == null)
      {
        throw new ArgumentNullException("testInstance");
      }

      if (testMethod == null)
      {
        throw new ArgumentNullException("testMethod");
      }

      List<InitializationHandlerAttribute> attributes = Utility.ToList(GetActionAttributes(testInstance.GetType()));
      attributes.AddRange(GetActionAttributes(testMethod));
      attributes.Sort(InitializationHandlerAttributePriorityComparer.Default);

      List<InitializationAction> result = new List<InitializationAction>();
      foreach (InitializationHandlerAttribute initializationHandlerAttribute in attributes)
      {
        result.Add(ActionFromAttribute(initializationHandlerAttribute));
      }

      return result;
    }

    /// <summary>
    /// Gets actions associated with type member.
    /// </summary>
    /// <param name="memberInfo">The member info.</param>
    /// <returns>List of actions.</returns>
    private static IEnumerable<InitializationHandlerAttribute> GetActionAttributes(MemberInfo memberInfo)
    {
      List<InitializationHandlerAttribute> result = new List<InitializationHandlerAttribute>();

      foreach (object attributeCandidate in memberInfo.GetCustomAttributes(typeof(InitializationHandlerAttribute), true))
      {
        InitializationHandlerAttribute attribute = attributeCandidate as InitializationHandlerAttribute;

        if (attribute != null)
        {
          result.Add(attribute);
        }
      }

      return result;
    }

    /// <summary>
    /// Gets action from attribute.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <returns>The corresponding action.</returns>
    private static InitializationAction ActionFromAttribute(InitializationHandlerAttribute attribute)
    {
      return new InitializationAction(attribute.InitializationHandlerType.AssemblyQualifiedName) { State = attribute.InitializationHandlerType };
    }

    /// <summary>
    /// Defines the InitializationHandlerAttribute priority comparer.
    /// </summary>
    private class InitializationHandlerAttributePriorityComparer : IComparer<InitializationHandlerAttribute>
    {
      /// <summary>
      /// The default comparer.
      /// </summary>
      private static readonly InitializationHandlerAttributePriorityComparer DefaultComparer = new InitializationHandlerAttributePriorityComparer();

      /// <summary>
      /// Gets default instance of <see cref="InitializationHandlerAttributePriorityComparer"/>.
      /// </summary>
      internal static InitializationHandlerAttributePriorityComparer Default
      {
        get { return DefaultComparer; }
      }

      /// <summary>
      /// Compares two attributes according to the priority.
      /// </summary>
      /// <param name="x">The first attribute.</param>
      /// <param name="y">The second attribute.</param>
      /// <returns><value>1</value> if x goes after y, <value>-1</value> if x goes before y, otherwise <value>0</value>.</returns>
      public int Compare(InitializationHandlerAttribute x, InitializationHandlerAttribute y)
      {
        if (x.Priority > y.Priority)
        {
          return 1;
        }

        if (x.Priority < y.Priority)
        {
          return -1;
        }

        return 0;
      }
    }
  }
}
