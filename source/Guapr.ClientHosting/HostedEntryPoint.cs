using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  Generic implement ion of <see cref="IHostedEntryPoint"/> which allows for strongly-typed
  ///  controls and state.
  /// </summary>
  /// <typeparam name="TFrameworkElement"> The type of the framework element that this host
  ///  provides. </typeparam>
  /// <typeparam name="TState"> The session data type that can be used to restore the application
  ///  into it's former state. </typeparam>
  public abstract class HostedEntryPoint<TFrameworkElement, TState> : IHostedEntryPoint
    where TFrameworkElement : FrameworkElement
  {
    /// <inheritdoc />
    FrameworkElement IHostedEntryPoint.Startup(IEntryPointStartupInfo startupInfo)
    {
      if (!startupInfo.TryLoadState(out TState data))
        data = default(TState);

      return Startup(startupInfo, data);
    }

    /// <summary> Invoked on startup of the application. </summary>
    protected abstract TFrameworkElement Startup(IEntryPointStartupInfo startupInfo, TState data);

    void IHostedEntryPoint.Shutdown(FrameworkElement originalInstance, IEntryPointShutdownInfo shutdownInfo)
    {
      var data = Shutdown((TFrameworkElement)originalInstance, shutdownInfo);
      shutdownInfo.SaveState<TState>(data);
    }

    /// <summary> St. </summary>
    /// <param name="originalInstance"> The original instance. </param>
    /// <param name="shutdownInfo"> Information describing the shutdown. </param>
    /// <returns> A TState. </returns>
    protected abstract TState Shutdown(TFrameworkElement originalInstance, IEntryPointShutdownInfo shutdownInfo);
  }
}