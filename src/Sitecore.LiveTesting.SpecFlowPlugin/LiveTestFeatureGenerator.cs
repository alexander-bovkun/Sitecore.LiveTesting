namespace Sitecore.LiveTesting.SpecFlowPlugin
{
  using System;
  using System.CodeDom;
  using System.Linq;
  using System.Web.Hosting;
  using TechTalk.SpecFlow.Generator.UnitTestConverter;
  using TechTalk.SpecFlow.Parser.SyntaxElements;

  /// <summary>
  /// Defines the LiveTestFeatureGenerator class.
  /// </summary>
  public class LiveTestFeatureGenerator : IFeatureGenerator
  {
    /// <summary>
    /// The integration tag.
    /// </summary>
    private const string IntegrationTag = "live";

    /// <summary>
    /// The base feature generator.
    /// </summary>
    private readonly IFeatureGenerator baseFeatureGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveTestFeatureGenerator" /> class.
    /// </summary>
    /// <param name="baseFeatureGenerator">The base feature generator.</param>
    public LiveTestFeatureGenerator(IFeatureGenerator baseFeatureGenerator)
    {
      this.baseFeatureGenerator = baseFeatureGenerator;
    }

    /// <summary>
    /// Generates the unit test fixture.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <param name="testClassName">Name of the test class.</param>
    /// <param name="targetNamespace">The target namespace.</param>
    /// <returns>Instance of CodeNamespace.</returns>
    public CodeNamespace GenerateUnitTestFixture(Feature feature, string testClassName, string targetNamespace)
    {
      CodeNamespace result = this.baseFeatureGenerator.GenerateUnitTestFixture(feature, testClassName, targetNamespace);

      if (this.IsLiveTest(feature))
      {
        this.DecorateStaticMethods(result);
        this.DecorateFixtureClasses(result);
      }

      return result;
    }

    /// <summary>
    /// Gets a value indicating whether the processed feature represents live test or not.
    /// </summary>
    /// <param name="feature">The feature.</param>
    /// <returns><value>true</value> if feature represents live test, otherwise <value>false</value>.</returns>
    protected virtual bool IsLiveTest(Feature feature)
    {
      return (feature.Tags != null) && feature.Tags.Any(tag => tag.Name == IntegrationTag);
    }

    /// <summary>
    /// The decorate static methods.
    /// </summary>
    /// <param name="output">The output.</param>
    protected virtual void DecorateStaticMethods(CodeNamespace output)
    {
      const string InstantiateMethodName = "Instantiate";
      const string GetDefaultTestApplicationManagerMethodName = "GetDefaultTestApplicationManager";
      const string GetDefaultApplicationHostMethodName = "GetDefaultApplicationHost";
      const string DoCallbackMethodName = "DoCallBack";
      const string GetAppDomainMethodName = "GetAppDomain";
      const string GetApplicationManagerMethodName = "GetApplicationManager";
      const string ApplicationIdPropertyName = "ApplicationId";
      const string IsHostedPropertyName = "IsHosted";
      const string DefaultApplicationHostVariableName = "defaultApplicationHost";

      CodeTypeDeclaration type = output.Types.Cast<CodeTypeDeclaration>().Single();

      foreach (CodeMemberMethod method in type.Members.OfType<CodeMemberMethod>().Where(m => (m.Name != InstantiateMethodName) && (m.Name != GetDefaultTestApplicationManagerMethodName) && (m.Name != GetDefaultApplicationHostMethodName) && ((m.Attributes & (MemberAttributes.Static | MemberAttributes.Public)) == (MemberAttributes.Static | MemberAttributes.Public)) && (m.Parameters.Count == 0)))
      {
        CodeStatement[] originalCodeStatements = method.Statements.Cast<CodeStatement>().ToArray<CodeStatement>();

        CodeVariableReferenceExpression defaultApplicationHostReferenceExpression = new CodeVariableReferenceExpression(DefaultApplicationHostVariableName);
        CodeStatement defaultApplicationHostDeclarationStatement = new CodeVariableDeclarationStatement(typeof(TestApplicationHost), DefaultApplicationHostVariableName);
        CodeStatement defaultApplicationHostAssignStatement = new CodeAssignStatement(defaultApplicationHostReferenceExpression, new CodeMethodInvokeExpression(this.GetStaticMethodReference(type, GetDefaultApplicationHostMethodName), new CodeTypeOfExpression(type.Name)));
        
        CodeStatement domainInitializationStatement = new CodeExpressionStatement(new CodeMethodInvokeExpression(this.GetStaticMethodReference(type, InstantiateMethodName), new CodeTypeOfExpression(type.Name)));
        CodeStatement domainCallbackStatement = new CodeExpressionStatement(new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(new CodeMethodInvokeExpression(new CodeTypeReferenceExpression(typeof(ApplicationManager)), GetApplicationManagerMethodName), GetAppDomainMethodName, new CodePropertyReferenceExpression(defaultApplicationHostReferenceExpression, ApplicationIdPropertyName)), DoCallbackMethodName, new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(type.Name), method.Name)));

        method.Statements.Clear();
        method.Statements.Add(defaultApplicationHostDeclarationStatement);
        method.Statements.Add(defaultApplicationHostAssignStatement);
        method.Statements.Add(new CodeConditionStatement(new CodeBinaryOperatorExpression(new CodePropertyReferenceExpression(new CodeTypeReferenceExpression(typeof(HostingEnvironment)), IsHostedPropertyName), CodeBinaryOperatorType.BooleanOr, new CodeBinaryOperatorExpression(defaultApplicationHostReferenceExpression, CodeBinaryOperatorType.IdentityEquality, new CodePrimitiveExpression(null))), originalCodeStatements, new[] { domainInitializationStatement, domainCallbackStatement }));
      }
    }

    /// <summary>
    /// The get static method reference.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="methodName">The method name.</param>
    /// <returns>The <see cref="CodeMethodReferenceExpression"/>.</returns>
    protected virtual CodeMethodReferenceExpression GetStaticMethodReference(CodeTypeDeclaration type, string methodName)
    {
      if (type.Members.OfType<CodeMemberMethod>().Any(m => (m.Name == methodName) && ((m.Attributes & (MemberAttributes.Static | MemberAttributes.Public)) == (MemberAttributes.Static | MemberAttributes.Public)) && (m.Parameters.Count == 1)))
      {
        return new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(type.Name), methodName);
      }

      if (type.BaseTypes.Count == 0)
      {
        return null;
      }

      return new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(type.BaseTypes[0]), methodName);
    }

    /// <summary>
    /// The decorate fixture classes.
    /// </summary>
    /// <param name="output">The output.</param>
    protected virtual void DecorateFixtureClasses(CodeNamespace output)
    {
      CodeTypeDeclaration type = output.Types.Cast<CodeTypeDeclaration>().Single();
      DecorateFixtureClasses(type);
    }

    /// <summary>
    /// The decorate fixture classes.
    /// </summary>
    /// <param name="type">The type.</param>
    private static void DecorateFixtureClasses(CodeTypeDeclaration type)
    {
      foreach (CodeTypeDeclaration innerType in type.Members.OfType<CodeTypeDeclaration>())
      {
        if (innerType.CustomAttributes.OfType<CodeAttributeDeclaration>().All(attribute => attribute.Name != "Serializable"))
        {
          innerType.CustomAttributes.Add(new CodeAttributeDeclaration(new CodeTypeReference(typeof(SerializableAttribute))));
        }

        DecorateFixtureClasses(innerType);
      }      
    }
  }
}