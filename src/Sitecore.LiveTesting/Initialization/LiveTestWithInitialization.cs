namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the base class for attribute-aware tests.
  /// </summary>
  [Obsolete("Use 'Sitecore.LiveTesting.LiveTest, Sitecore.LiveTesting' instead.")]
  public class LiveTestWithInitialization : LiveTest
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitialization"/> class.
    /// </summary>
    public LiveTestWithInitialization() : this(HostingEnvironment.IsHosted ? new InitializationManager(new TestInitializationActionDiscoverer(), new InitializationActionExecutor()) : null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestWithInitialization"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization manager.</param>
    protected LiveTestWithInitialization(InitializationManager initializationManager) : base(initializationManager)
    {
    }
  }
}
