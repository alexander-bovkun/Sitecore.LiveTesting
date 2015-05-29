namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using System;
  using System.CodeDom;
  using System.Collections.Generic;
  using System.Configuration;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Security;
  using System.Web.Configuration;
  using Sitecore.LiveTesting.SpecFlowPlugin.Config;
  using TechTalk.SpecFlow.Generator;
  using TechTalk.SpecFlow.Generator.UnitTestConverter;
  using TechTalk.SpecFlow.Parser.SyntaxElements;

  /// <summary>
  /// The live test decorator.
  /// </summary>
  public class LiveTestDecorator : ITestClassTagDecorator, ITestMethodTagDecorator
  {
    /// <summary>
    /// The configuration file name.
    /// </summary>
    private const string ConfigurationFileName = "Sitecore.LiveTesting.SpecFlowPlugin.config";

    /// <summary>
    /// The section name.
    /// </summary>
    private const string SectionName = "sitecore.livetesting.specflowplugin";

    /// <summary>
    /// The integration tag.
    /// </summary>
    private const string LiveTestTag = "live";

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
    /// The can decorate from.
    /// </summary>
    /// <param name="tagName">The tag name.</param>
    /// <param name="generationContext">The generation context.</param>
    /// <returns>The <see cref="bool"/>.</returns>
    public bool CanDecorateFrom(string tagName, TestClassGenerationContext generationContext)
    {
      PluginSection pluginSection = this.GetConfiguration(Path.GetDirectoryName(generationContext.Feature.SourceFile));
      
      return (tagName == LiveTestTag) || ((pluginSection != null) && pluginSection.TagMappings.Cast<TagMapping>().Any(tagMapping => tagMapping.Tag == tagName));
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
      PluginSection pluginSection = this.GetConfiguration(Path.GetDirectoryName(generationContext.Feature.SourceFile));

      return (pluginSection != null) && pluginSection.TagMappings.Cast<TagMapping>().Any(tagMapping => tagMapping.Tag == tagName);
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
        PluginSection pluginSection = this.GetConfiguration(Path.GetDirectoryName(feature.SourceFile));

        if ((pluginSection != null) && (pluginSection.BaseClass != null) && (!string.IsNullOrEmpty(pluginSection.BaseClass.Type)))
        {
          return pluginSection.BaseClass.Type;
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
      PluginSection pluginSection = this.GetConfiguration(Path.GetDirectoryName(featureFile));
      TagMapping tagMapping = null;

      if (pluginSection != null)
      {
        tagMapping = pluginSection.TagMappings.Cast<TagMapping>().SingleOrDefault(t => t.Tag == tagName);
      }

      if (tagMapping != null)
      {
        CodeAttributeDeclarationCollection attributeDeclarations = GetAttributeDeclarationsFromTagMapping(tagMapping);

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
      PluginSection pluginSection = this.GetConfiguration(Path.GetDirectoryName(featureFile));
      TagMapping tagMapping = null;

      if (pluginSection != null)
      {
        tagMapping = pluginSection.TagMappings.Cast<TagMapping>().SingleOrDefault(t => t.Tag == tagName);
      }

      if (tagMapping != null)
      {
        CodeAttributeDeclarationCollection attributeDeclarations = GetAttributeDeclarationsFromTagMapping(tagMapping);

        if (attributeDeclarations != null)
        {
          method.CustomAttributes.AddRange(attributeDeclarations);
        }
      }
    }

    /// <summary>
    /// Gets the configuration for the specified feature file directory.
    /// </summary>
    /// <param name="path">The path of the directory which contains the feature file.</param>
    /// <returns>The <see cref="Configuration"/>.</returns>
    protected virtual PluginSection GetConfiguration(string path)
    {
      WebConfigurationFileMap fileMap = new WebConfigurationFileMap();
      string rootPath = Path.GetFullPath(path);
      DirectoryInfo directory;

      try
      {
        directory = new DirectoryInfo(path);
      }
      catch (SecurityException)
      {
        return null;
      }

      path = rootPath;

      while (directory != null)
      {
        try
        {
          if (directory.GetFiles(ConfigurationFileName).Length > 0)
          {
            rootPath = directory.FullName;
          }

          directory = directory.Parent;
        }
        catch (SecurityException)
        {
          directory = null;
        }
      }

      path = path.Remove(0, rootPath.Length);

      if ((path.Length == 0) || (path[0] != Path.DirectorySeparatorChar))
      {
        path = Path.DirectorySeparatorChar + path;
      }

      fileMap.VirtualDirectories.Add("/", new VirtualDirectoryMapping(rootPath, true, ConfigurationFileName));

      PluginSection result;

      AppDomain.CurrentDomain.AssemblyResolve += ExecutingAssemblyResolver;
      try
      {
        result = (PluginSection)WebConfigurationManager.OpenMappedWebConfiguration(fileMap, path).GetSection(SectionName);
      }
      finally
      {
        AppDomain.CurrentDomain.AssemblyResolve -= ExecutingAssemblyResolver;
      }
      
      return result;
    }

    /// <summary>
    /// Gets the executing assembly when an assembly with the same name is requested.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="args">The arguments.</param>
    /// <returns>The current assembly when appropriate or <value>null</value> otherwise.</returns>
    private static Assembly ExecutingAssemblyResolver(object sender, ResolveEventArgs args)
    {
      return (args.Name == Assembly.GetExecutingAssembly().GetName().Name) ? Assembly.GetExecutingAssembly() : null;
    }

    /// <summary>
    /// Gets attribute declarations from tag mapping.
    /// </summary>
    /// <param name="tagMapping">The tag mapping.</param>
    /// <returns>The <see cref="CodeAttributeDeclaration"/>.</returns>
    private static CodeAttributeDeclarationCollection GetAttributeDeclarationsFromTagMapping(TagMapping tagMapping)
    {
      CodeAttributeDeclarationCollection result = new CodeAttributeDeclarationCollection();

      foreach (TagAttribute attribute in tagMapping.Attributes)
      {
        result.Add(new CodeAttributeDeclaration(attribute.Type, attribute.Arguments.Cast<TagAttributeArgument>().Select(arg => new CodeAttributeArgument(arg.Name, new CodeSnippetExpression(arg.CodeSnippet))).ToArray()));
      }

      return result;
    }
  }
}
