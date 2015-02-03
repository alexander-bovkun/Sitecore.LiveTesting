namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  /// <summary>
  /// Defines the runtime processor call event args.
  /// </summary>
  public class RuntimeProcessorCallEventArgs : EventArgs
  {
    /// <summary>
    /// The pipeline name.
    /// </summary>
    private readonly string pipelineName;

    /// <summary>
    /// The pipeline domain.
    /// </summary>
    private readonly string pipelineDomain;

    /// <summary>
    /// The processor index.
    /// </summary>
    private readonly int processorIndex;

    /// <summary>
    /// The pipeline definition.
    /// </summary>
    private readonly ProcessorDefinition processorDefinition;

    /// <summary>
    /// The pipeline args.
    /// </summary>
    private readonly PipelineArgs pipelineArgs;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeProcessorCallEventArgs" /> class.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <param name="processorIndex">Index of the processor.</param>
    /// <param name="processorDefinition">The processor definition.</param>
    /// <param name="pipelineArgs">The pipeline args.</param>
    public RuntimeProcessorCallEventArgs([NotNull] string pipelineName, [NotNull] string pipelineDomain, int processorIndex, [NotNull] ProcessorDefinition processorDefinition, [NotNull] PipelineArgs pipelineArgs)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineDomain, "pipelineDomain");
      Assert.ArgumentNotNull(processorDefinition, "processorDefinition");
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      this.pipelineName = pipelineName;
      this.pipelineDomain = pipelineDomain;
      this.processorIndex = processorIndex;
      this.processorDefinition = processorDefinition;
      this.pipelineArgs = pipelineArgs;
    }

    /// <summary>
    /// Gets the name of the pipeline.
    /// </summary>
    /// <value>The name of the pipeline.</value>
    [NotNull]
    public string PipelineName
    {
      get { return this.pipelineName; }
    }

    /// <summary>
    /// Gets the pipeline domain.
    /// </summary>
    /// <value>The pipeline domain.</value>
    [NotNull]
    public string PipelineDomain
    {
      get { return this.pipelineDomain; }
    }

    /// <summary>
    /// Gets the index of the processor.
    /// </summary>
    /// <value>The index of the processor.</value>
    public int ProcessorIndex 
    {
      get { return this.processorIndex; }
    }

    /// <summary>
    /// Gets the pipeline definition.
    /// </summary>
    /// <value>The pipeline definition.</value>
    [NotNull]
    public ProcessorDefinition ProcessorDefinition
    {
      get { return this.processorDefinition; }
    }

    /// <summary>
    /// Gets the pipeline args.
    /// </summary>
    /// <value>The pipeline args.</value>
    [NotNull]
    public PipelineArgs PipelineArgs
    {
      get { return this.pipelineArgs; }
    }
  }
}
