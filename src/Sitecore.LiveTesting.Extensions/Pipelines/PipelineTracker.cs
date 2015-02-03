namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class that tracks pipeline calls.
  /// </summary>
  public class PipelineTracker : IDisposable
  {
    /// <summary>
    /// The pipeline manager.
    /// </summary>
    private readonly PipelineManager pipelineManager;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// The tracked pipelines.
    /// </summary>
    private ICollection<Tuple<string, string>> trackedPipelines;

    /// <summary>
    /// The pipeline calls.
    /// </summary>
    private IList<RuntimePipelineCall> pipelineCalls;

    /// <summary>
    /// The executing pipelines.
    /// </summary>
    private Stack<RuntimePipelineCall> executingPipelines;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineTracker" /> class.
    /// </summary>
    /// <param name="pipelineManager">The pipeline manager.</param>
    public PipelineTracker([NotNull] PipelineManager pipelineManager)
    {
      Assert.ArgumentNotNull(pipelineManager, "pipelineManager");

      this.pipelineManager = pipelineManager;
    }

    /// <summary>
    /// Gets the pipeline calls.
    /// </summary>
    /// <value>The pipeline calls.</value>
    [NotNull]
    public virtual IList<RuntimePipelineCall> PipelineCalls
    {
      get
      {
        return this.pipelineCalls ?? new List<RuntimePipelineCall>();
      }
    }

    /// <summary>
    /// Gets the pipeline manager.
    /// </summary>
    /// <value>The pipeline manager.</value>
    [NotNull]
    protected PipelineManager PipelineManager
    {
      get { return this.pipelineManager; }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(!this.disposed);
      this.disposed = true;
    }

    /// <summary>
    /// Resets this instance.
    /// </summary>
    public virtual void Reset()
    {
      if (this.pipelineCalls != null)
      {
        this.pipelineCalls.Clear();
      }

      if (this.executingPipelines != null)
      {
        this.executingPipelines.Clear();
      }
    }

    /// <summary>
    /// Tracks the pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <returns>Current instance of <see cref="PipelineTracker"/></returns>
    public PipelineTracker TrackPipeline([NotNull] string pipelineName)
    {
      return this.TrackPipeline(pipelineName, string.Empty);
    }

    /// <summary>
    /// Tracks the pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <returns>Current instance of <see cref="PipelineTracker"/></returns>
    public PipelineTracker TrackPipeline([NotNull] string pipelineName, [NotNull] string pipelineDomain)
    {
      Assert.IsFalse(this.disposed, "Instance has been already disposed.");
      this.StartPipelineTracking(pipelineName, pipelineDomain);

      return this;
    }

    /// <summary>
    /// Does the not track pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <returns>Current instance of <see cref="PipelineTracker"/></returns>
    public PipelineTracker DoNotTrackPipeline([NotNull] string pipelineName)
    {
      return this.DoNotTrackPipeline(pipelineName, string.Empty);
    }

    /// <summary>
    /// Does the not track pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <returns>Current instance of <see cref="PipelineTracker"/></returns>
    public PipelineTracker DoNotTrackPipeline([NotNull] string pipelineName, [NotNull] string pipelineDomain)
    {
      Assert.IsFalse(this.disposed, "Instance has been already disposed.");
      this.StopPipelineTracking(pipelineName, pipelineDomain);

      return this;
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.trackedPipelines != null)
        {
          ProcessorProxy.AfterPipelineProcessorCalled -= this.OnAfterPipelineProcessorCalled;

          foreach (Tuple<string, string> pipelineHeader in this.trackedPipelines.ToArray())
          {
            this.DoNotTrackPipeline(pipelineHeader.Item1, pipelineHeader.Item2);
          }
        }
      }
    }

    /// <summary>
    /// Starts the pipeline tracking.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    protected virtual void StartPipelineTracking(string pipelineName, string pipelineDomain)
    {
      this.PipelineManager.StartPipelineTracking(pipelineName, pipelineDomain);

      if (this.trackedPipelines == null)
      {
        this.trackedPipelines = new HashSet<Tuple<string, string>>();
        ProcessorProxy.AfterPipelineProcessorCalled += this.OnAfterPipelineProcessorCalled;
      }

      this.trackedPipelines.Add(new Tuple<string, string>(pipelineName, pipelineDomain));
    }

    /// <summary>
    /// Stops the pipeline tracking.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    protected virtual void StopPipelineTracking(string pipelineName, string pipelineDomain)
    {
      this.PipelineManager.StopPipelineTracking(pipelineName, pipelineDomain);

      if (this.trackedPipelines != null)
      {
        this.trackedPipelines.Remove(new Tuple<string, string>(pipelineName, pipelineDomain));
      }
    }

    /// <summary>
    /// Called after processor is called.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The <see cref="RuntimeProcessorCallEventArgs" /> instance containing the event data.</param>
    protected virtual void OnAfterPipelineProcessorCalled(object sender, RuntimeProcessorCallEventArgs args)
    {
      lock (this)
      {
        if (this.pipelineCalls == null)
        {
          this.pipelineCalls = new List<RuntimePipelineCall>();
        }

        if (this.executingPipelines == null)
        {
          this.executingPipelines = new Stack<RuntimePipelineCall>();
        }

        if (args.ProcessorIndex == 0)
        {
          RuntimePipelineCall pipelineCall = new RuntimePipelineCall(args.PipelineName, args.PipelineDomain, args.PipelineArgs);

          this.pipelineCalls.Add(pipelineCall);
          this.executingPipelines.Push(pipelineCall);
        }

        if (string.IsNullOrEmpty(args.ProcessorDefinition.Type) && string.IsNullOrEmpty(args.ProcessorDefinition.TypeReference))
        {
          this.executingPipelines.Pop();
        }
        else
        {
          this.executingPipelines.Peek().ProcessorCalls.Add(args.ProcessorDefinition);
        }
      }
    }
  }
}
