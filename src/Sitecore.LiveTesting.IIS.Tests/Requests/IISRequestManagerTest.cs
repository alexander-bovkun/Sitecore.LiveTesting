namespace Sitecore.LiveTesting.IIS.Tests.Requests
{
  using System;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.IIS.Applications;
  using Sitecore.LiveTesting.IIS.Requests;
  using Sitecore.LiveTesting.Requests;
  using Xunit;

  /// <summary>
  /// Defines the test class for <see cref="IISRequestManager"/>.
  /// </summary>
  public class IISRequestManagerTest : SequentialTest
  {
    /// <summary>
    /// Should throw invalid operation exception if run from non hosted environment.
    /// </summary>
    [Fact]
    public void ShouldThrowInvalidOperationExceptionIfRunFromNonHostedEnvironment()
    {
      Assert.ThrowsDelegate action = RequestExecutionTest;

      Assert.Throws<InvalidOperationException>(action);
    }

    /// <summary>
    /// Should execute request.
    /// </summary>
    [Fact]
    public void ShouldExecuteRequest()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplication application = applicationManager.StartApplication(new TestApplicationHost("RequestManagerTest", "/", "..\\Website"));

        application.ExecuteAction((Action)RequestExecutionTest);
      }
    }

    /// <summary>
    /// Should handle failing requests.
    /// </summary>
    [Fact]
    public void ShouldHandleFailingRequests()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplication application = applicationManager.StartApplication(new TestApplicationHost("RequestManagerTest", "/", "..\\Website"));

        application.ExecuteAction((Action)FailingRequestTest);
      }
    }

    /// <summary>
    /// The request execution test.
    /// </summary>
    private static void RequestExecutionTest()
    {
      IISRequestManager requestManager = new IISRequestManager();

      Response result = requestManager.ExecuteRequest(new Request { Path = "\\TestPage.aspx" });

      Assert.Equal(200, result.StatusCode);
      Assert.Equal(string.Empty, result.Content);
    }

    /// <summary>
    /// The failing request test.
    /// </summary>
    private static void FailingRequestTest()
    {
      IISRequestManager requestManager = new IISRequestManager();

      Response result = requestManager.ExecuteRequest(new Request { Path = "\\NonExistingPage.aspx" });

      Assert.Equal(404, result.StatusCode);
      Assert.Contains("The resource cannot be found", result.Content);
    }
  }
}
