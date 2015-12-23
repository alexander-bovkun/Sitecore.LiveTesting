namespace Sitecore.LiveTesting.Requests
{
  using System;

  /// <summary>
  /// Defines the base class for all request managers.
  /// </summary>
  public abstract class RequestManager : MarshalByRefObject
  {
    /// <summary>
    /// Executes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The <see cref="Response"/>.</returns>
    public abstract Response ExecuteRequest(Request request);
  }
}
