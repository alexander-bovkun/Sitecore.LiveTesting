namespace Sitecore.LiveTesting.Extensions.Initialization.Diagnostics
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Runtime.ExceptionServices;
  using Sitecore.LiveTesting.Extensions.Diagnostics;

  /// <summary>
  /// Defines initialization handler that makes memory dump on first chance exception.
  /// </summary>
  public class MemoryDumpOnException : FirstChanceException
  {
    /// <summary>
    /// The default memory dump file name.
    /// </summary>
    private const string DefaultMemoryDumpFileName = "MemoryDump.dmp";

    /// <summary>
    /// The file name.
    /// </summary>
    private readonly string fileName;

    /// <summary>
    /// The dump type.
    /// </summary>
    private readonly DumpType dumpType;

    /// <summary>
    /// The exceptions to track.
    /// </summary>
    private readonly IEnumerable<Type> exceptionsToTrack;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    public MemoryDumpOnException() : this(new Type[0])
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="exceptionsToTrack">The exceptions to track.</param>
    public MemoryDumpOnException(IEnumerable<Type> exceptionsToTrack) : this(DefaultMemoryDumpFileName, exceptionsToTrack)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="exceptionsToTrack">The exceptions to track.</param>
    public MemoryDumpOnException(string fileName, IEnumerable<Type> exceptionsToTrack) : this(fileName, DumpType.WithFullMemory, exceptionsToTrack)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="dumpType">The dump type.</param>
    /// <param name="exceptionsToTrack">The exceptions to track.</param>
    public MemoryDumpOnException(string fileName, DumpType dumpType, IEnumerable<Type> exceptionsToTrack)
    {
      if (fileName == null)
      {
        throw new ArgumentNullException("fileName");
      }

      if (exceptionsToTrack == null)
      {
        throw new ArgumentNullException("exceptionsToTrack");
      }

      this.fileName = fileName;
      this.dumpType = dumpType;
      this.exceptionsToTrack = exceptionsToTrack;
    }

    /// <summary>
    /// Gets the file name.
    /// </summary>
    protected string FileName
    {
      get { return this.fileName; }
    }

    /// <summary>
    /// Gets the dump type.
    /// </summary>
    protected DumpType DumpType
    {
      get { return this.dumpType; }
    }

    /// <summary>
    /// Gets the exceptions to track.
    /// </summary>
    protected IEnumerable<Type> ExceptionsToTrack
    {
      get { return this.exceptionsToTrack; }
    }

    /// <summary>
    /// On first chance exception event handler.
    /// </summary>
    /// <param name="eventArgs">The first chance exception event args.</param>
    protected override void OnFirstChanceException(FirstChanceExceptionEventArgs eventArgs)
    {
      if (this.exceptionsToTrack.Any() && (!this.exceptionsToTrack.Contains(eventArgs.Exception.GetType())))
      {
        return;
      }

      DumpUtility.WriteDump(this.FileName, this.DumpType);
    }
  }
}
