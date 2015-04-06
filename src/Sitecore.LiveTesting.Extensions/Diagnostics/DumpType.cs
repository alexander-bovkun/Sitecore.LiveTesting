namespace Sitecore.LiveTesting.Extensions.Diagnostics
{
  using System;

  /// <summary>
  /// Defines the type of memory dump.
  /// </summary>
  [Flags]
  public enum DumpType
  {
    /// <summary>
    /// Include all accessible memory in the process.
    /// </summary>
    WithFullMemory = 0x00000002
  }
}
