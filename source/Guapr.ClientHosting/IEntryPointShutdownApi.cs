using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary> Interface for entry point session. </summary>
  public interface IEntryPointShutdownApi
  {
    /// <summary> Saves state that can be restored later via <see cref="IEntryPointStartupApi"/>. </summary>
    void SaveState<T>(string name, T state);

    /// <summary> A directory where additional (larger) state can be stored. </summary>
    DirectoryInfo StateDirectory { get; }
  }
}