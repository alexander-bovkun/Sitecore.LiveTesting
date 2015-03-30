namespace Sitecore.LiveTesting.Initialization
{
  using System;
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
    /// Initializes a new instance of the <see cref="RequestInitializationContext"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    public RequestInitializationContext(Request request)
    {
      if (request == null)
      {
        throw new ArgumentNullException("request");
      }

      this.request = request;
    }

    /// <summary>
    /// Gets the request.
    /// </summary>
    public Request Request
    {
      get { return this.request; }
    }
  }
}
