using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary> Arguments for entry point. </summary>
  public interface IEntryPointStartupInfo
  {
    /// <summary> Attempts to load state of the given type. </summary>
    bool TryLoadState<T>(out T state);

    /// <summary> A directory where additional (larger) state can be stored. </summary>
    DirectoryInfo StateDirectory{ get; }
  }
}