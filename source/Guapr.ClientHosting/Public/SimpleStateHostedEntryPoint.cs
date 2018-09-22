using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  Provides an implementation of <see cref="IHostedEntryPoint"/> which alleviates a lot of the
  ///  work required to save and restore application state.
  /// </summary>
  /// <typeparam name="TFrameworkElement"> The type of control that should be shown in the server
  ///  host window. </typeparam>
  /// <typeparam name="TState"> The type of state to persist/load. </typeparam>
  public abstract class SimpleStateHostedEntryPoint<TFrameworkElement, TState> : IHostedEntryPoint
    where TFrameworkElement : FrameworkElement
  {
    private IEasySerializer _easySerializer;
    private IStateDirectoryOwner _stateDirectoryOwner
       ;

    /// <summary>
    ///  The control that was created via <see cref="CreateControl"/>. 
    ///  
    ///  Not valid until <see cref="CreateControl"/> is invoked.
    /// </summary>
    public TFrameworkElement Control { get; private set; }

    /// <summary>
    ///  The current state of the application/control.
    ///  
    ///  This property is populated when the application is started in one of two ways:
    ///    1. By deserializing the state from the last session  
    ///    2. By calling <see cref="CreateDefaultState"/> if the application wasn't loaded or if (1)
    ///    failed.
    /// </summary>
    public TState CurrentState { get; set; }

    /// <summary>
    ///  The directory where additional state (in addition to <see cref="CurrentState"/>) can be
    ///  stored.
    ///  
    ///  Not valid in the constructor or in the call to <see cref="CreateDefaultState"/>.
    /// </summary>
    public DirectoryInfo StateDirectory
      => _stateDirectoryOwner.StateDirectory;

    /// <summary> Creates the control that should be shown by the server. </summary>
    public abstract TFrameworkElement CreateControl();

    /// <summary>
    ///  Creates the state that should be used if the application had never been created or if the
    ///  last session loading failed.
    /// </summary>
    /// <returns> The new default state. </returns>
    public abstract TState CreateDefaultState();

    /// <summary>
    ///  Gets the state to persist and load on next application load.  By default, returns
    ///  <see cref="CurrentState"/>.
    /// </summary>
    public virtual TState GetStateToPersist()
      => CurrentState;

    /// <summary>
    ///  Invoked when the control should be focused.  Only called once.
    ///  
    ///  Invoked when <see cref="IEntryPointStartupApi.FocusGranted"/> is fired.
    /// </summary>
    public virtual void OnFocusReady()
      => Control.Focus();

    /// <inheritdoc />
    public FrameworkElement Startup(IEntryPointStartupApi startupApi)
    {
      _stateDirectoryOwner = startupApi;
      _easySerializer = startupApi.CreateSimpleSerializer();

      startupApi.FocusGranted +=
        (_1, _2) => EntryPointUtilities.SafeExecute("Attempting to focus Control", OnFocusReady);

      if (!_easySerializer.LoadState(null, out TState state))
      {
        try
        {
          state = CreateDefaultState();
        }
        catch (Exception e)
        {
          EntryPointUtilities.LogException("Creating default state", e);
        }
      }

      CurrentState = state;
      Control = CreateControl();
      return Control;
    }

    /// <inheritdoc />
    public void Shutdown(FrameworkElement originalInstance, IEntryPointShutdownApi shutdownApi)
    {
      EntryPointUtilities.SafeExecute("Saving state",
                                      () =>
                                      {
                                        var state = GetStateToPersist();
                                        _easySerializer.SaveState(null, state);
                                      });
    }
  }
}