namespace Sitecore.LiveTesting.Initialization
{
  /// <summary>
  /// Defines the interface used to set initialization context for initializers.
  /// </summary>
  public interface IInitializationContextAware
  {
    /// <summary>
    /// Sets initialization context.
    /// </summary>
    /// <param name="context">The initialization context.</param>
    void SetInitializationContext(InitializationContext context);
  }
}
