namespace Sitecore.LiveTesting.Initialization
{
  using System.Collections.Generic;

  /// <summary>
  /// Defines the InitializationHandlerAttribute priority comparer.
  /// </summary>
  internal class InitializationHandlerAttributePriorityComparer : IComparer<InitializationHandlerAttribute>
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
