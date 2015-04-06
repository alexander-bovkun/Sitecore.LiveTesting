namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.Collections.Generic;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines the class for requests.
  /// </summary>
  [Serializable]
  public class Request
  {
    /// <summary>
    /// The server variables.
    /// </summary>
    private readonly IDictionary<string, string> serverVariables;

    /// <summary>
    /// The headers.
    /// </summary>
    private readonly IDictionary<string, string> headers;

    /// <summary>
    /// The initialization handlers.
    /// </summary>
    private readonly IList<InitializationHandler> initializationHandlers;

    /// <summary>
    /// The path.
    /// </summary>
    private string path;

    /// <summary>
    /// The query string.
    /// </summary>
    private string queryString;

    /// <summary>
    /// The data.
    /// </summary>
    private string data;

    /// <summary>
    /// The verb.
    /// </summary>
    private string verb;

    /// <summary>
    /// The http version.
    /// </summary>
    private string httpVersion;

    /// <summary>
    /// The address.
    /// </summary>
    private string address;

    /// <summary>
    /// The client address.
    /// </summary>
    private string clientAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="Request"/> class.
    /// </summary>
    public Request()
    {
      this.serverVariables = new Dictionary<string, string>();
      this.headers = new Dictionary<string, string>();
      this.initializationHandlers = new List<InitializationHandler>();

      this.Path = string.Empty;
      this.QueryString = string.Empty;
      this.Data = string.Empty;
      this.Port = 80;
      this.Verb = "GET";
      this.HttpVersion = "HTTP/1.0";
      this.Address = "127.0.0.1";
      this.ClientAddress = "127.0.0.1";
    }

    /// <summary>
    /// Gets the server variables.
    /// </summary>
    public IDictionary<string, string> ServerVariables
    {
      get { return this.serverVariables; }
    }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    public IDictionary<string, string> Headers
    {
      get { return this.headers; }
    }

    /// <summary>
    /// Gets the initialization handlers.
    /// </summary>
    public IList<InitializationHandler> InitializationHandlers
    {
      get { return this.initializationHandlers; }
    }

    /// <summary>
    /// Gets or sets the path.
    /// </summary>
    public string Path
    {
      get
      {
        return this.path;
      }
      
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.path = value;
      }
    }

    /// <summary>
    /// Gets or sets the query string.
    /// </summary>
    public string QueryString
    {
      get
      {
        return this.queryString;
      }
      
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.queryString = value;
      }
    }

    /// <summary>
    /// Gets or sets request data.
    /// </summary>
    public string Data
    {
      get
      {
        return this.data;
      }
      
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.data = value;
      }
    }

    /// <summary>
    /// Gets or sets the local address.
    /// </summary>
    public string Address
    {
      get
      {
        return this.address;
      }

      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.address = value;
      } 
    }

    /// <summary>
    /// Gets or sets the port.
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Gets or sets the client address.
    /// </summary>
    public string ClientAddress
    {
      get
      {
        return this.clientAddress;
      }

      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.clientAddress = value;
      }
    }

    /// <summary>
    /// Gets or sets the client port.
    /// </summary>
    public int ClientPort { get; set; }

    /// <summary>
    /// Gets or sets the verb.
    /// </summary>
    public string Verb
    {
      get
      {
        return this.verb;
      }

      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.verb = value;
      }
    }

    /// <summary>
    /// Gets or sets the http version.
    /// </summary>
    public string HttpVersion
    {
      get
      {
        return this.httpVersion;
      }

      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.httpVersion = value;
      }
    }

    /// <summary>
    /// Gets or sets the user token.
    /// </summary>
    public IntPtr UserToken { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request is secure.
    /// </summary>
    public bool IsSecure { get; set; }
  }
}
