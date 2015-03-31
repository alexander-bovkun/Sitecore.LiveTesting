namespace Sitecore.LiveTesting.Tests.Request
{
  using System;
  using Sitecore.LiveTesting.Initialization;
  using Sitecore.LiveTesting.Request;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="RequestManager"/>.
  /// </summary>
  public class RequestManagerTest : LiveTest
  {
    /// <summary>
    /// Should request page.
    /// </summary>
    [Fact]
    public void ShouldRequestPage()
    {
      RequestManager manager = new RequestManager();
      Request request = new Request { Path = "TestPage.aspx" };

      Response response = manager.ExecuteRequest(request);

      Assert.Equal(200, response.StatusCode);
      Assert.Equal("Test page", response.Content);
    }

    /// <summary>
    /// Should call initialization handler before and after request.
    /// </summary>
    [Fact]
    public void ShouldCallInitializationHandlerBeforeAndAfterRequest()
    {
      RequestManager manager = new RequestManager();
      Request request = new Request { Path = "TestPage.aspx", InitializationHandlers = { { typeof(SampleRequestInitializationHandler), new object[] { "parameter" } } } };

      Response response = manager.ExecuteRequest(request);

      Assert.Equal("parameter", response.Content);
    }

    /// <summary>
    /// The sample request initialization handler.
    /// </summary>
    private class SampleRequestInitializationHandler : IInitializationContextAware, IDisposable
    {
      /// <summary>
      /// The parameter.
      /// </summary>
      private readonly string parameter;

      /// <summary>
      /// The initialization context.
      /// </summary>
      private RequestInitializationContext initializationContext;

      /// <summary>
      /// Initializes a new instance of the <see cref="SampleRequestInitializationHandler"/> class.
      /// </summary>
      /// <param name="parameter">The parameter.</param>
      public SampleRequestInitializationHandler(string parameter)
      {
        this.parameter = parameter;
      }

      /// <summary>
      /// The set initialization context.
      /// </summary>
      /// <param name="context">The context.</param>
      public void SetInitializationContext(object context)
      {
        this.initializationContext = (RequestInitializationContext)context;
      }

      /// <summary>
      /// The dispose.
      /// </summary>
      public void Dispose()
      {
        this.initializationContext.Response.Content = this.parameter;
      }
    }
  }
}
