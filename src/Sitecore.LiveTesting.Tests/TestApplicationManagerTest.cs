namespace Sitecore.LiveTesting.Tests
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Runtime.Remoting;
  using System.Web.Hosting;
  using Xunit;
  using ApplicationHost = Sitecore.LiveTesting.ApplicationHost;

  /// <summary>
  /// Defines the test class for <see cref="TestApplicationManager"/>.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "It is test")]
  public class TestApplicationManagerTest : IDisposable
  {
    /// <summary>
    /// The application host.
    /// </summary>
    private readonly ApplicationHost applicationHost = new ApplicationHost("ApplicationId", "/", "..\\Website");

    /// <summary>
    /// The dispose.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "It is test")]
    public void Dispose()
    {
      ApplicationManager.GetApplicationManager().ShutdownApplication(this.applicationHost.ApplicationId);
    }

    /// <summary>
    /// Should start application.
    /// </summary>
    [Fact]
    public void ShouldStartApplication()
    {
      TestApplicationManager applicationManager = new TestApplicationManager();

      TestApplication result = applicationManager.StartApplication(this.applicationHost);

      Assert.NotNull(result);
      Assert.True(RemotingServices.IsObjectOutOfAppDomain(result));
      Assert.True(RemotingServices.IsTransparentProxy(result));
      Assert.Contains(this.applicationHost.ApplicationId, ApplicationManager.GetApplicationManager().GetRunningApplications().Select(app => app.ID));
    }

    /// <summary>
    /// Should return null for not running applications.
    /// </summary>
    [Fact]
    public void ShouldReturnNullForNotRunningApplications()
    {
      TestApplicationManager applicationManager = new TestApplicationManager();

      TestApplication result = applicationManager.GetRunningApplication(this.applicationHost);

      Assert.Null(result);
    }

    /// <summary>
    /// Should return application instance for running applications.
    /// </summary>
    [Fact]
    public void ShouldReturnApplicationInstanceForRunningApplications()
    {
      TestApplicationManager applicationManager = new TestApplicationManager();
      TestApplication application = (TestApplication)ApplicationManager.GetApplicationManager().CreateObject(this.applicationHost.ApplicationId, typeof(TestApplication), this.applicationHost.VirtualPath, Path.GetFullPath(this.applicationHost.PhysicalPath), false, true);
      
      TestApplication result = applicationManager.GetRunningApplication(this.applicationHost);

      Assert.Equal(application, result);
    }

    /// <summary>
    /// Should stop application.
    /// </summary>
    [Fact]
    public void ShouldStopApplication()
    {
      TestApplicationManager applicationManager = new TestApplicationManager();
      TestApplication application = (TestApplication)ApplicationManager.GetApplicationManager().CreateObject(this.applicationHost.ApplicationId, typeof(TestApplication), this.applicationHost.VirtualPath, Path.GetFullPath(this.applicationHost.PhysicalPath), false, true);

      applicationManager.StopApplication(application);

      Assert.DoesNotContain(this.applicationHost.ApplicationId, ApplicationManager.GetApplicationManager().GetRunningApplications().Select(app => app.ID));
    }

    /// <summary>
    /// Should return all running applications.
    /// </summary>
    [Fact]
    public void ShouldReturnAllRunningApplications()
    {
      TestApplicationManager applicationManager = new TestApplicationManager();
      TestApplication application = (TestApplication)ApplicationManager.GetApplicationManager().CreateObject(this.applicationHost.ApplicationId, typeof(TestApplication), this.applicationHost.VirtualPath, Path.GetFullPath(this.applicationHost.PhysicalPath), false, true);

      TestApplication[] result = applicationManager.GetRunningApplications().ToArray();

      Assert.Equal(1, result.Length);
      Assert.Contains(application, result);
    }
  }
}
