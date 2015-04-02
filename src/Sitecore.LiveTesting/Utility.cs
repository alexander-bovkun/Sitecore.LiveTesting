namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines all-purpose utility class.
  /// </summary>
  internal static class Utility
  {
    /// <summary>
    /// Gets the factory method to instantiate live test.
    /// </summary>
    /// <param name="type">Type of the test.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="argumentTypes">Argument types.</param>
    /// <returns>Reference to factory method.</returns>
    internal static MethodInfo GetInheritedMethod(Type type, string methodName, Type[] argumentTypes)
    {
      if (type == null)
      {
        throw new ArgumentNullException("type");
      }

      MethodInfo result = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, argumentTypes, new ParameterModifier[0]);

      while ((result == null) && (type != null))
      {
        result = type.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, argumentTypes, new ParameterModifier[0]);
        type = type.BaseType;
      }

      return result;
    }

    /// <summary>
    /// Gets initialization handler attributes associated with type member.
    /// </summary>
    /// <typeparam name="T">Type of the attribute.</typeparam>
    /// <param name="memberInfo">The member info.</param>
    /// <returns>List of initialization handler attributes.</returns>
    internal static List<T> GetAttributes<T>(MemberInfo memberInfo) where T : class
    {
      return ToInitializationHandlerAttributeList<T>(memberInfo.GetCustomAttributes(typeof(T), true));
    }

    /// <summary>
    /// Gets initialization handler attributes associated with assembly.
    /// </summary>
    /// <typeparam name="T">Type of the attribute.</typeparam>
    /// <param name="assembly">The assembly.</param>
    /// <returns>List of initialization handler attributes.</returns>
    internal static List<T> GetAttributes<T>(Assembly assembly) where T : class
    {
      return ToInitializationHandlerAttributeList<T>(assembly.GetCustomAttributes(typeof(T), false));
    }

    /// <summary>
    /// Converts initialization handler attribute into corresponding initialization action.
    /// </summary>
    /// <param name="attribute">The attribute.</param>
    /// <param name="context">The initialization context.</param>
    /// <returns>The initialization action.</returns>
    internal static InitializationAction InitializationActionFromInitializationHandlerAttribute(InitializationHandlerAttribute attribute, object context)
    {
      return new InitializationAction(attribute.InitializationHandler.Type.AssemblyQualifiedName) { State = attribute.InitializationHandler, Context = context };
    }

    /// <summary>
    /// Converts sequence of actions into list.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="actions">The sequence of actions.</param>
    /// <returns>The list of actions.</returns>
    internal static List<T> ToList<T>(IEnumerable<T> actions)
    {
      List<T> resultCandidate = actions as List<T>;

      if (resultCandidate != null)
      {
        return resultCandidate;
      }

      return new List<T>(actions);
    }

    /// <summary>
    /// Converts attributes array to initialization handler attribute list.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    /// <param name="attributes">The attributes.</param>
    /// <returns>The initialization handler attribute list.</returns>
    private static List<T> ToInitializationHandlerAttributeList<T>(IEnumerable<object> attributes) where T : class
    {
      List<T> result = new List<T>();

      foreach (object attributeCandidate in attributes)
      {
        T attribute = attributeCandidate as T;

        if (attribute != null)
        {
          result.Add(attribute);
        }
      }

      return result;
    }
  }
}
