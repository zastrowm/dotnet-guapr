using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  API for functionality that can be invoked when the entry-point is being started up.
  /// </summary>
  public interface IEntryPointStartupApi : IStateDirectoryOwner
  {
    /// <summary>
    ///  Invoked when focus has been granted to the control. This event is only fired once, and can be
    ///  used for reassigning focus to the last control that was focused when the app was last
    ///  unloaded.
    /// </summary>
    event EventHandler FocusGranted;
  }
}