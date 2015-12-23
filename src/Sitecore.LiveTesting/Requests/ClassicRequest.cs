namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.Collections.Generic;

  /// <summary>
  /// Defines model for classic mode requests.
  /// </summary>
  public class ClassicRequest : Request
  {
    /// <summary>
    /// The server variables.
    /// </summary>
    private readonly IDictionary<string, string> serverVariables;

    /// <summary>
    /// The address.
    /// </summary>
    private string address;

    /// <summary>
    /// The client address.
    /// </summary>
    private string clientAddress;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClassicRequest"/> class.
    /// </summary>
    public ClassicRequest()
    {
      this.serverVariables = new Dictionary<string, string>();

      this.Port = 80;
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
    /// Gets or sets the user token.
    /// </summary>
    public IntPtr UserToken { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the request is secure.
    /// </summary>
    public bool IsSecure { get; set; }
  }
}
