namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.IO;
  using System.Text;
  using System.Web;
  using System.Web.Hosting;

  /// <summary>
  /// Defines the more convenient version of <see cref="SimpleWorkerRequest"/> to use.
  /// </summary>
  internal class WorkerRequest : SimpleWorkerRequest
  {
    /// <summary>
    /// The request.
    /// </summary>
    private readonly Request request;

    /// <summary>
    /// The writer.
    /// </summary>
    private readonly StringWriter writer;

    /// <summary>
    /// The context.
    /// </summary>
    private HttpContext context;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerRequest"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    internal WorkerRequest(Request request) : this(request, new StringWriter())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerRequest" /> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="writer">The writer.</param>
    internal WorkerRequest(Request request, StringWriter writer) : base(request.Path, request.QueryString, writer)
    {
      if (request == null)
      {
        throw new ArgumentNullException("request");
      }

      if (writer == null)
      {
        throw new ArgumentNullException("writer");
      }

      this.request = request;
      this.writer = writer;
    }

    /// <summary>
    /// Gets the response text.
    /// </summary>
    /// <value>The response text.</value>
    internal string ResponseText
    {
      get
      {
        return this.writer.GetStringBuilder().ToString();
      }
    }

    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    internal HttpContext Context
    {
      get
      {
        return this.context;
      }
    }

    /// <summary>
    /// Registers for an optional notification when all the response data is sent.
    /// </summary>
    /// <param name="callback">The notification callback that is called when all data is sent (out-of-band).</param>
    /// <param name="extraData">An additional parameter to the callback.</param>
    public override void SetEndOfSendNotification(EndOfSendNotification callback, object extraData)
    {
      base.SetEndOfSendNotification(callback, extraData);

      HttpContext contextCandidate = extraData as HttpContext;

      if (contextCandidate != null)
      {
        this.context = contextCandidate;
      }
    }

    /// <summary>
    /// Gets the query string.
    /// </summary>
    /// <returns>The query string.</returns>
    public override string GetQueryString()
    {
      return this.request.QueryString;
    }

    /// <summary>
    /// Returns the HTTP request verb.
    /// </summary>
    /// <returns>The HTTP verb for this request.</returns>
    public override string GetHttpVerbName()
    {
      return this.request.Verb;
    }

    /// <summary>
    /// Returns the local port number.
    /// </summary>
    /// <returns>The local port number.</returns>
    public override int GetLocalPort()
    {
      return this.request.Port;
    }

    /// <summary>
    /// Returns the portion of the HTTP request body that has already been read.
    /// </summary>
    /// <returns>The portion of the HTTP request body that has been read.</returns>
    public override byte[] GetPreloadedEntityBody()
    {
      return Encoding.Unicode.GetBytes(this.request.Data);
    }

    /// <summary>
    /// Gets the length of the portion of the HTTP request body that has currently been read.
    /// </summary>
    /// <returns>An integer containing the length of the currently read HTTP request body.</returns>
    public override int GetPreloadedEntityBodyLength()
    {
      return Encoding.Unicode.GetBytes(this.request.Data).Length;
    }

    /// <summary>
    /// Gets the length of the entire HTTP request body.
    /// </summary>
    /// <returns>An integer containing the length of the entire HTTP request body.</returns>
    public override int GetTotalEntityBodyLength()
    {
      return this.GetPreloadedEntityBodyLength();
    }

    /// <summary>
    /// Returns a value indicating whether all request data is available and no further reads from the client are required.
    /// </summary>
    /// <returns>true if all request data is available; otherwise, false.</returns>
    public override bool IsEntireEntityBodyIsPreloaded()
    {
      return true;
    }
  }
}
