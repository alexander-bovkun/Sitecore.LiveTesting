namespace Sitecore.LiveTesting.Extensions.Initialization.Diagnostics
{
  using System;
  using System.Runtime.ExceptionServices;

  /// <summary>
  /// Defines the base class for initialization handlers that register themself for first chance exceptions.
  /// </summary>
  public abstract class FirstChanceException : IDisposable
  {
    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FirstChanceException"/> class.
    /// </summary>
    protected FirstChanceException()
    {
      AppDomain.CurrentDomain.FirstChanceException += this.FirstChanceExceptionHandler;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="FirstChanceException"/> class.
    /// </summary>
    ~FirstChanceException()
    {
      this.Dispose(false);
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
        AppDomain.CurrentDomain.FirstChanceException -= this.FirstChanceExceptionHandler;
      }

      this.disposed = true;
    }

    /// <summary>
    /// On first chance exception event handler.
    /// </summary>
    /// <param name="firstChanceExceptionEventArgs">The first chance exception event args.</param>
    protected abstract void OnFirstChanceException(FirstChanceExceptionEventArgs firstChanceExceptionEventArgs);

    /// <summary>
    /// Called when exception has just been thrown.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The first chance exception event args.</param>
    private void FirstChanceExceptionHandler(object sender, FirstChanceExceptionEventArgs e)
    {
      this.OnFirstChanceException(e);
    }
  }
}
