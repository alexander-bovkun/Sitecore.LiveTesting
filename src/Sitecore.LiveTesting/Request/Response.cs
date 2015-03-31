namespace Sitecore.LiveTesting.Request
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Defines the class for responses.
  /// </summary>
  [Serializable]
  public class Response
  {
    /// <summary>
    /// The headers.
    /// </summary>
    private readonly IDictionary<string, string> headers;

    /// <summary>
    /// The content.
    /// </summary>
    private string content;

    /// <summary>
    /// The status description.
    /// </summary>
    private string statusDescription;

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    public Response()
    {
      this.headers = new Dictionary<string, string>();

      this.Content = string.Empty;
      this.StatusDescription = string.Empty;
    }

    /// <summary>
    /// Gets the headers.
    /// </summary>
    public IDictionary<string, string> Headers
    {
      get { return this.headers; }
    }

    /// <summary>
    /// Gets or sets the content of response.
    /// </summary>
    public string Content
    {
      get
      {
        return this.content;
      }
      
      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.content = value;
      }
    }

    /// <summary>
    /// Gets or sets the status code of response.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Gets or sets the status description.
    /// </summary>
    public string StatusDescription
    {
      get
      {
        return this.statusDescription;
      }

      set
      {
        if (value == null)
        {
          throw new ArgumentNullException("value");
        }

        this.statusDescription = value;
      }
    }
  }
}
