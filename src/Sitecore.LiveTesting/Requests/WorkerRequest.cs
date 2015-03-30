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
    /// The response.
    /// </summary>
    private readonly Response response;

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerRequest"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    internal WorkerRequest(Request request, Response response) : this(request, response, new StringWriter())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WorkerRequest"/> class.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <param name="response">The response.</param>
    /// <param name="writer">The writer.</param>
    internal WorkerRequest(Request request, Response response, StringWriter writer) : base(request.Path, request.QueryString, writer)
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
    /// Gets the response.
    /// </summary>
    internal Response Response
    {
      get { return this.response; }
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
    /// Returns HTTP version for the request.
    /// </summary>
    /// <returns>The HTTP version.</returns>
    public override string GetHttpVersion()
    {
      return this.request.HttpVersion;
    }

    /// <summary>
    /// Returns local address.
    /// </summary>
    /// <returns>The local address.</returns>
    public override string GetLocalAddress()
    {
      return this.request.Address;
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
    /// Returns the remote address.
    /// </summary>
    /// <returns>The remote address.</returns>
    public override string GetRemoteAddress()
    {
      return this.request.ClientAddress;
    }

    /// <summary>
    /// Returns the remote port number.
    /// </summary>
    /// <returns>The remote port number. </returns>
    public override int GetRemotePort()
    {
      return this.request.ClientPort;
    }

    /// <summary>
    /// The get server variable.
    /// </summary>
    /// <param name="name">The name of the variable.</param>
    /// <returns>The value of the variable.</returns>
    public override string GetServerVariable(string name)
    {
      string result;

      if (name == null)
      {
        throw new ArgumentNullException("name");
      }

      if (this.request.ServerVariables.TryGetValue(name, out result))
      {
        return result;
      }

      return string.Empty;
    }

    /// <summary>
    /// Returns the user token.
    /// </summary>
    /// <returns>The user token.</returns>
    public override IntPtr GetUserToken()
    {
      return this.request.UserToken;
    }

    /// <summary>
    /// Returns known request header.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <returns>The header.</returns>
    public override string GetKnownRequestHeader(int index)
    {
      return this.GetUnknownRequestHeader(HttpWorkerRequest.GetKnownRequestHeaderName(index));
    }

    /// <summary>
    /// Returns the unknown request header.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <returns>The unknown request header.</returns>
    public override string GetUnknownRequestHeader(string name)
    {
      string result;

      if (this.request.Headers.TryGetValue(name, out result))
      {
        return result;
      }

      return null;      
    }

    /// <summary>
    /// Returns value indicating whether the request is secure or not.
    /// </summary>
    /// <returns>The value indicating whether the request is secure or not.</returns>
    public override bool IsSecure()
    {
      return this.request.IsSecure;
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

    /// <summary>
    /// Sets the status for the response.
    /// </summary>
    /// <param name="statusCode">The status code.</param>
    /// <param name="statusDescription">The status description.</param>
    public override void SendStatus(int statusCode, string statusDescription)
    {
      this.Response.StatusCode = statusCode;
      this.Response.StatusDescription = statusDescription;
    }

    /// <summary>
    /// Sets the response content from memory.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <param name="length">The length.</param>
    public override void SendResponseFromMemory(byte[] data, int length)
    {
      this.Response.Content += Encoding.UTF8.GetString(data, 0, length);
    }

    /// <summary>
    /// Sets the response headers.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="value">The value.</param>
    public override void SendKnownResponseHeader(int index, string value)
    {
      string name = GetKnownResponseHeaderName(index);
      this.SendUnknownResponseHeader(name, value);
    }

    /// <summary>
    /// Sets the response headers.
    /// </summary>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="value">
    /// The value.
    /// </param>
    public override void SendUnknownResponseHeader(string name, string value)
    {
      if (name == null)
      {
        throw new ArgumentNullException("name");
      }

      if (this.Response.Headers.ContainsKey(name))
      {
        this.Response.Headers[name] = value;
      }
      else
      {
        this.Response.Headers.Add(name, value);
      }
    }

    /// <summary>
    /// Sets the response content from file.
    /// </summary>
    /// <param name="filename">The filename.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    public override void SendResponseFromFile(string filename, long offset, long length)
    {
      if (filename == null)
      {
        throw new ArgumentNullException("filename");
      }

      this.response.Content += File.ReadAllText(filename).Substring((int)offset, (int)length);
    }

    /// <summary>
    /// Handles the start of request.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="extraData">The extra data.</param>
    public override void SetEndOfSendNotification(EndOfSendNotification callback, object extraData)
    {
      base.SetEndOfSendNotification(callback, extraData);

      this.Initialization();
    }

    /// <summary>
    /// Handles the end of request.
    /// </summary>
    public override void EndOfRequest()
    {
      base.EndOfRequest();

      this.Termination();
    }

    /// <summary>
    /// The initialization.
    /// </summary>
    private void Initialization()
    {
      this.Response.Content = string.Empty;
      this.Response.Headers.Clear();
      this.Response.StatusCode = 0;
      this.Response.StatusDescription = string.Empty;
    }

    /// <summary>
    /// The termination.
    /// </summary>
    private void Termination()
    {
    }
  }
}
