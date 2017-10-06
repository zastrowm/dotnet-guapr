using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Guapr.ClientHosting;
using Newtonsoft.Json;

namespace Guapr.App
{
  /// <summary>
  ///  A proxy for <see cref="IHostedEntryPoint"/> that exists in the AppDomain where the proxy was
  ///  created.  Provides additional functionality for interacting with the given instance.
  /// </summary>
  public class InDomainWatcherWrapper : MarshalByRefObject
  {
    private readonly IHostedEntryPoint _entryPoint;
    private FrameworkElement _element;
    private StartupAndShutdownApi _startupApi;

    /// <summary> Constructor. </summary>
    /// <param name="entryPoint"> The entry point to wrap. </param>
    public InDomainWatcherWrapper(IHostedEntryPoint entryPoint)
    {
      _entryPoint = entryPoint;
    }

    /// <summary>
    ///  Initializes the given entry point by calling <see cref="IHostedEntryPoint.Startup"/>
    ///  returning the given framework as an INativeHandleContract.
    /// </summary>
    public INativeHandleContract Initialize(string directoryPath)
    {
      _startupApi = new StartupAndShutdownApi(new DirectoryInfo(directoryPath));
      _element = _entryPoint.Startup(_startupApi);
      return FrameworkElementAdapters.ViewToContractAdapter(_element);
    }

    /// <summary> Invoked when the host has focused the proxy element. </summary>
    public void NotifyFocused()
      => _startupApi.FireFocusGranted(_element, EventArgs.Empty);

    /// <summary> Allows the entry point time to save any data that it wants to store. </summary>
    public void Shutdown()
    {
      _entryPoint.Shutdown(_element, _startupApi);
    }

    /// <summary> Implementation of the startup/shutdown infos </summary>
    private class StartupAndShutdownApi : IEntryPointStartupApi,
                                           IEntryPointShutdownApi
    {
      public StartupAndShutdownApi(DirectoryInfo stateDirectory)
      {
        StateDirectory = stateDirectory;
      }

      /// <inheritdoc />
      public DirectoryInfo StateDirectory { get; }

      /// <inheritdoc />
      public event EventHandler FocusGranted;

      /// <summary> Invokes the <see cref="FocusGranted"/> event. </summary>
      public void FireFocusGranted(object sender, EventArgs args)
        => FocusGranted?.Invoke(sender, args);

      /// <inheritdoc />
      bool IEntryPointStartupApi.TryLoadState<T>(string name, out T state)
      {
        name = GetSafeName(name);

        try
        {
          var file = GetSessionFile(name);
          if (!file.Exists)
          {
            state = default(T);
            return false;
          }

          using (var reader = new StreamReader(file.Open(FileMode.Open, FileAccess.Read)))
          using (var jsonReader = new JsonTextReader(reader))
          {
            var serializer = new JsonSerializer();
            state = serializer.Deserialize<T>(jsonReader);
            return true;
          }
        }
        catch
        {
          state = default(T);
          return false;
        }
      }

      /// <inheritdoc />
      void IEntryPointShutdownApi.SaveState<T>(string name, T state)
      {
        name = GetSafeName(name);

        try
        {
          var file = GetSessionFile(name);
          var serializer = new JsonSerializer();

          using (var writer = new StreamWriter(file.Open(FileMode.Create)))
          {
            serializer.Serialize(writer, state);
          }
        }
        catch
        {
        }
      }

      private string GetSafeName(string name)
      {
        if (name == null)
          return "";

        return "." + Convert.ToBase64String(Encoding.UTF8.GetBytes(name));
      }

      private FileInfo GetSessionFile(string name)
        => new FileInfo(Path.Combine(StateDirectory.FullName, $".__session.state{name}__"));
    }
  }
}