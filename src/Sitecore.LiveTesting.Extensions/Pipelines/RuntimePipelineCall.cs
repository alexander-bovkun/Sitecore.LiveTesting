namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  /// <summary>
  /// Defines the runtime pipeline call information.
  /// </summary>
  public class RuntimePipelineCall
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
    /// The pipeline args.
    /// </summary>
    private readonly PipelineArgs pipelineArgs;

    /// <summary>
    /// The processor calls.
    /// </summary>
    private readonly IList<ProcessorDefinition> processorCalls;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimePipelineCall" /> class.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <param name="pipelineArgs">The pipeline args.</param>
    public RuntimePipelineCall(string pipelineName, string pipelineDomain, PipelineArgs pipelineArgs)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineDomain, "pipelineDomain");
      Assert.ArgumentNotNull(pipelineArgs, "pipelineArgs");

      this.pipelineName = pipelineName;
      this.pipelineDomain = pipelineDomain;
      this.pipelineArgs = pipelineArgs;
      this.processorCalls = new List<ProcessorDefinition>();
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
    /// Gets the pipeline args.
    /// </summary>
    /// <value>The pipeline args.</value>
    [NotNull]
    public PipelineArgs PipelineArgs 
    {
      get { return this.pipelineArgs; }
    }

    /// <summary>
    /// Gets the processor calls.
    /// </summary>
    /// <value>The processor calls.</value>
    [NotNull]
    public IList<ProcessorDefinition> ProcessorCalls
    {
      get { return this.processorCalls; }
    }
  }
}
