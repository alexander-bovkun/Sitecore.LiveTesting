namespace Sitecore.LiveTesting.Tests.Initialization
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using Sitecore.LiveTesting.Initialization;
  using Sitecore.LiveTesting.Requests;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="RequestInitializationActionDiscoverer"/>.
  /// </summary>
  public class RequestInitializationActionDiscovererTest
  {
    /// <summary>
    /// Should throw not supported exception for other than request initialization context type of contexts.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionForOtherThanRequestInitializationContextTypeOfContexts()
    {
      RequestInitializationActionDiscoverer discoverer = new RequestInitializationActionDiscoverer();

      Assert.ThrowsDelegate action = () => discoverer.GetInitializationActions(new object());

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Should get initialization actions from request.
    /// </summary>
    [Fact]
    public void ShouldGetInitializationActionsFromRequest()
    {
      object[] arguments = { "argument" };
      RequestInitializationContext context = new RequestInitializationContext(new Request { InitializationHandlers = { new InitializationHandler(typeof(SampleInitializationHandler), arguments) } }, new Response());
      RequestInitializationActionDiscoverer discoverer = new RequestInitializationActionDiscoverer();

      IEnumerable<InitializationAction> result = discoverer.GetInitializationActions(context).ToArray();

      Assert.Equal(1, result.Count());
      Assert.Equal(typeof(SampleInitializationHandler).FullName, result.Single().Id);
      Assert.IsType<InitializationHandler>(result.Single().State);
      Assert.Equal(typeof(SampleInitializationHandler), ((InitializationHandler)result.Single().State).Type);
      Assert.Equal(arguments, ((InitializationHandler)result.Single().State).Arguments);
      Assert.Equal(context, result.Single().Context);
    }

    /// <summary>
    /// The sample initialization handler.
    /// </summary>
    private class SampleInitializationHandler
    {
    }
  }
}
