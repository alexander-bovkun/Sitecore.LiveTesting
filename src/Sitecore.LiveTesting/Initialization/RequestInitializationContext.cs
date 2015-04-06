namespace Sitecore.LiveTesting.Initialization
{
  using System;
  using System.Web;
  using Sitecore.LiveTesting.Requests;

  /// <summary>
  /// Defines the initialization context that provides additional information about the request being executed.
  /// </summary>
  public class RequestInitializationContext
  {
    /// <summary>
    /// The request.
    /// </summary>
    private readonly Request request;

    /// <summary>
    /// The response.
    /// </summary>
    private readonly Response response;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestInitializationContext"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    public RequestInitializationContext(Request request, Response response)
    {
      if (request == null)
      {
        throw new ArgumentNullException("request");
      }

      if (response == null)
      {
        throw new ArgumentNullException("response");
      }

      this.request = request;
      this.response = response;
    }

    /// <summary>
    /// Gets the request.
    /// </summary>
    public Request Request
    {
      get { return this.request; }
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    public Response Response
    {
      get { return this.response; }
    }

    /// <summary>
    /// Gets or sets the http context.
    /// </summary>
    public HttpContext HttpContext { get; set; }
  }
}
