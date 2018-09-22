using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Guapr.ClientHosting.Internal
{
  /// <summary> Client implementation of the startup/shutdown apis. </summary>
  internal class StartupAndShutdownApi : IEntryPointStartupApi,
                                         IEntryPointShutdownApi
  {
    public StartupAndShutdownApi(DirectoryInfo stateDirectory)
    {
      StateDirectory = stateDirectory;
    }

    /// <inheritdoc cref="IStateDirectoryOwner.StateDirectory" />
    public DirectoryInfo StateDirectory { get; }

    /// <inheritdoc />
    public event EventHandler FocusGranted;

    /// <summary> Invokes the <see cref="FocusGranted"/> event. </summary>
    public void FireFocusGranted(object sender, EventArgs args)
      => FocusGranted?.Invoke(sender, args);
  }
}