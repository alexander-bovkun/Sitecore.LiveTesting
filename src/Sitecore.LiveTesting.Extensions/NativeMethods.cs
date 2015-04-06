namespace Sitecore.LiveTesting.Extensions
{
  using System;
  using System.Runtime.InteropServices;
  using Sitecore.LiveTesting.Extensions.Diagnostics;

  /// <summary>
  /// Defines the native methods for this assembly.
  /// </summary>
  internal static class NativeMethods
  {
    /// <summary>
    /// Get the current unmanaged thread id.
    /// </summary>
    /// <returns>The thread id.</returns>
    [DllImport("kernel32.dll")]
    internal static extern uint GetCurrentThreadId();

    /// <summary>
    /// Writes memory dump to the file.
    /// </summary>
    /// <param name="processHandle">The process Handle.</param>
    /// <param name="processId">The process Id.</param>
    /// <param name="fileHandle">The file Handle.</param>
    /// <param name="dumpType">The dump Type.</param>
    /// <param name="exception">The exception.</param>
    /// <param name="userStream">The user Stream.</param>
    /// <param name="callback">The callback.</param>
    /// <returns>The status of operation.</returns>
    [DllImport("DbgHelp.dll")]
    internal static extern bool MiniDumpWriteDump(IntPtr processHandle, int processId, SafeHandle fileHandle, DumpType dumpType, ref ExceptionInformation exception, IntPtr userStream, IntPtr callback);

    /// <summary>
    /// The exception information for MiniDumpWriteDump function.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1049:TypesThatOwnNativeResourcesShouldBeDisposable", Justification = "This struct has purely descriptive meaning and has no effect on resource allocation/deallocation")]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    internal struct ExceptionInformation
    {
      /// <summary>
      /// The thread id.
      /// </summary>
      internal uint ThreadId;

      /// <summary>
      /// The exception pointers.
      /// </summary>
      internal IntPtr ExceptionPointers;

      /// <summary>
      /// The client pointers.
      /// </summary>
      [MarshalAs(UnmanagedType.Bool)]
      private readonly bool clientPointers;
    }
  }
}
