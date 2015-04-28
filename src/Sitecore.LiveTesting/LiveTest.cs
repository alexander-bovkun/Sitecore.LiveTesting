namespace Sitecore.LiveTesting
{
  using System;
  using System.Configuration;
  using System.Globalization;
  using System.IO;
  using System.Reflection;
  using Sitecore.LiveTesting.Applications;

  /// <summary>
  /// Defines the base class for tests.
  /// </summary>
  [DynamicConstruction]
  public class LiveTest : ContextBoundObject
  {
    /// <summary>
    /// The default application id.
    /// </summary>
    private const string DefaultApplicationId = "Sitecore.LiveTesting.Default";

    /// <summary>
    /// The website path setting name.
    /// </summary>
    private const string WebsitePathSettingName = "Sitecore.LiveTesting.WebsitePath";

    /// <summary>
    /// The name of method that gets default application manager.
    /// </summary>
    private const string GetDefaultTestApplicationManagerName = "GetDefaultTestApplicationManager";

    /// <summary>
    /// The name of method that gets default application host.
    /// </summary>
    private const string GetDefaultApplicationHostName = "GetDefaultApplicationHost";

    /// <summary>
    /// The default test application manager.
    /// </summary>
    private static readonly TestApplicationManager DefaultTestApplicationManager = new TestApplicationManager();

    /// <summary>
    /// Gets default test application manager for the specified test type.
    /// </summary>
    /// <param name="testType">The test type.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>An instance of <see cref="DefaultTestApplicationManager"/>.</returns>
    public static TestApplicationManager GetDefaultTestApplicationManager(Type testType, params object[] arguments)
    {
      return DefaultTestApplicationManager;
    }

    /// <summary>
    /// Gets default application host for the specified test type.
    /// </summary>
    /// <param name="testType">Type of the test.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>The default application host.</returns>
    public static TestApplicationHost GetDefaultApplicationHost(Type testType, params object[] arguments)
    {
      return new TestApplicationHost(DefaultApplicationId, "/", ConfigurationManager.AppSettings.Get(WebsitePathSettingName) ?? Directory.GetParent(Environment.CurrentDirectory).FullName);
    }

    /// <summary>
    /// Creates an instance of corresponding class.
    /// </summary>
    /// <param name="testType">Type of the test to instantiate.</param>
    /// <param name="arguments">The arguments.</param>
    /// <returns>Instance of the class.</returns>
    public static LiveTest Instantiate(Type testType, params object[] arguments)
    {
      MethodInfo getDefaultTestApplicationManagerMethod = Utility.GetInheritedMethod(testType, GetDefaultTestApplicationManagerName, new[] { typeof(Type), typeof(object[]) });
      MethodInfo getDefaultApplicationHostMethod = Utility.GetInheritedMethod(testType, GetDefaultApplicationHostName, new[] { typeof(Type), typeof(object[]) });

      if (getDefaultTestApplicationManagerMethod == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", testType.FullName, GetDefaultTestApplicationManagerName, typeof(LiveTest).FullName));
      }

      if (getDefaultApplicationHostMethod == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cannot create an instance of type '{0}' because there is no '{1}' static method defined in its inheritance hierarchy. See '{2}' methods for an example of corresponding method signature.", testType.FullName, GetDefaultApplicationHostName, typeof(LiveTest).FullName));
      }

      object[] allArguments = { testType, arguments };

      TestApplicationManager testApplicationManager = (TestApplicationManager)getDefaultTestApplicationManagerMethod.Invoke(null, allArguments);

      if (testApplicationManager == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to get an instance of '{0}'.", typeof(TestApplicationManager).FullName));
      }

      TestApplicationHost host = (TestApplicationHost)getDefaultApplicationHostMethod.Invoke(null, allArguments);

      if (host == null)
      {
        throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to get an instance of '{0}'.", typeof(TestApplicationHost).FullName));
      }

      TestApplication testApplication = testApplicationManager.StartApplication(host);

      if (testApplication == null)
      {
        throw new InvalidOperationException("Failed to get application to execute tests in.");
      }

      return (LiveTest)testApplication.CreateObject(testType, arguments);
    }
  }
}
