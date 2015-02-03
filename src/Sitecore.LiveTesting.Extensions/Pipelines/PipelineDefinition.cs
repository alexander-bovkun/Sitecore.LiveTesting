namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System.Collections.Generic;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class that represents runtime pipeline information.
  /// </summary>
  public class PipelineDefinition
  {
    /// <summary>
    /// The processors.
    /// </summary>
    private readonly IList<ProcessorDefinition> processors;
    
    /// <summary>
    /// The name.
    /// </summary>
    private string name;

    /// <summary>
    /// The domain.
    /// </summary>
    private string domain;

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineDefinition" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public PipelineDefinition([NotNull] string name) : this(name, string.Empty)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineDefinition" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="domain">The domain.</param>
    public PipelineDefinition([NotNull] string name, [NotNull] string domain) : this(name, domain, new List<ProcessorDefinition>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PipelineDefinition" /> class.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="domain">The domain.</param>
    /// <param name="processors">The processors.</param>
    protected PipelineDefinition([NotNull] string name, [NotNull] string domain, [NotNull] IList<ProcessorDefinition> processors)
    {
      Assert.ArgumentNotNullOrEmpty(name, "name");
      Assert.ArgumentNotNull(domain, "domain");
      Assert.ArgumentNotNull(processors, "processors");

      this.name = name;
      this.domain = domain;
      this.processors = processors;
    }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    [NotNull]
    public string Name 
    {
      get
      {
        return this.name;
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");
        this.name = value;
      }
    }

    /// <summary>
    /// Gets or sets the domain.
    /// </summary>
    /// <value>The domain.</value>
    [NotNull]
    public string Domain 
    {
      get
      {
        return this.domain;
      }

      set
      {
        Assert.ArgumentNotNull(value, "value");
        this.domain = value;
      }
    }

    /// <summary>
    /// Gets the processors.
    /// </summary>
    /// <value>The processors.</value>
    [NotNull]
    public IList<ProcessorDefinition> Processors 
    {
      get
      {
        return this.processors;
      }
    }
  }
}
