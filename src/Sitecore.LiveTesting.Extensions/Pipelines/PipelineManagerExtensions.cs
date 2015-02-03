namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System.Globalization;
  using System.Linq;
  using System.Xml.Linq;
  using Sitecore.Diagnostics;
  using Sitecore.Reflection;

  /// <summary>
  /// Defines the extension methods for PipelineManager.
  /// </summary>
  public static class PipelineManagerExtensions
  {
    /// <summary>
    /// The processor element name.
    /// </summary>
    private const string ProcessorElementName = "processor";

    /// <summary>
    /// The parameter element name.
    /// </summary>
    private const string ParameterElementName = "param";

    /// <summary>
    /// The type attribute name.
    /// </summary>
    private const string TypeAttributeName = "type";

    /// <summary>
    /// The description attribute name.
    /// </summary>
    private const string DescriptionAttributeName = "desc";

    /// <summary>
    /// The pipeline name attribute value.
    /// </summary>
    private const string PipelineNameAttributeValue = "pipelineName";

    /// <summary>
    /// The pipeline domain attribute value.
    /// </summary>
    private const string PipelineDomainAttributeValue = "pipelineDomain";

    /// <summary>
    /// The processor index attribute value.
    /// </summary>
    private const string ProcessorIndexAttributeValue = "processorIndex";

    /// <summary>
    /// The processor definition attribute value.
    /// </summary>
    private const string ProcessorDefinitionAttributeValue = "processorDefinition";

    /// <summary>
    /// Starts the pipeline tracking.
    /// </summary>
    /// <param name="pipelineManager">The pipeline manager.</param>
    /// <param name="pipelineName">Name of the pipeline.</param>
    public static void StartPipelineTracking(this PipelineManager pipelineManager, string pipelineName)
    {
      pipelineManager.StartPipelineTracking(pipelineName, string.Empty);
    }

    /// <summary>    
    /// Starts the pipeline tracking.
    /// </summary>
    /// <param name="pipelineManager">The pipeline manager.</param>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    public static void StartPipelineTracking(this PipelineManager pipelineManager, string pipelineName, string pipelineDomain)
    {
      Assert.ArgumentNotNull(pipelineManager, "pipelineManager");

      PipelineDefinition pipelineDefinition = pipelineManager.GetPipelineDefinition(pipelineName, pipelineDomain);

      Assert.IsNotNull(pipelineDefinition, "Pipeline with name '{0}' was not found in domain '{1}'", pipelineName, pipelineDomain);

      for (int index = 0; index < pipelineDefinition.Processors.Count; ++index)
      {
        ProcessorDefinition processorDefinition = pipelineDefinition.Processors[index];

        Assert.IsNotNull(processorDefinition.Type, "Referenced processors are not supported.");

        if (ReflectionUtil.GetTypeInfo(processorDefinition.Type) != typeof(ProcessorProxy))
        {
          processorDefinition.ProcessorElement = new XElement(ProcessorElementName, new XAttribute(TypeAttributeName, ReflectionUtil.GetTypeString(typeof(ProcessorProxy))), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, PipelineNameAttributeValue), new XText(pipelineDefinition.Name)), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, PipelineDomainAttributeValue), new XText(pipelineDefinition.Domain)), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, ProcessorIndexAttributeValue), new XText(index.ToString(CultureInfo.InvariantCulture))), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, ProcessorDefinitionAttributeValue), new XCData(processorDefinition.ProcessorElement.ToString())));
        }
        else if (processorDefinition.ProcessorElement.Elements(ParameterElementName).Single(parameterElement => parameterElement.Attribute(DescriptionAttributeName).Value == ProcessorDefinitionAttributeValue).Value == string.Empty)
        {
          pipelineDefinition.Processors.RemoveAt(index--);
        }
      }

      pipelineDefinition.Processors.Add(new ProcessorDefinition(new XElement(ProcessorElementName, new XAttribute(TypeAttributeName, ReflectionUtil.GetTypeString(typeof(ProcessorProxy))), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, PipelineNameAttributeValue), new XText(pipelineDefinition.Name)), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, PipelineDomainAttributeValue), new XText(pipelineDefinition.Domain)), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, ProcessorIndexAttributeValue), new XText(pipelineDefinition.Processors.Count.ToString(CultureInfo.InvariantCulture))), new XElement(ParameterElementName, new XAttribute(DescriptionAttributeName, ProcessorDefinitionAttributeValue)))) { RunIfAborted = true });

      pipelineManager.UpdatePipelineDefinition(pipelineDefinition);
    }

    /// <summary>
    /// Stops the pipeline tracking.
    /// </summary>
    /// <param name="pipelineManager">The pipeline manager.</param>
    /// <param name="pipelineName">Name of the pipeline.</param>
    public static void StopPipelineTracking(this PipelineManager pipelineManager, string pipelineName)
    {
      StopPipelineTracking(pipelineManager, pipelineName, string.Empty);
    }

    /// <summary>
    /// Stops the pipeline tracking.
    /// </summary>
    /// <param name="pipelineManager">The pipeline manager.</param>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    public static void StopPipelineTracking(this PipelineManager pipelineManager, string pipelineName, string pipelineDomain)
    {
      Assert.ArgumentNotNull(pipelineManager, "pipelineManager");

      PipelineDefinition pipelineDefinition = pipelineManager.GetPipelineDefinition(pipelineName, pipelineDomain);

      Assert.IsNotNull(pipelineDefinition, "Pipeline with name '{0}' was not found in domain '{1}'", pipelineName, pipelineDomain);

      for (int index = 0; index < pipelineDefinition.Processors.Count; ++index)
      {
        ProcessorDefinition processorDefinition = pipelineDefinition.Processors[index];

        if ((processorDefinition.Type != null) && (ReflectionUtil.GetTypeInfo(processorDefinition.Type) == typeof(ProcessorProxy)))
        {
          string processorDefinitionRawValue = processorDefinition.ProcessorElement.Elements(ParameterElementName).Single(element => element.Attribute(DescriptionAttributeName).Value == ProcessorDefinitionAttributeValue).Value;

          if (processorDefinitionRawValue == string.Empty)
          {
            pipelineDefinition.Processors.RemoveAt(index--);
          }
          else
          {
            processorDefinition.ProcessorElement = XElement.Parse(processorDefinitionRawValue);
          }
        }
      }

      pipelineManager.UpdatePipelineDefinition(pipelineDefinition);
    }
  }
}
