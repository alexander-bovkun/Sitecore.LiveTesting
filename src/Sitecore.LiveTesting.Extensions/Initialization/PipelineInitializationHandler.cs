namespace Sitecore.LiveTesting.Extensions.Initialization
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using Sitecore.LiveTesting.Extensions.Configuration;
  using Sitecore.LiveTesting.Extensions.Pipelines;

  /// <summary>
  /// Defines the pipeline initialization handler.
  /// </summary>
  public class PipelineInitializationHandler : IDisposable
  {
    /// <summary>
    /// The configuration switcher.
    /// </summary>
    private static SitecoreConfigurationSwitcher configurationSwitcher;

    /// <summary>
    /// The pipeline manager.
    /// </summary>
    private static PipelineManager pipelineManager;

    /// <summary>
    /// The pipeline tracker.
    /// </summary>
    private static PipelineTracker pipelineTracker;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineInitializationHandler"/> class.
    /// </summary>
    public PipelineInitializationHandler()
    {
      configurationSwitcher = new SitecoreConfigurationSwitcher();
      pipelineManager = new PipelineManager(configurationSwitcher);
      pipelineTracker = new PipelineTracker(pipelineManager);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="PipelineInitializationHandler"/> class.
    /// </summary>
    ~PipelineInitializationHandler()
    {
      this.Dispose(false);
    }

    /// <summary>
    /// Gets the Sitecore configuration switcher.
    /// </summary>
    /// <value>The Sitecore configuration switcher.</value>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
    public static SitecoreConfigurationSwitcher SitecoreConfigurationSwitcher
    {
      get { return configurationSwitcher; }
    }

    /// <summary>
    /// Gets the pipeline manager.
    /// </summary>
    /// <value>The pipeline manager.</value>
    public static PipelineManager PipelineManager
    {
      get { return pipelineManager; }
    }

    /// <summary>
    /// Gets the pipeline tracker.
    /// </summary>
    /// <value>The pipeline tracker.</value>
    public static PipelineTracker PipelineTracker
    {
      get { return pipelineTracker; }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (disposing)
      {
        if (pipelineTracker != null)
        {
          pipelineTracker.Dispose();
          pipelineTracker = null;
        }

        if (configurationSwitcher != null)
        {
          configurationSwitcher.Dispose();
          configurationSwitcher = null;
        }
      }

      this.disposed = true;
    }
  }
}
