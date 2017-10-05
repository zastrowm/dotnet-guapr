using System;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace WpfWatcher
{
  internal enum ControlHostStatus
  {
    Valid,
    Loading,
    Waiting,
  }

  /// <summary>
  ///  Contains the implementation of a loading a specific control from another app domain.
  /// </summary>
  internal class AppDomainedControlHost : IDisposable
  {
    private readonly AssemblyConfiguration _configuration;
    private readonly string _rootDictionary;
    private readonly string _assemblyToObserve;

    private AppDomain _lastDomain;
    private readonly DispatcherTimer _timer;
    private InDomainWatcherWrapper _currentEntryPoint;

    private readonly FileSystemWatcher _watcher;

    public AppDomainedControlHost(AssemblyConfiguration configuration)
    {
      _configuration = configuration;

      _rootDictionary = Path.GetDirectoryName(_configuration.PathToAssembly);
      _assemblyToObserve = new AssemblyNameHelper().GetAssemblyNameOf(_configuration.PathToAssembly);

      _watcher = new FileSystemWatcher
                 {
                   Path = _rootDictionary,
                   IncludeSubdirectories = false,
                   Filter = Path.GetFileName(_configuration.PathToAssembly)
                 };

      var dispatcher = Dispatcher.CurrentDispatcher;

      FileSystemEventHandler callback = delegate { dispatcher.InvokeAsync(() => _timer.Start()); };

      _watcher.Created += callback;
      _watcher.Changed += callback;
      _watcher.Deleted += callback;

      _watcher.EnableRaisingEvents = true;

      _timer = new DispatcherTimer
               {
                 Interval = TimeSpan.FromSeconds(1),
                 IsEnabled = false
               };

      _timer.Tick += delegate
                     {
                       _timer.Stop();
                       Reload();
                     };
    }

    /// <summary> The control that will actually contain the loaded control. </summary>
    public FrameworkElement Host { get; private set; }

    /// <summary> The current status of the App-Domain control. </summary>
    public ControlHostStatus Status
    {
      get { return _status; }
      set
      {
        _status = value;
        StatusChanged?.Invoke(this, _status);
      }
    }

    private ControlHostStatus _status;

    /// <summary> Invoked when the status changes. </summary>
    public event EventHandler<ControlHostStatus> StatusChanged;

    public string AdditionalInformation { get; set; }

    /// <summary>
    ///  Tear down any app domain that currently exists, create a new one and reload the control in
    ///  the given app domain.
    /// </summary>
    public async void Reload()
    {
      UnloadDomain();

      Status = ControlHostStatus.Waiting;

      if (!File.Exists(_configuration.PathToAssembly))
      {
        // try again in a little bit
        _timer.Start();
        return;
      }

      AdditionalInformation = null;
      Status = ControlHostStatus.Loading;

      try
      {
        // creating an app-domain (and loading everything etc.) takes a while, so do it on a background
        // thread and resume later. 
        await Task.Run(() =>
                       {
                         _lastDomain = AppDomainUtils.CreateShadowAppDomain("Document-Host")
                                                     .ResolveAssembliesFrom(_rootDictionary);

                         _currentEntryPoint = _lastDomain.CreateInstanceOf<InAppDomainController>()
                                                         .FindEntryPoint(_assemblyToObserve);
                       });

        // dictionaries are serialized (read: copied) instead of passed by reference across app-domains
        Dictionary<string, string> tempConfiguration;
        var reference = _currentEntryPoint.Initialize(_configuration.Data, out tempConfiguration);
        _configuration.Data = tempConfiguration;

        var proxyElement = FrameworkElementAdapters.ContractToViewAdapter(reference);

        Host = proxyElement;
        proxyElement.Loaded += delegate
                               {
                                 Keyboard.Focus(proxyElement);
                                 TraversalRequest request = new TraversalRequest(FocusNavigationDirection.Next);
                                 Host.MoveFocus(request);
                               };

        Status = ControlHostStatus.Valid;
      }
      catch (Exception exception)
      {
        AdditionalInformation = exception.ToString();
        UnloadDomain(ignoreShutdown: true);

        Status = ControlHostStatus.Waiting;
      }
    }

    /// <summary> Shuts down the current entry point. </summary>
    private void UnloadDomain(bool ignoreShutdown = false)
    {
      if (_lastDomain == null)
        return;

      if (!ignoreShutdown && _currentEntryPoint != null)
      {
        try
        {
          _configuration.Data = _currentEntryPoint.Shutdown(_configuration.Data);
          _currentEntryPoint = null;
        }
        catch (Exception)
        {
        }
      }

      Host = null;
      AppDomain.Unload(_lastDomain);
      _lastDomain = null;
    }

    /// <inheritdoc/>
    public void Dispose()
    {
      _watcher.EnableRaisingEvents = false;
      _timer.Stop();

      UnloadDomain();
    }
  }
}