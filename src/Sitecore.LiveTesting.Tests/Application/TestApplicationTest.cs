namespace Sitecore.LiveTesting.Tests
{
  using System;
  using System.Collections.Generic;
  using System.Web.Hosting;
  using NSubstitute;
  using Sitecore.LiveTesting.Application;
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
    /// Should throw not supported exception if method is not static.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionIfMethodIsNotStatic()
    {
      TestApplication application = new TestApplication();

      Assert.ThrowsDelegate action = () => application.ExecuteAction(this.GetType().GetMethod("ShouldExecuteAction"));

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Should execute action.
    /// </summary>
    [Fact]
    public void ShouldExecuteAction()
    {
      TestApplication application = new TestApplication();

      object result = application.ExecuteAction(this.GetType().GetMethod("Action"), "parameter");

      Assert.Equal("Parameter: parameter", result);
    }

    /// <summary>
    /// Should call initialization manager on construction.
    /// </summary>
    [Fact]
    public void ShouldCallInitializationManagerOnConstruction()
    {
      InitializationManager initializationManager = Substitute.For<InitializationManager>(new TestInitializationActionDiscoverer(), new InitializationActionExecutor());
      TestApplication application = new TestApplication(initializationManager);

      initializationManager.Received().Initialize(0, Arg.Is<TestApplicationInitializationContext>(context => context.Application == application));
    }

    /// <summary>
    /// Should call initialization manager on stop.
    /// </summary>
    [Fact]
    public void ShouldCallInitializationManagerOnStop()
    {
      InitializationManager initializationManager = Substitute.For<InitializationManager>(new TestInitializationActionDiscoverer(), new InitializationActionExecutor());
      TestApplication application = new TestApplication(initializationManager);

      ((IRegisteredObject)application).Stop(false);

      initializationManager.Received().Cleanup(0, Arg.Is<TestApplicationInitializationContext>(context => context.Application == application));
    }
  }
}
