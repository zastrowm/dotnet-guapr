using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  Base type for assemblies to implement (by using <see cref="HostedTargetTypeAttribute"/> to
  ///  designate the correct type to use) to create the control/window that will be shown in the
  ///  host.
  /// </summary>
  public interface IHostedEntryPoint
  {
    /// <summary> Initializes the watcher, creating the element to observe. </summary>
    /// <param name="startupApi"> Api that exposes functionality that can be used to facilitate an app
    ///  to it's pre-shutdown state. </param>
    /// <returns> A FrameworkElement that should be shown in the preview window. </returns>
    FrameworkElement Startup(IEntryPointStartupApi startupApi);

    /// <summary>
    ///  Invoked prior to the application-domain being unloaded.  Can be used to save data so that the
    ///  GUI can be restored to its current on reload.
    /// </summary>
    /// <param name="originalInstance"> The original instance that was returned from
    ///  <see cref="Startup" />. </param>
    /// <param name="shutdownApi"> Api that can be used to store data that will be made available the
    ///  next time that <see cref="Startup"/> is invoked on app reload. </param>
    void Shutdown(FrameworkElement originalInstance, IEntryPointShutdownApi shutdownApi);
  }
}