namespace Sitecore.LiveTesting.Extensions.Initialization.Diagnostics
{
  using System;
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
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    public MemoryDumpOnException() : this(DefaultMemoryDumpFileName, DumpType.WithFullMemory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    public MemoryDumpOnException(string fileName) : this(fileName, DumpType.WithFullMemory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="dumpType">The dump type.</param>
    public MemoryDumpOnException(DumpType dumpType) : this(DefaultMemoryDumpFileName, dumpType)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryDumpOnException"/> class.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="dumpType">The dump type.</param>
    public MemoryDumpOnException(string fileName, DumpType dumpType)
    {
      if (fileName == null)
      {
        throw new ArgumentNullException("fileName");
      }

      this.fileName = fileName;
      this.dumpType = dumpType;
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
    /// On first chance exception event handler.
    /// </summary>
    /// <param name="eventArgs">The first chance exception event args.</param>
    protected override void OnFirstChanceException(FirstChanceExceptionEventArgs eventArgs)
    {
      DumpUtility.WriteDump(this.FileName, this.DumpType);
    }
  }
}
