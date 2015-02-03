namespace Sitecore.LiveTesting.Initialization
{
  using System;

  /// <summary>
  /// Defines the initialization action.
  /// </summary>
  public class InitializationAction
  {
    /// <summary>
    /// The id.
    /// </summary>
    private readonly string id;

    /// <summary>
    /// Initializes a new instance of the <see cref="InitializationAction"/> class.
    /// </summary>
    /// <param name="id">The identifier.</param>
    public InitializationAction(string id)
    {
      if (id == null)
      {
        throw new ArgumentNullException("id");
      }

      this.id = id;
    }

    /// <summary>
    /// Gets action key.
    /// </summary>
    public string Id
    {
      get { return this.id; }
    }

    /// <summary>
    /// Gets or sets the action state.
    /// </summary>
    public object State { get; set; }
  }
}
