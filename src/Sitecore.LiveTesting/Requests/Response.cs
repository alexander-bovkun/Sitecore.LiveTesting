namespace Sitecore.LiveTesting.Requests
{
  using System;

  /// <summary>
  /// Defines the class for responses.
  /// </summary>
  [Serializable]
  public class Response
  {
    /// <summary>
    /// The content.
    /// </summary>
    private string content;

    /// <summary>
    /// Initializes a new instance of the <see cref="Response"/> class.
    /// </summary>
    public Response()
    {
      this.Content = string.Empty;
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
  }
}
