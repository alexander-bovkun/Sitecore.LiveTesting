namespace Sitecore.LiveTesting
{
  using System;

  /// <summary>
  /// The class that provides basic information about the hosting environment.
  /// </summary>
  public class ApplicationHost
  {
    /// <summary>
    /// The application id.
    /// </summary>
    private readonly string applicationId;    

    /// <summary>
    /// The virtual path.
    /// </summary>
    private readonly string virtualPath;

    /// <summary>
    /// The physical path.
    /// </summary>
    private readonly string physicalPath;

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationHost"/> class.
    /// </summary>
    /// <param name="applicationId">The application id.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="physicalPath">The physical path.</param>
    public ApplicationHost(string applicationId, string virtualPath, string physicalPath)
    {
      if (applicationId == null)
      {
        throw new ArgumentNullException("applicationId");
      }      

      if (virtualPath == null)
      {
        throw new ArgumentNullException("virtualPath");
      }

      if (physicalPath == null)
      {
        throw new ArgumentNullException("physicalPath");
      }

      this.applicationId = applicationId;
      this.virtualPath = virtualPath;
      this.physicalPath = physicalPath;
    }

    /// <summary>
    /// Gets the application ID.
    /// </summary>
    public string ApplicationId
    {
      get { return this.applicationId; }
    }

    /// <summary>
    /// Gets the virtual path for the application.
    /// </summary>
    public string VirtualPath
    {
      get { return this.virtualPath; }
    }

    /// <summary>
    /// Gets the physical path for the application.
    /// </summary>
    public string PhysicalPath
    {
      get { return this.physicalPath; }
    }
  }
}
