using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary> Interface for entry point session. </summary>
  public interface IEntryPointShutdownInfo
  {
    /// <summary> Saves state that can be restored later via <see cref="IEntryPointStartupInfo"/>. </summary>
    void SaveState<T>(T state);

    /// <summary> A directory where additional (larger) state can be stored. </summary>
    DirectoryInfo StateDirectory { get; }
  }
}