namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using System.CodeDom;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.LiveTesting.Initialization;
  using TechTalk.SpecFlow.Generator;
  using TechTalk.SpecFlow.Generator.UnitTestConverter;
  using TechTalk.SpecFlow.Parser.SyntaxElements;

  /// <summary>
  /// The live test decorator.
  /// </summary>
  public class LiveTestDecorator : ITestClassTagDecorator, ITestMethodTagDecorator
  {
    /// <summary>
    /// The integration tag.
    /// </summary>
    private const string LiveTestTag = "live";

    /// <summary>
    /// The discoverer.
    /// </summary>
    private readonly TagMappingDiscoverer discoverer;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestDecorator"/> class.
    /// </summary>
    /// <param name="discoverer">The discoverer.</param>
    public LiveTestDecorator(TagMappingDiscoverer discoverer)
    {
      this.discoverer = discoverer;
    }

    /// <summary>
    /// Gets the priority.
    /// </summary>
    public virtual int Priority
    {
      get { return 0; }
    }

    /// <summary>
    /// Gets a value indicating whether remove processed tags.
    /// </summary>
    public virtual bool RemoveProcessedTags
    {
      get { return false; }
    }

    /// <summary>
    /// Gets a value indicating whether apply other decorators for processed tags.
    /// </summary>
    public virtual bool ApplyOtherDecoratorsForProcessedTags
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the discoverer.
    /// </summary>
    protected TagMappingDiscoverer Discoverer
    {
      get { return this.discoverer; }
    }

    /// <summary>
    /// The can decorate from.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="generationContext">The generation context.</param>
    /// <returns>The <see cref="bool"/>.</returns>
    public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
    {
      return (tagName == LiveTestTag) || (!string.IsNullOrEmpty(this.Discoverer.GetTagMapping(generationContext.Feature.SourceFile, tagName)));
    }

    /// <summary>
    /// The decorate from.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="generationContext">The generation context.</param>
    public void DecorateFrom(string tagName, TestClassGenerationContext generationContext)
    {
      if (tagName == LiveTestTag)
      {
        string baseClassName = this.GetLiveTestBaseClassName(generationContext.Feature);

        this.InheritFromIntegrationTestBase(generationContext.TestClass, baseClassName);
      }

      this.DecorateWithDiscoveredTagMappings(generationContext.Feature.SourceFile, tagName, generationContext.TestClass);
    }

    /// <summary>
    /// The can decorate from.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="generationContext">The generation context.</param>
    /// <param name="testMethod">The test method.</param>
    /// <returns>The <see cref="bool"/>.</returns>
    public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
    {
      return !string.IsNullOrEmpty(this.Discoverer.GetTagMapping(generationContext.Feature.SourceFile, tagName));
    }

    /// <summary>
    /// The decorate from.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="generationContext">The generation context.</param>
    /// <param name="testMethod">The test method.</param>
    public void DecorateFrom(string tagName, TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
    {
      this.DecorateWithDiscoveredTagMappings(generationContext.Feature.SourceFile, tagName, testMethod);
    }

    /// <summary>
    /// Requires the integration test support.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns>The base class name.</returns>
    protected virtual string GetLiveTestBaseClassName(Feature feature)
    {
      if (feature.Tags != null)
      {
        IEnumerable<Tag> inheritenceTags = feature.Tags.Where(tag => tag.Name.StartsWith(":")).ToArray();

        if (inheritenceTags.Count() == 1)
        {
          return inheritenceTags.Single().Name.Substring(1);
        }
      }

      return typeof(LiveTest).FullName;
    }

    /// <summary>
    /// Inherits from integration test base.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="baseClassName">The base class name.</param>
    protected virtual void InheritFromIntegrationTestBase(CodeTypeDeclaration type, string baseClassName)
    {
      type.BaseTypes.Insert(0, new CodeTypeReference(baseClassName));
    }

    /// <summary>
    /// The decorate with discovered tag mappings.
    /// </summary>
    /// <param name="featureFile">The feature File.</param>
    /// <param name="tagName">The tag Name.</param>
    /// <param name="type">The type.</param>
    protected virtual void DecorateWithDiscoveredTagMappings(string featureFile, string tagName, CodeTypeDeclaration type)
    {
      string codeSnippet = this.discoverer.GetTagMapping(featureFile, tagName);

      if (!string.IsNullOrEmpty(codeSnippet))
      {
        CodeAttributeDeclarationCollection attributeDeclarations = DeserializeAttributeDeclarations(codeSnippet);

        if (attributeDeclarations != null)
        {
          type.CustomAttributes.AddRange(attributeDeclarations);
        }
      }
    }

    /// <summary>
    /// The decorate with discovered tag mappings.
    /// </summary>
    /// <param name="featureFile">The feature File.</param>
    /// <param name="tagName">The tag Name.</param>
    /// <param name="method">The method.</param>
    protected virtual void DecorateWithDiscoveredTagMappings(string featureFile, string tagName, CodeMemberMethod method)
    {
      string codeSnippet = this.discoverer.GetTagMapping(featureFile, tagName);

      if (!string.IsNullOrEmpty(codeSnippet))
      {
        CodeAttributeDeclarationCollection attributeDeclarations = DeserializeAttributeDeclarations(codeSnippet);

        if (attributeDeclarations != null)
        {
          method.CustomAttributes.AddRange(attributeDeclarations);
        }
      }
    }

    /// <summary>
    /// The deserialize attribute declarations.
    /// </summary>
    /// <param name="codeSnippet">The code snippet.</param>
    /// <returns>The <see cref="CodeAttributeDeclaration"/>.</returns>
    private static CodeAttributeDeclarationCollection DeserializeAttributeDeclarations(string codeSnippet)
    {
      CodeAttributeDeclarationCollection result = new CodeAttributeDeclarationCollection();
      List<CodeAttributeArgument> arguments = new List<CodeAttributeArgument>();

      int attributeTypeNameStart = -1;
      int attributeTypeNameEnd = -1;
      bool attributeTypeNameSpecified = false;
      int parameterNameStart = -1;
      int parameterExpressionStart = -1;
      int openedBracesCount = 0;

      for (int index = 0; index < codeSnippet.Length; ++index)
      {
        switch (codeSnippet[index])
        {
          case '{':
            {
              ++openedBracesCount;
              parameterNameStart = index + 1;

              break;
            }

          case '}':
            {
              --openedBracesCount;
              
              string parameterName = codeSnippet.Substring(parameterNameStart, parameterExpressionStart - parameterNameStart - 1).Trim();
              string parameterExpression = codeSnippet.Substring(parameterExpressionStart, index - parameterExpressionStart).Trim();

              arguments.Add(new CodeAttributeArgument(parameterName, new CodeSnippetExpression(parameterExpression)));

              break;
            }

          case ',':
            {
              if (openedBracesCount == 1)
              {
                parameterExpressionStart = index + 1;
              }

              break;
            }

          case ' ':
            {
              attributeTypeNameSpecified = false;

              break;
            }

          default:
            {
              if ((openedBracesCount == 0) && (!attributeTypeNameSpecified))
              {
                if (attributeTypeNameStart != -1)
                {
                  string parameterName = codeSnippet.Substring(attributeTypeNameStart, attributeTypeNameEnd - attributeTypeNameStart + 1).Trim();

                  result.Add(new CodeAttributeDeclaration(parameterName, arguments.ToArray()));
                  arguments.Clear();
                }

                attributeTypeNameStart = index;
              }

              attributeTypeNameSpecified = openedBracesCount == 0;

              if (attributeTypeNameSpecified)
              {
                attributeTypeNameEnd = index;                
              }

              break;
            }
        }
      }

      if (attributeTypeNameStart != -1)
      {
        string parameterName = codeSnippet.Substring(attributeTypeNameStart, attributeTypeNameEnd - attributeTypeNameStart + 1).Trim();

        result.Add(new CodeAttributeDeclaration(parameterName, arguments.ToArray()));
      }

      return result;
    }
  }
}
