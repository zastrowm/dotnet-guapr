using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  An object which provides a directory to which data can be written/read from during setup or
  ///  teardown.
  /// </summary>
  public interface IStateDirectoryOwner
  {
    /// <summary>
    ///  A directory where files can be read/written to store state during startup and shutdown.
    /// </summary>
    DirectoryInfo StateDirectory { get; }
  }
}