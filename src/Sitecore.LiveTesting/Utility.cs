namespace Sitecore.LiveTesting
{
  using System;
  using System.Collections.Generic;
  using System.Reflection;

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
  }
}
