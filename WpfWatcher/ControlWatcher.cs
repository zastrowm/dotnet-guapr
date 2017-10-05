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

      _host.Reload();
    }

    public static readonly DependencyProperty StatusProperty = DependencyProperty.Register(
      "Status",
      typeof(string),
      typeof(ControlWatcher),
      new PropertyMetadata(default(string)));

    public string Status
    {
      get { return (string)GetValue(StatusProperty); }
      set { SetValue(StatusProperty, value); }
    }

    private void HandleHostChanged(object sender, ControlHostStatus e)
    {
      var assemblyName = Path.GetFileName(_configuration.PathToAssembly);

      switch (_host.Status)
      {
        case ControlHostStatus.Valid:
          Content = _host.Host;
          Status = $"Running «{assemblyName}»";
          break;
        case ControlHostStatus.Loading:
          Content = _waitingControl;
          _waitingControl.Message = "Loading assemblies...";
          Status = $"Loading «{assemblyName}»";
          break;
        case ControlHostStatus.Waiting:
          Content = _waitingControl;
          _waitingControl.Message = "Waiting for valid assemblies...";
          Status = $"Waiting for valid assemblies from «{assemblyName}»";
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