namespace Sitecore.LiveTesting.Extensions.InitializationHandlers.Request
{
  using System;
  using System.Web;
  using Sitecore.Diagnostics;
  using Sitecore.LiveTesting.Initialization;

  /// <summary>
  /// Defines initialization handler that exposes an instance of <see cref="HttpContext"/> from.
  /// </summary>
  public class HttpContextProvider : IInitializationContextAware, IDisposable
  {
    /// <summary>
    /// The context to restore.
    /// </summary>
    private readonly HttpContext contextToRestore;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpContextProvider"/> class.
    /// </summary>
    public HttpContextProvider()
    {
      this.contextToRestore = HttpContext.Current;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="HttpContextProvider"/> class. 
    /// </summary>
    ~HttpContextProvider()
    {
      this.Dispose(false);
    }

    /// <summary>
    /// The set initialization context.
    /// </summary>
    /// <param name="context">The context.</param>
    public void SetInitializationContext([NotNull] object context)
    {
      Assert.ArgumentNotNull(context, "context");

      RequestInitializationContext requestInitializationContext = context as RequestInitializationContext;

      if (requestInitializationContext == null)
      {
        throw new NotSupportedException(string.Format("Only contexts derived from '{0}' are supported.", typeof(RequestInitializationContext).FullName));
      }

      HttpContext.Current = ((RequestInitializationContext)context).HttpContext;
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
      if (this.disposed)
      {
        return;
      }

      if (disposing)
      {
        HttpContext.Current = this.contextToRestore;
      }

      this.disposed = true;
    }
  }
}
