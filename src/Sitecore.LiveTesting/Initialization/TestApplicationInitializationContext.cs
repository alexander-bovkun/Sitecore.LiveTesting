namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using Sitecore.LiveTesting.Applications;

  /// <summary>
  /// Defines the initialization context that provides basic information about the application being instantiated.
  /// </summary>
  public class TestApplicationInitializationContext
  {
    /// <summary>
    /// The application.
    /// </summary>
    private readonly TestApplication application;

    /// <summary>
    /// Initializes a new instance of the <see cref="TestApplicationInitializationContext"/> class.
    /// </summary>
    /// <param name="application">The application.</param>
    public TestApplicationInitializationContext(TestApplication application)
    {
      if (application == null)
      {
        throw new ArgumentNullException("application");
      }

      this.application = application;
    }

    /// <summary>
    /// Gets the application.
    /// </summary>
    public TestApplication Application
    {
      get { return this.application; }
    }
  }
}
