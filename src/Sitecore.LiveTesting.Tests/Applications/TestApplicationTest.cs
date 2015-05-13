namespace Sitecore.LiveTesting.Tests.Applications
{
  using System;
  using System.Collections.Generic;
  using System.Web.Hosting;
  using NSubstitute;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.Initialization;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="TestApplication"/>.
  /// </summary>
  public class TestApplicationTest
  {
    /// <summary>
    /// The action.
    /// </summary>
    /// <param name="parameter">The parameter.</param>
    /// <returns>The result of the action.</returns>
    public static string Action(string parameter)
    {
      return "Parameter: " + parameter;
    }

    /// <summary>
    /// Should create object.
    /// </summary>
    [Fact]
    public void ShouldCreateObject()
    {
      TestApplication application = new TestApplication();

      object result = application.CreateObject(typeof(KeyValuePair<int, int>), 1, 2);

      Assert.Equal(new KeyValuePair<int, int>(1, 2), result);
    }

    /// <summary>
    /// Should execute action.
    /// </summary>
    [Fact]
    public void ShouldExecuteAction()
    {
      TestApplication application = new TestApplication();

      object result = application.ExecuteAction(new Func<string, string>(Action), "parameter");

      Assert.Equal("Parameter: parameter", result);
    }

    /// <summary>
    /// Should call initialization manager on construction.
    /// </summary>
    [Fact]
    public void ShouldCallInitializationManagerOnConstruction()
    {
      InitializationManager initializationManager = Substitute.For<InitializationManager>(new TestInitializationActionDiscoverer(), new InitializationActionExecutor());
      TestApplication application = new CustomTestApplication(initializationManager);

      initializationManager.Received().Initialize(0, Arg.Is<TestApplicationInitializationContext>(context => context.Application == application));
    }

    /// <summary>
    /// Should call initialization manager on stop.
    /// </summary>
    [Fact]
    public void ShouldCallInitializationManagerOnStop()
    {
      InitializationManager initializationManager = Substitute.For<InitializationManager>(new TestInitializationActionDiscoverer(), new InitializationActionExecutor());
      TestApplication application = new CustomTestApplication(initializationManager);

      ((IRegisteredObject)application).Stop(false);

      initializationManager.Received().Cleanup(0, Arg.Is<TestApplicationInitializationContext>(context => context.Application == application));
    }

    /// <summary>
    /// Defines the sample test application.
    /// </summary>
    private class CustomTestApplication : TestApplication
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="CustomTestApplication"/> class.
      /// </summary>
      /// <param name="initializationManager">The initialization manager.</param>
      public CustomTestApplication(InitializationManager initializationManager) : base(initializationManager)
      {
      }
    }
  }
}
