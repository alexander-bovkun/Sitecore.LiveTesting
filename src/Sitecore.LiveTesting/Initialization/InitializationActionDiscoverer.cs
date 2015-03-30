namespace Sitecore.LiveTesting.Initialization
{
  using System.Collections.Generic;

  /// <summary>
  /// Defines the base class for all initialization action discoverers.
  /// </summary>
  public abstract class InitializationActionDiscoverer
  {
    /// <summary>
    /// Gets initialization actions.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    /// <returns>List of discovered initialization actions.</returns>
    public abstract IEnumerable<InitializationAction> GetInitializationActions(object context);
  }
}
