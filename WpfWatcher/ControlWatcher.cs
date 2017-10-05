using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfWatcher
{
  /// <summary>
  ///  Takes an WPF-control assembly and loads it, automatically refreshing the control/app-domain
  ///  when the assembly changes.
  /// </summary>
  public class ControlWatcher : ContentPresenter,
                                IDisposable
  {
    private readonly AssemblyConfiguration _configuration;
    private readonly AppDomainedControlHost _host;
    private readonly MessageControl _waitingControl;

    public ControlWatcher(AssemblyConfiguration configuration)
    {
      _configuration = configuration;
      _waitingControl = new MessageControl();
      Content = _waitingControl;

      _host = new AppDomainedControlHost(_configuration);
      _host.StatusChanged += HandleHostChanged;

      Loaded += delegate
                {
                  _host.Reload();
                };
    }

    public Window Window
      => Window.GetWindow(this);

    private void HandleHostChanged(object sender, ControlHostStatus e)
    {
      switch (_host.Status)
      {
        case ControlHostStatus.Valid:
          Content = _host.Host;
          Window.Title = $"WPF Watcher - Running {Path.GetFileName(_configuration.PathToAssembly)}";
          break;
        case ControlHostStatus.Loading:
          Content = _waitingControl;
          _waitingControl.Message = "Loading assemblies...";
          Window.Title = $"WPF Watcher - Loading {Path.GetFileName(_configuration.PathToAssembly)}";
          break;
        case ControlHostStatus.Waiting:
          Content = _waitingControl;
          _waitingControl.Message = "Waiting for valid assemblies...";
          Window.Title = $"WPF Watcher - Waiting for {Path.GetFileName(_configuration.PathToAssembly)}";
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      _waitingControl.AdditionalInformation = _host.AdditionalInformation;
    }

    public void Dispose()
    {
      _host.StatusChanged -= HandleHostChanged;
      _host.Dispose();
    }
  }
}