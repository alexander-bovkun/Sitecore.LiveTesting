namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.IO;
  using System.Globalization;
  using System.Web;
  using System.Web.Hosting;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// The request manager.
  /// </summary>
  public class RequestManager : MarshalByRefObject
  {
    /// <summary>
    /// The initialization action discoverer.
    /// </summary>
    private readonly InitializationManager initializationManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestManager"/> class.
    /// </summary>
    public RequestManager() : this(new InitializationManager(new RequestInitializationActionDiscoverer(), new InitializationActionExecutor()))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestManager"/> class.
    /// </summary>
    /// <param name="initializationManager">The initialization action discoverer.</param>
    protected RequestManager(InitializationManager initializationManager)
    {
      if (initializationManager == null)
      {
        throw new ArgumentNullException("initializationManager");
      }

      this.initializationManager = initializationManager;
    }

    /// <summary>
    /// Gets the initialization manager.
    /// </summary>
    protected InitializationManager InitializationManager
    {
      get { return this.initializationManager; }
    }

    /// <summary>
    /// Executes the request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>The <see cref="Response"/>.</returns>
    public Response ExecuteRequest(Request request)
    {
      if (request == null)
      {
        throw new ArgumentNullException("request");
      }

      if (!HostingEnvironment.IsHosted)
      {
        throw new InvalidOperationException("Cannot execute request in environment which is not hosted.");
      }
      
      HttpWorkerRequest workerRequest = this.GetWorkerRequest(request);
      this.ExecuteWorkerRequest(workerRequest);
      return this.GetResponse(workerRequest);
    }

    /// <summary>
    /// Gets the worker request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns> The <see cref="HttpWorkerRequest"/>.</returns>
    protected virtual HttpWorkerRequest GetWorkerRequest(Request request)
    {
      return new WorkerRequest(this.InitializationManager, new RequestInitializationContext(request, new Response()), TextWriter.Null);
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    /// <param name="workerRequest">The worker request.</param>
    /// <returns>The <see cref="Response"/>.</returns>
    protected virtual Response GetResponse(HttpWorkerRequest workerRequest)
    {
      if (workerRequest == null)
      {
        throw new ArgumentNullException("workerRequest");
      }

      WorkerRequest workerRequestCandidate = workerRequest as WorkerRequest;

      if (workerRequestCandidate == null)
      {
        throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "workerRequest is of improper type. It should be based on {0}", typeof(WorkerRequest).FullName));
      }

      return workerRequestCandidate.Response;
    }

    /// <summary>
    /// Executes worker request.
    /// </summary>
    /// <param name="workerRequest">The worker request.</param>
    protected virtual void ExecuteWorkerRequest(HttpWorkerRequest workerRequest)
    {
      HttpRuntime.ProcessRequest(workerRequest);
    }
  }
}
