namespace Sitecore.LiveTesting.IIS.Tests
{
  using System;
  using System.Threading;

  /// <summary>
  /// Defines initialization handler for which makes test execution sequential.
  /// </summary>
  public sealed class SequentialInitializationHandler : IDisposable
  {
    /// <summary>
    /// The mutex name.
    /// </summary>
    private const string MutexName = "Sitecore.LiveTesting.IIS.Tests";

    /// <summary>
    /// The mutex.
    /// </summary>
    private readonly Mutex mutex;

    /// <summary>
    /// Initializes a new instance of the <see cref="SequentialInitializationHandler"/> class.
    /// </summary>
    public SequentialInitializationHandler()
    {
      this.mutex = new Mutex(false, MutexName);
      this.mutex.WaitOne();
    }

    /// <summary>
    /// Disposes instance of the class.
    /// </summary>
    public void Dispose()
    {
      this.mutex.Dispose();
    }
  }
}
