using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary> Arguments for entry point. </summary>
  public interface IEntryPointStartupApi
  {
    /// <summary> Attempts to load state of the given type. </summary>
    bool TryLoadState<T>(string name, out T state);

    /// <summary> A directory where additional (larger) state can be stored. </summary>
    DirectoryInfo StateDirectory{ get; }

    /// <summary>
    ///  Invoked when focus has been granted to the control. This event is only fired once, and can be
    ///  used for reassigning focus to the last control that was focused when the app was last
    ///  unloaded.
    /// </summary>
    event EventHandler FocusGranted;
  }
}