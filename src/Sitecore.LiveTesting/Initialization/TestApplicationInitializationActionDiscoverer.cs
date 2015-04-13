namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Reflection;

  /// <summary>
  /// Defines the initialization action discoverer for application scoped actions.
  /// </summary>
  public class TestApplicationInitializationActionDiscoverer : InitializationActionDiscoverer
  {
    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The list of initialization actions.</returns>
    public sealed override IEnumerable<InitializationAction> GetInitializationActions(object context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      TestApplicationInitializationContext applicationContext = context as TestApplicationInitializationContext;

      if (applicationContext == null)
      {
        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Only contexts derived from '{0}' are supported.", typeof(TestApplicationInitializationContext).FullName));
      }

      return this.GetInitializationActions(applicationContext);
    }

    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <returns>The list of initialization actions.</returns>
    protected virtual IEnumerable<InitializationAction> GetInitializationActions(TestApplicationInitializationContext context)
    {
      List<InitializationHandlerAttribute> attributes = new List<InitializationHandlerAttribute>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        attributes.AddRange(Utility.GetAttributes<InitializationHandlerAttribute>(assembly));
      }

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
