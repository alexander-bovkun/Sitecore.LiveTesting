namespace Sitecore.LiveTesting.Requests
{
  using System;
  using System.Web;
  using System.Web.Hosting;

  /// <summary>
  /// The request manager.
  /// </summary>
  public class RequestManager : MarshalByRefObject, IRegisteredObject
  {
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
    /// Unregisters the object..
    /// </summary>
    /// <param name="immediate"><value>true</value> if register immediately, otherwise <value>false</value>.</param>
    void IRegisteredObject.Stop(bool immediate)
    {
      HostingEnvironment.UnregisterObject(this);
    }

    /// <summary>
    /// Gets the worker request.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns> The <see cref="HttpWorkerRequest"/>.</returns>
    protected virtual HttpWorkerRequest GetWorkerRequest(Request request)
    {
      return new WorkerRequest(request);
    }

    /// <summary>
    /// Gets the response.
    /// </summary>
    /// <param name="workerRequest">The worker request.</param>
    /// <returns>The <see cref="Response"/>.</returns>
    protected virtual Response GetResponse(HttpWorkerRequest workerRequest)
    {
      if (!(workerRequest is WorkerRequest))
      {
        throw new ArgumentException(string.Format("workerRequest is of improper type. It should be based on {0}", typeof(WorkerRequest).FullName));
      }

      WorkerRequest request = (WorkerRequest)workerRequest;
      Response result = new Response();

      if (request.ResponseText != null)
      {
        result.Content = request.ResponseText;
      }

      if (request.Context != null)
      {
        result.StatusCode = request.Context.Response.StatusCode;
      }

      return result;
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
