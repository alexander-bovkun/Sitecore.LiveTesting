namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading;
  using System.Web;
  using System.Web.Hosting;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the more convenient version of <see cref="SimpleWorkerRequest"/> to use.
  /// </summary>
  internal class ClassicWorkerRequest : SimpleWorkerRequest
  {
    /// <summary>
    /// The connection count.
    /// </summary>
    private static int connectionCount;

    /// <summary>
    /// The connection id.
    /// </summary>
    private readonly int connectionId;

    /// <summary>
    /// The initialization manager.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// The request initialization context.
    /// </summary>
    private readonly RequestInitializationContext context;

    /// <summary>
    /// The end of send callback.
    /// </summary>
    private EndOfSendNotification endOfSendCallback;

    /// <summary>
    /// The end of send callback data.
    /// </summary>
    private object endOfSendCallbackData;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassicWorkerRequest"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization Manager.</param>
    /// <param name="context">The request initialization context.</param>
    /// <param name="writer">The writer.</param>
    internal ClassicWorkerRequest(InitializationManager initializationManager, RequestInitializationContext context, TextWriter writer) : base(context.Request.Path, context.Request.QueryString, writer)
    {
      if (initializationManager == null)
      {
        throw new ArgumentNullException("initializationManager");
      }

      if (context == null)
      {
        throw new ArgumentNullException("context");
      }

      this.connectionId = Interlocked.Increment(ref connectionCount);

      this.initializationManager = initializationManager;
      this.context = context;
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    internal Response Response
    {
      get { return this.context.Response; }
    }

    /// <summary>
    /// Gets the query string.
    /// </summary>
    /// <returns>The query string.</returns>
    public override string GetQueryString()
    {
      return this.context.Request.QueryString;
    }

    /// <summary>
    /// Returns the HTTP request verb.
    /// </summary>
    /// <returns>The HTTP verb for this request.</returns>
    public override string GetHttpVerbName()
    {
      return this.context.Request.Verb;
    }

    /// <summary>
    /// Returns HTTP version for the request.
    /// </summary>
    /// <returns>The HTTP version.</returns>
    public override string GetHttpVersion()
    {
      return this.context.Request.HttpVersion;
    }

    /// <summary>
    /// Returns local address.
    /// </summary>
    /// <returns>The local address.</returns>
    public override string GetLocalAddress()
    {
      return this.context.Request.Address;
    }

    /// <summary>
    /// Returns the local port number.
    /// </summary>
    /// <returns>The local port number.</returns>
    public override int GetLocalPort()
    {
      return this.context.Request.Port;
    }

    /// <summary>
    /// Returns the remote address.
    /// </summary>
    /// <returns>The remote address.</returns>
    public override string GetRemoteAddress()
    {
      return this.context.Request.ClientAddress;
    }

    /// <summary>
    /// Returns the remote port number.
    /// </summary>
    /// <returns>The remote port number. </returns>
    public override int GetRemotePort()
    {
      return this.context.Request.ClientPort;
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

      if (this.context.Request.ServerVariables.TryGetValue(name, out result))
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
      return this.context.Request.UserToken;
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

      if (this.context.Request.Headers.TryGetValue(name, out result))
      {
        return result;
      }

      return null;      
    }

    /// <summary>
    /// Returns unknown request headers.
    /// </summary>
    /// <returns>The unknown request headers.</returns>
    public override string[][] GetUnknownRequestHeaders()
    {
      int index = 0;

      foreach (KeyValuePair<string, string> header in this.context.Request.Headers)
      {
        if (HttpWorkerRequest.GetKnownRequestHeaderIndex(header.Key) == -1)
        {
          ++index;
        }
      }

      string[][] result = new string[index][];

      foreach (KeyValuePair<string, string> header in this.context.Request.Headers)
      {
        if (HttpWorkerRequest.GetKnownRequestHeaderIndex(header.Key) == -1)
        {
          result[--index] = new[] { header.Key, header.Value };
        }
      }

      return result;
    }

    /// <summary>
    /// Returns value indicating whether the request is secure or not.
    /// </summary>
    /// <returns>The value indicating whether the request is secure or not.</returns>
    public override bool IsSecure()
    {
      return this.context.Request.IsSecure;
    }

    /// <summary>
    /// Returns the portion of the HTTP request body that has already been read.
    /// </summary>
    /// <returns>The portion of the HTTP request body that has been read.</returns>
    public override byte[] GetPreloadedEntityBody()
    {
      return Encoding.Unicode.GetBytes(this.context.Request.Data);
    }

    /// <summary>
    /// Gets the length of the portion of the HTTP request body that has currently been read.
    /// </summary>
    /// <returns>An integer containing the length of the currently read HTTP request body.</returns>
    public override int GetPreloadedEntityBodyLength()
    {
      return Encoding.Unicode.GetBytes(this.context.Request.Data).Length;
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
    /// The get connection id.
    /// </summary>
    /// <returns>The connection id.</returns>
    public override long GetConnectionID()
    {
      return this.connectionId;
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

      this.context.Response.Content += File.ReadAllText(filename).Substring((int)offset, (int)length);
    }

    /// <summary>
    /// Handles the start of request.
    /// </summary>
    /// <param name="callback">The callback.</param>
    /// <param name="extraData">The extra data.</param>
    public override void SetEndOfSendNotification(EndOfSendNotification callback, object extraData)
    {
      base.SetEndOfSendNotification(callback, extraData);

      this.endOfSendCallback = callback;
      this.endOfSendCallbackData = extraData;

      HttpContext httpContext = extraData as HttpContext;
      this.context.HttpContext = httpContext;

      this.Initialization();
    }

    /// <summary>
    /// Handles the end of request.
    /// </summary>
    public override void EndOfRequest()
    {
      base.EndOfRequest();

      if (this.endOfSendCallback != null)
      {
        this.endOfSendCallback(this, this.endOfSendCallbackData);
        
        this.endOfSendCallback = null;
        this.endOfSendCallbackData = null;
      }

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

      this.initializationManager.Initialize((int)this.GetConnectionID(), this.context);
    }

    /// <summary>
    /// The termination.
    /// </summary>
    private void Termination()
    {
      this.initializationManager.Cleanup((int)this.GetConnectionID(), this.context);
    }
  }
}
