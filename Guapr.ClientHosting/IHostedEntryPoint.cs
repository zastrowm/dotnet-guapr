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
    /// <param name="values"> Any startup data/options that should be used to create the given element. </param>
    /// <returns> A FrameworkElement that should be shown in the preview window. </returns>
    FrameworkElement Initialize(Dictionary<string, string> values);

    /// <summary>
    ///  Invoked prior to the application-domain being unloaded.  Can be used to store data in the
    ///  values dictionary.
    /// </summary>
    /// <param name="originalInstance"> The original instance that was returned from
    ///  <see cref="IHostedEntryPoint.Initialize(Dictionary{String,String})" />. </param>
    /// <param name="values"> Any startup data/options that should be persisted so that they can later
    ///  be used in <see cref="IHostedEntryPoint.Initialize(Dictionary{String,String})" />
    ///  the next time the assembly is loaded. </param>
    void Shutdown(FrameworkElement originalInstance, Dictionary<string, string> values);
  }
}