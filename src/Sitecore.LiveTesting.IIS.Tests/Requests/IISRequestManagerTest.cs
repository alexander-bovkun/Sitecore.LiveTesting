﻿namespace Sitecore.LiveTesting.IIS.Tests.Requests
{
  using System;
  using Sitecore.LiveTesting.Applications;
  using Sitecore.LiveTesting.IIS.Applications;
  using Sitecore.LiveTesting.IIS.Requests;
  using Sitecore.LiveTesting.IIS.Tests.Applications;
  using Sitecore.LiveTesting.Initialization;
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
    /// Should perform initialization routines.
    /// </summary>
    [Fact]
    public void ShouldPerformInitializationRoutines()
    {
      using (IISTestApplicationManager applicationManager = new IISTestApplicationManager())
      {
        TestApplication application = applicationManager.StartApplication(new TestApplicationHost("RequestManagerTest", "/", "..\\Website"));

        application.ExecuteAction((Action)InitializationRoutinesTest);
      }
    }

    /// <summary>
    /// The request execution test.
    /// </summary>
    private static void RequestExecutionTest()
    {
      IISRequestManager requestManager = new IISRequestManager();
      Request request = new Request { Path = "\\IntegratedPipelineTestPage.aspx", QueryString = "queryString=test", Data = "data", Verb = "POST" };

      request.Headers.Add("custom-header", "header value");

      Response result = requestManager.ExecuteRequest(request);

      Assert.Equal(200, result.StatusCode);
      Assert.Equal("OK", result.StatusDescription);
      Assert.Equal(string.Empty, result.Content);
      Assert.Equal("header value", result.Headers["custom-response-header"]);
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

    /// <summary>
    /// The initialization routines test.
    /// </summary>
    private static void InitializationRoutinesTest()
    {
      IISRequestManager requestManager = new IISRequestManager();
      Request request = new Request { Path = "/TestPage.aspx", InitializationHandlers = { new InitializationHandler(typeof(SampleRequestInitializationHandler)) } };

      request.Headers.Add("custom-header", "header value");

      Response result = requestManager.ExecuteRequest(request);

      Assert.Contains("0123456789", result.Content);
    }

    /// <summary>
    /// Defines the sample request initialization handler.
    /// </summary>
    private class SampleRequestInitializationHandler
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="SampleRequestInitializationHandler"/> class.
      /// </summary>
      public SampleRequestInitializationHandler()
      {
        IISTestApplicationManagerTest.TestEnvironmentVariable = "0123456789";
      }
    }
  }
}