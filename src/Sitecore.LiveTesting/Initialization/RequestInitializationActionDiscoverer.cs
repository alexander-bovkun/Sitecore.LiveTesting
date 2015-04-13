namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Globalization;
  using System.Collections.Generic;

  /// <summary>
  /// The initialization action discoverer that discovers actions for request context.
  /// </summary>
  public class RequestInitializationActionDiscoverer : InitializationActionDiscoverer
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

      RequestInitializationContext requestInitializationContext = context as RequestInitializationContext;

      if (requestInitializationContext == null)
      {
        throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Only contexts derived from '{0}' are supported.", typeof(RequestInitializationContext).FullName));
      }

      return this.GetInitializationActions(requestInitializationContext);
    }

    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The request initialization context.</param>
    /// <returns>List of discovered initialization actions.</returns>
    protected virtual IEnumerable<InitializationAction> GetInitializationActions(RequestInitializationContext context)
    {
      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      List<InitializationAction> result = new List<InitializationAction>();

      foreach (InitializationHandler initializationHandler in context.Request.InitializationHandlers)
      {
        result.Add(new InitializationAction(initializationHandler.Type.FullName) { State = initializationHandler, Context = context });
      }

      return result;
    }
  }
}
