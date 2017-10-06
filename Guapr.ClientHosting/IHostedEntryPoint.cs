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
    /// <param name="startupInfo"> Any startup data/options that should be used to create the given element. </param>
    /// <returns> A FrameworkElement that should be shown in the preview window. </returns>
    FrameworkElement Startup(IEntryPointStartupInfo startupInfo);

    /// <summary>
    ///  Invoked prior to the application-domain being unloaded.  Can be used to store data in the
    ///  values dictionary.
    /// </summary>
    /// <param name="originalInstance"> The original instance that was returned from
    ///  <see cref="Startup" />. </param>
    /// <param name="shutdownInfo"> Session object that allows serializing any data. </param>
    void Shutdown(FrameworkElement originalInstance, IEntryPointShutdownInfo shutdownInfo);
  }
}