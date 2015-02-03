namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System;
  using System.Xml;
  using System.Xml.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Pipelines;

  /// <summary>
  /// Defines the proxy for pipeline processors.
  /// </summary>
  public sealed class ProcessorProxy
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
    /// The processor definition.
    /// </summary>
    private readonly ProcessorDefinition processorDefinition;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorProxy" /> class.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <param name="processorIndex">Index of the processor.</param>
    /// <param name="processorDefinition">The processor definition.</param>
    public ProcessorProxy([NotNull] string pipelineName, [NotNull] string pipelineDomain, [NotNull] string processorIndex, [NotNull] string processorDefinition)
    {
      int parsedProcessorIndex;

      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineDomain, "pipelineDomain");
      Assert.ArgumentCondition(int.TryParse(processorIndex, out parsedProcessorIndex), "processorIndex", "Index of the processor must be an integer.");
      Assert.ArgumentNotNull(processorDefinition, "processorDefinition");

      this.pipelineName = pipelineName;
      this.pipelineDomain = pipelineDomain;
      this.processorIndex = parsedProcessorIndex;
      
      this.processorDefinition = processorDefinition == string.Empty ? new ProcessorDefinition() : new ProcessorDefinition(XElement.Parse(processorDefinition));
    }

    /// <summary>
    /// Occurs before pipeline is called.
    /// </summary>
    public static event EventHandler<RuntimeProcessorCallEventArgs> BeforePipelineProcessorCalled;

    /// <summary>
    /// Occurs after pipeline is called.
    /// </summary>
    public static event EventHandler<RuntimeProcessorCallEventArgs> AfterPipelineProcessorCalled;

    /// <summary>
    /// Processes this instance.
    /// </summary>
    /// <param name="pipelineArgs">The pipeline args.</param>
    public void Process(PipelineArgs pipelineArgs)
    {
      CoreProcessor processor = new CoreProcessor();
      bool isFakeProcessor = string.IsNullOrEmpty(this.processorDefinition.Type) && string.IsNullOrEmpty(this.processorDefinition.TypeReference);

      if (!isFakeProcessor)
      {
        XmlDocument medium = new XmlDocument();

        using (XmlReader reader = this.processorDefinition.ProcessorElement.CreateReader())
        {
          medium.Load(reader);
        }

        processor.Initialize(medium.DocumentElement);
      }

      EventHandler<RuntimeProcessorCallEventArgs> beforePipelineCalled = BeforePipelineProcessorCalled;
      if (beforePipelineCalled != null)
      {
        beforePipelineCalled(this, new RuntimeProcessorCallEventArgs(this.pipelineName, this.pipelineDomain, this.processorIndex, this.processorDefinition, pipelineArgs));
      }

      if (!isFakeProcessor)
      {
        processor.Invoke(new object[] { pipelineArgs });
      }

      EventHandler<RuntimeProcessorCallEventArgs> afterPipelineCalled = AfterPipelineProcessorCalled;
      if (afterPipelineCalled != null)
      {
        afterPipelineCalled(this, new RuntimeProcessorCallEventArgs(this.pipelineName, this.pipelineDomain, this.processorIndex, this.processorDefinition, pipelineArgs));
      }
    }
  }
}