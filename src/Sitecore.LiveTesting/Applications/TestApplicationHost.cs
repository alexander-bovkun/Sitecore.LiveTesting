namespace Sitecore.LiveTesting.Applications
{
  using System;
  using System.Globalization;
  using System.Runtime.Serialization;
  using System.Security.Permissions;

  /// <summary>
  /// The class that provides basic information about the hosting environment.
  /// </summary>
  [Serializable]
  public class TestApplicationHost : ISerializable
  {
    /// <summary>
    /// The applicationId serialization key.
    /// </summary>
    private const string ApplicationIdSerializationKey = "applicationId";

    /// <summary>
    /// The virtualPath serialization key.
    /// </summary>
    private const string VirtualPathSerializationKey = "virtualPath";

    /// <summary>
    /// The physicalPath serialization key.
    /// </summary>
    private const string PhysicalPathSerializationKey = "physicalPath";

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
    /// Initializes a new instance of the <see cref="TestApplicationHost"/> class.
    /// </summary>
    /// <param name="applicationId">The application id.</param>
    /// <param name="virtualPath">The virtual path.</param>
    /// <param name="physicalPath">The physical path.</param>
    public TestApplicationHost(string applicationId, string virtualPath, string physicalPath)
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
    /// Initializes a new instance of the <see cref="TestApplicationHost"/> class.
    /// </summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The streaming context.</param>
    protected TestApplicationHost(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException("info");
      }

      this.applicationId = info.GetString(ApplicationIdSerializationKey);

      if (this.applicationId == null)
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Serialization info should not contain null value for '{0}' key.", ApplicationIdSerializationKey));
      }

      this.virtualPath = info.GetString(VirtualPathSerializationKey);

      if (this.virtualPath == null)
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Serialization info should not contain null value for '{0}' key.", VirtualPathSerializationKey));
      }

      this.physicalPath = info.GetString(PhysicalPathSerializationKey);

      if (this.physicalPath == null)
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Serialization info should not contain null value for '{0}' key.", PhysicalPathSerializationKey));
      }
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

    /// <summary>
    /// Gets object data for the object being serialized.
    /// </summary>
    /// <param name="info">The serialization information.</param>
    /// <param name="context">The streaming context.</param>
    [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
    public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null)
      {
        throw new ArgumentNullException("info");
      }

      info.AddValue(ApplicationIdSerializationKey, this.applicationId);
      info.AddValue(VirtualPathSerializationKey, this.virtualPath);
      info.AddValue(PhysicalPathSerializationKey, this.physicalPath);
    }
  }
}
