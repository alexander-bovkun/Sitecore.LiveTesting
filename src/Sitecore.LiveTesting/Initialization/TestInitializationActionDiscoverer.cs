namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;

  /// <summary>
  /// Defines the class that discovers initialization actions.
  /// </summary>
  public class TestInitializationActionDiscoverer : InitializationActionDiscoverer
  {
    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    /// <returns>List of discovered initialization actions.</returns>
    public sealed override IEnumerable<InitializationAction> GetInitializationActions(object context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      TestInitializationContext initializationContext = context as TestInitializationContext;

      if (initializationContext == null)
      {
        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Only contexts derived from '{0}' are supported.", typeof(TestInitializationContext).AssemblyQualifiedName));
      }

      return this.GetInitializationActions(initializationContext);
    }

    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The test initialization context.</param>
    /// <returns>List of discovered initialization actions.</returns>
    protected virtual IEnumerable<InitializationAction> GetInitializationActions(TestInitializationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      List<InitializationHandlerAttribute> attributes = Utility.GetAttributes<InitializationHandlerAttribute>(context.Instance.GetType());
      attributes.AddRange(Utility.GetAttributes<InitializationHandlerAttribute>(context.Method));
      attributes.Sort(InitializationHandlerAttributePriorityComparer.Default);

      List<InitializationAction> result = new List<InitializationAction>();
      foreach (InitializationHandlerAttribute initializationHandlerAttribute in attributes)
      {
        result.Add(Utility.InitializationActionFromInitializationHandlerAttribute(initializationHandlerAttribute, context));
      }

      return result;
    }
  }
}
