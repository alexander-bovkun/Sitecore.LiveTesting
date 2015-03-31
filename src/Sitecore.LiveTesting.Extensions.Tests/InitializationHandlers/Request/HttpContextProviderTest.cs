namespace Sitecore.LiveTesting.Extensions.Tests.InitializationHandlers.Request
{
  using System;
  using System.IO;
  using System.Web;
  using Sitecore.LiveTesting.Extensions.InitializationHandlers.Request;
  using Sitecore.LiveTesting.Initialization;
  using Sitecore.LiveTesting.Request;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="HttpContextProviderTest"/>.
  /// </summary>
  public class HttpContextProviderTest
  {
    /// <summary>
    /// Should throw not supported exception for other than request initialization context type of contexts.
    /// </summary>
    [Fact]
    public void ShouldThrowNotSupportedExceptionForOtherThanRequestInitializationContextTypeOfContexts()
    {
      HttpContextProvider provider = new HttpContextProvider();

      Assert.ThrowsDelegate action = () => provider.SetInitializationContext(new object());

      Assert.Throws<NotSupportedException>(action);
    }

    /// <summary>
    /// Should set and restore http context from initialization context.
    /// </summary>
    [Fact]
    public void ShouldSetAndRestoreHttpContextFromInitializationContext()
    {
      HttpContext.Current = null;
      HttpContext httpContext = new HttpContext(new HttpRequest(string.Empty, "http://url", string.Empty), new HttpResponse(new StringWriter()));

      using (HttpContextProvider provider = new HttpContextProvider())
      {
        provider.SetInitializationContext(new RequestInitializationContext(new Request(), new Response()) { HttpContext = httpContext });

        Assert.Equal(httpContext, HttpContext.Current);
      }

      Assert.Null(HttpContext.Current);
    }
  }
}
