namespace Sitecore.LiveTesting.Extensions.Pipelines
{
  using System.Xml.Linq;
  using Sitecore.Diagnostics;

  /// <summary>
  /// Defines the class that represents runtime processor information.
  /// </summary>
  public class ProcessorDefinition
  {
    /// <summary>
    /// The processor element name.
    /// </summary>
    private const string ProcessorElementName = "processor";

    /// <summary>
    /// The type attribute name.
    /// </summary>
    private const string TypeAttributeName = "type";

    /// <summary>
    /// The type reference attribute name.
    /// </summary>
    private const string TypeReferenceAttributeName = "ref";

    /// <summary>
    /// The method attribute name.
    /// </summary>
    private const string MethodAttributeName = "method";    

    /// <summary>
    /// The default method name.
    /// </summary>
    private const string DefaultMethodName = "Process";

    /// <summary>
    /// The run if aborted attribute name.
    /// </summary>
    private const string RunIfAbortedAttributeName = "runIfAborted";

    /// <summary>
    /// The string representation of true.
    /// </summary>
    private static readonly string StringRepresentationOfTrue = true.ToString();

    /// <summary>
    /// The processor element.
    /// </summary>
    private XElement processorElement;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorDefinition" /> class.
    /// </summary>
    public ProcessorDefinition() : this(new XElement(ProcessorElementName))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ProcessorDefinition" /> class.
    /// </summary>
    /// <param name="processorElement">The processor element.</param>
    public ProcessorDefinition([NotNull] XElement processorElement)
    {
      Assert.ArgumentNotNull(processorElement, "processorElement");
      
      this.processorElement = processorElement;
    }

    /// <summary>
    /// Gets or sets the processor element.
    /// </summary>
    /// <value>The processor element.</value>
    [NotNull]
    public XElement ProcessorElement
    {
      get 
      {
        return this.processorElement;
      }
      
      set
      {
        Assert.ArgumentNotNull(value, "value");

        this.processorElement = value;
      }
    }

    /// <summary>
    /// Gets or sets the type.
    /// </summary>
    /// <value>The type.</value>
    [CanBeNull]
    public string Type
    {
      get
      {
        return this.GetAttributeValue(TypeAttributeName);
      }
      
      set
      {
        this.SetAttributeValue(TypeAttributeName, value);
      }
    }

    /// <summary>
    /// Gets or sets the type reference.
    /// </summary>
    /// <value>The type reference.</value>
    [CanBeNull]
    public string TypeReference 
    {
      get
      {
        return this.GetAttributeValue(TypeReferenceAttributeName);
      }

      set
      {
        this.SetAttributeValue(TypeReferenceAttributeName, value);
      }
    }

    /// <summary>
    /// Gets or sets the name of the method.
    /// </summary>
    /// <value>The name of the method.</value>
    [NotNull]
    public string MethodName 
    {
      get
      {
        XAttribute methodAttribute = this.processorElement.Attribute(MethodAttributeName);

        if (methodAttribute == null)
        {
          return DefaultMethodName;
        }

        return methodAttribute.Value;
      }

      set
      {
        Assert.ArgumentNotNullOrEmpty(value, "value");

        this.processorElement.SetAttributeValue(MethodAttributeName, value == DefaultMethodName ? null : value);
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether pipeline should run if aborted.
    /// </summary>
    /// <value>
    /// <c>true</c> if processor is run if aborted; otherwise, <c>false</c>.
    /// </value>
    public bool RunIfAborted 
    {
      get
      {
        XAttribute attribute = this.processorElement.Attribute(RunIfAbortedAttributeName);

        if (attribute == null)
        {
          return false;
        }

        return bool.Parse(attribute.Value);
      }

      set
      {
        this.processorElement.SetAttributeValue(RunIfAbortedAttributeName, !value ? null : StringRepresentationOfTrue);
      }
    }

    /// <summary>
    /// Gets the attribute value.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns>The value of attribute</returns>
    [CanBeNull]
    private string GetAttributeValue([NotNull] string attributeName)
    {
      XAttribute attribute = this.processorElement.Attribute(attributeName);

      if (attribute == null)
      {
        return null;
      }

      return attribute.Value;
    }

    /// <summary>
    /// Sets the attribute value.
    /// </summary>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <param name="value">The value.</param>
    private void SetAttributeValue([NotNull] string attributeName, string value)
    {
      XAttribute attribute = this.processorElement.Attribute(attributeName);

      if (attribute == null)
      {
        this.processorElement.SetAttributeValue(attributeName, string.Empty);
        attribute = this.processorElement.Attribute(attributeName);
      }

      if (value == null)
      {
        this.processorElement.SetAttributeValue(attributeName, null);
      }
      else
      {
        attribute.Value = value;
      }
    }
  }
}
