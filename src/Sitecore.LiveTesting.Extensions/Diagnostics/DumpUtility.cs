namespace Sitecore.LiveTesting.Extensions.Diagnostics
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Runtime.InteropServices;

  /// <summary>
  /// Defines utility methods to collect memory dumps.
  /// </summary>
  public static class DumpUtility
  {
    /// <summary>
    /// Writes the memory dump to the file.
    /// </summary>
    /// <param name="fileName">The file name.</param>
    /// <param name="dumpType">The dump type.</param>
    public static void WriteDump(string fileName, DumpType dumpType)
    {
      using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
      {
        Process currentProcess = Process.GetCurrentProcess();
        NativeMethods.ExceptionInformation exceptionInformation = new NativeMethods.ExceptionInformation { ThreadId = NativeMethods.GetCurrentThreadId(), ExceptionPointers = Marshal.GetExceptionPointers() };

        NativeMethods.MiniDumpWriteDump(currentProcess.Handle, currentProcess.Id, fileStream.SafeFileHandle, dumpType, ref exceptionInformation, IntPtr.Zero, IntPtr.Zero);
      }
    }
  }
}
