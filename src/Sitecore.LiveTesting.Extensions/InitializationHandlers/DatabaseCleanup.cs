namespace Sitecore.LiveTesting.Extensions.InitializationHandlers
{
  using System;
  using System.Transactions;
  using Sitecore.Configuration;
  using Sitecore.Data;

  /// <summary>
  /// Defines the transaction initialization handler.
  /// </summary>
  public class TransactionInitializationHandler : IDisposable
  {
    /// <summary>
    /// The transaction scope
    /// </summary>
    private readonly TransactionScope transactionScope;

    /// <summary>
    /// The disposed.
    /// </summary>
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TransactionInitializationHandler"/> class.
    /// </summary>
    public TransactionInitializationHandler()
    {
      foreach (Database database in Factory.GetDatabases())
      {
        database.Caches.DataCache.Clear();
      }

      this.transactionScope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 0, 10, 0));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="TransactionInitializationHandler"/> class.
    /// </summary>
    ~TransactionInitializationHandler()
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
        this.transactionScope.Dispose();
      }

      this.disposed = true;
    }
  }
}
