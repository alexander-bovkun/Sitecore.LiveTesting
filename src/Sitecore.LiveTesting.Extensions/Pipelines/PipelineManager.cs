namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System;
  using System.Collections.Generic;
  using System.Xml;
  using System.Xml.Linq;
  using System.Xml.XPath;
  using Sitecore.Configuration;
  using Sitecore.Diagnostics;
  using Sitecore.LiveTesting.Extensions.Configuration;
  using Sitecore.Pipelines;

  /// <summary>
  /// Defines the class that allows to get information and change pipelines.
  /// </summary>
  public class PipelineManager
  {
    /// <summary>
    /// Defines "There are multiple pipeline definitions with the name {0}." phrase.
    /// </summary>
    private const string ThereAreMultiplePipelineDefinitionsWithName0 = "There are multiple pipeline definitions with the name {0}.";

    /// <summary>
    /// Defines "The requested pipeline with the name {0} does not exist. Pipeline creation is not supported." phrase.
    /// </summary>
    private const string RequestedPipelineWithName0DoesNotExistPipelineCreationIsNotSupported = "The requested pipeline with name the {0} does not exist. Pipeline creation is not supported.";

    /// <summary>
    /// The pipelines xpath.
    /// </summary>
    private const string PipelinesXPath = "/sitecore/pipelines/*|/sitecore/pipelines/group/pipelines/*";

    /// <summary>
    /// The pipeline domain name xpath.
    /// </summary>
    private const string PipelineDomainNameXPath = "../../@groupName";

    /// <summary>
    /// The pipeline xpath template.
    /// </summary>
    private const string PipelineXPathTemplate = "/sitecore/pipelines/{0}";

    /// <summary>
    /// The pipeline group xpath template.
    /// </summary>
    private const string PipelineGroupXPathTemplate = "group[@groupName='{0}']/pipelines/{1}";

    /// <summary>
    /// The processor element name.
    /// </summary>
    private const string ProcessorElementName = "processor";

    /// <summary>
    /// The configuration switcher.
    /// </summary>
    private readonly SitecoreConfigurationSwitcher configurationSwitcher;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineManager" /> class.
    /// </summary>
    /// <param name="configurationSwitcher">The configuration switcher.</param>
    public PipelineManager(SitecoreConfigurationSwitcher configurationSwitcher)
    {
      Assert.ArgumentNotNull(configurationSwitcher, "configurationSwitcher");

      this.configurationSwitcher = configurationSwitcher;
    }

    /// <summary>
    /// Gets the pipeline names.
    /// </summary>
    /// <returns>The sequence of pipeline and corresponding domain names.</returns>
    [NotNull]
    public IEnumerable<Tuple<string, string>> GetPipelineNames()
    {
      XPathNodeIterator pipelineIterator = this.configurationSwitcher.GetNodeIterator(PipelinesXPath);

      while (pipelineIterator.MoveNext())
      {
        string domainName = pipelineIterator.Current.Evaluate(PipelineDomainNameXPath) as string;

        yield return new Tuple<string, string>(pipelineIterator.Current.Name, domainName ?? string.Empty);
      }
    }

    /// <summary>
    /// Gets the pipeline definition.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <returns>The pipeline definition.</returns>
    [CanBeNull]
    public PipelineDefinition GetPipelineDefinition([NotNull] string pipelineName)
    {
      return this.GetPipelineDefinition(pipelineName, string.Empty);
    }

    /// <summary>
    /// Gets the pipeline definition.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <returns>The pipeline definition.</returns>
    [CanBeNull]
    public PipelineDefinition GetPipelineDefinition([NotNull] string pipelineName, [NotNull] string pipelineDomain)
    {
      Assert.ArgumentNotNullOrEmpty(pipelineName, "pipelineName");
      Assert.ArgumentNotNull(pipelineDomain, "pipelineDomain");

      XPathNavigator pipelineNavigator = this.GetPipelineNavigator(pipelineName, pipelineDomain);

      if (pipelineNavigator == null)
      {
        return null;
      }

      XElement pipelineElement;

      using (XmlReader reader = pipelineNavigator.ReadSubtree())
      {
        pipelineElement = XElement.Load(reader);
      }

      return this.MapElementToPipeline(pipelineName, pipelineDomain, pipelineElement);
    }

    /// <summary>
    /// Updates the pipeline definition.
    /// </summary>
    /// <param name="pipelineDefinition">The pipeline definition.</param>
    public void UpdatePipelineDefinition([NotNull] PipelineDefinition pipelineDefinition)
    {
      this.UpdatePipelineDefinition(pipelineDefinition, true);
    }

    /// <summary>
    /// Updates the pipeline definition.
    /// </summary>
    /// <param name="pipelineDefinition">The pipeline definition.</param>
    /// <param name="clearCache">if set to <c>true</c> the cache will be cleared.</param>
    public void UpdatePipelineDefinition([NotNull] PipelineDefinition pipelineDefinition, bool clearCache)
    {
      Assert.ArgumentNotNull(pipelineDefinition, "pipelineDefinition");

      XPathNavigator pipelineNavigator = this.GetPipelineNavigator(pipelineDefinition.Name, pipelineDefinition.Domain);
      Assert.IsNotNull(pipelineNavigator, RequestedPipelineWithName0DoesNotExistPipelineCreationIsNotSupported);

      XElement pipelineElement;

      using (XmlReader reader = pipelineNavigator.ReadSubtree())
      {
        pipelineElement = XElement.Load(reader);
      }

      this.UpdatePipelineElement(pipelineElement, pipelineDefinition);

      using (XmlReader reader = pipelineElement.CreateReader())
      {
        pipelineNavigator.ReplaceSelf(reader);
      }

      if (clearCache)
      {
        this.ClearCache();
      }
    }

    /// <summary>
    /// Maps the element to pipeline.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <param name="pipelineElement">The pipeline element.</param>
    /// <returns>PipelineDefinition instance created from pipeline element.</returns>
    protected virtual PipelineDefinition MapElementToPipeline(string pipelineName, string pipelineDomain, XElement pipelineElement)
    {
      PipelineDefinition pipelineDefinition = new PipelineDefinition(pipelineName, pipelineDomain);

      foreach (XElement processorElement in pipelineElement.Elements(ProcessorElementName))
      {
        pipelineDefinition.Processors.Add(this.MapElementToProcessor(processorElement));
      }

      return pipelineDefinition;
    }

    /// <summary>
    /// Maps the element to processor.
    /// </summary>
    /// <param name="processorElement">The processor element.</param>
    /// <returns>ProcessorDefinition instance created from processor element.</returns>
    protected virtual ProcessorDefinition MapElementToProcessor(XElement processorElement)
    {
      return new ProcessorDefinition(processorElement);
    }

    /// <summary>
    /// Maps the processor to element.
    /// </summary>
    /// <param name="processorDefinition">The processor definition.</param>
    /// <returns>Xml element.</returns>
    protected virtual XElement MapProcessorToElement(ProcessorDefinition processorDefinition)
    {
      return processorDefinition.ProcessorElement;
    }

    /// <summary>
    /// Updates the pipeline element.
    /// </summary>
    /// <param name="pipelineElement">The pipeline element.</param>
    /// <param name="pipelineDefinition">The pipeline definition.</param>
    protected virtual void UpdatePipelineElement(XElement pipelineElement, PipelineDefinition pipelineDefinition)
    {
      IEnumerator<XElement> elementEnumerator = pipelineElement.Elements(ProcessorElementName).GetEnumerator();
      IEnumerator<ProcessorDefinition> processorEnumerator = pipelineDefinition.Processors.GetEnumerator();

      IDictionary<XElement, object> elementsToReplace = new Dictionary<XElement, object>();
      ICollection<XElement> elementsToRemove = new LinkedList<XElement>();
      ICollection<ProcessorDefinition> processorsToAdd = new LinkedList<ProcessorDefinition>();
      
      bool elementFlag, processorFlag;

      do
      {
        elementFlag = elementEnumerator.MoveNext();
        processorFlag = processorEnumerator.MoveNext();

        if (elementFlag && processorFlag)
        {
          elementsToReplace.Add(elementEnumerator.Current, processorEnumerator.Current.ProcessorElement);
        }
        else if (elementFlag)
        {
          elementsToRemove.Add(elementEnumerator.Current);
        }
        else if (processorFlag)
        {
          processorsToAdd.Add(processorEnumerator.Current);
        }
      }
      while (elementFlag || processorFlag);

      foreach (KeyValuePair<XElement, object> pair in elementsToReplace)
      {
        pair.Key.ReplaceWith(pair.Value);
      }

      foreach (XElement element in elementsToRemove)
      {
        element.Remove();
      }

      foreach (ProcessorDefinition processorDefinition in processorsToAdd)
      {
        pipelineElement.Add(this.MapProcessorToElement(processorDefinition));
      }
    }

    /// <summary>
    /// Gets the pipeline xpath.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <returns>The xpath that corresponds to the pipeline.</returns>
    private string GetPipelineXPath([NotNull] string pipelineName, [NotNull] string pipelineDomain)
    {
      if (pipelineDomain == string.Empty)
      {
        return string.Format(PipelineXPathTemplate, pipelineName);
      }

      return string.Format(PipelineXPathTemplate, string.Format(PipelineGroupXPathTemplate, pipelineDomain, pipelineName));
    }

    /// <summary>
    /// Gets the pipeline navigator.
    /// </summary>
    /// <param name="pipelineName">Name of the pipeline.</param>
    /// <param name="pipelineDomain">The pipeline domain.</param>
    /// <returns>The pipeline navigator.</returns>
    private XPathNavigator GetPipelineNavigator(string pipelineName, string pipelineDomain)
    {
      XPathNodeIterator pipelineIterator = this.configurationSwitcher.GetNodeIterator(this.GetPipelineXPath(pipelineName, pipelineDomain));
      XPathNavigator result = null;

      while (pipelineIterator.MoveNext())
      {
        Assert.IsNull(result, string.Format(ThereAreMultiplePipelineDefinitionsWithName0, pipelineName));
        result = pipelineIterator.Current;
      }

      return result;
    }

    /// <summary>
    /// Clears the cache.
    /// </summary>
    private void ClearCache()
    {
      Factory.Reset();
      CorePipelineFactory.ClearCache();

      this.configurationSwitcher.RestoreWorkingConfiguration();
    }
  }
}
