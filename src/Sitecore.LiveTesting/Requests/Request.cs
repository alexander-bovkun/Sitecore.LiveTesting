namespace Sitecore.LiveTesting.Requests
{
  using System;

  /// <summary>
  /// Defines the class for requests.
  /// </summary>
  [Serializable]
  public class Request
  {
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
    /// Initializes a new instance of the <see cref="Request"/> class.
    /// </summary>
    public Request()
    {
      this.Path = string.Empty;
      this.QueryString = string.Empty;
      this.Data = string.Empty;
      this.Port = 80;
      this.Verb = "GET";
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
    /// Gets or sets the port.
    /// </summary>
    public int Port { get; set; }

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
  }
}
