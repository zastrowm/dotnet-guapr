using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  Generic implementation of <see cref="IHostedEntryPoint"/> which allows for strongly-typed
  ///  controls and state.
  /// </summary>
  /// <typeparam name="TFrameworkElement"> The type of the framework element that this host
  ///  provides. </typeparam>
  /// <typeparam name="TState"> The session data type that can be used to restore the application
  ///  into it's former state. </typeparam>
  public abstract class HostedEntryPoint<TFrameworkElement, TState> : IHostedEntryPoint
    where TFrameworkElement : FrameworkElement
  {
    /// <summary> Invoked on startup of the application. </summary>
    /// <param name="startupApi"> Api that exposes functionality that can be used to facilitate an app
    ///  to it's pre-shutdown state. </param>
    /// <param name="data"> The data that was restored from the last session. </param>
    /// <returns> A Control that should be shown in the preview window. </returns>
    protected abstract TFrameworkElement Startup(IEntryPointStartupApi startupApi, TState data);

    /// <summary>
    ///  Invoked prior to the application-domain being unloaded.  Can be used to save data so that the
    ///  GUI can be restored to its current on reload.
    /// </summary>
    /// <param name="gui"> The original instance that was returned from <see cref="Startup" />. </param>
    /// <param name="shutdownApi"> Api that can be used to store data that will be made available the
    ///  next time that <see cref="Startup"/> is invoked on app reload. </param>
    /// <returns> The state that should be saved and restored on next startup. </returns>
    protected abstract TState Shutdown(TFrameworkElement gui, IEntryPointShutdownApi shutdownApi);

    /// <inheritdoc />
    FrameworkElement IHostedEntryPoint.Startup(IEntryPointStartupApi startupApi)
    {
      if (!startupApi.TryLoadState(null, out TState data))
        data = default(TState);

      return Startup(startupApi, data);
    }

    /// <inheritdoc />
    void IHostedEntryPoint.Shutdown(FrameworkElement originalInstance, IEntryPointShutdownApi shutdownApi)
    {
      var data = Shutdown((TFrameworkElement)originalInstance, shutdownApi);
      shutdownApi.SaveState<TState>(null, data);
    }
  }
}