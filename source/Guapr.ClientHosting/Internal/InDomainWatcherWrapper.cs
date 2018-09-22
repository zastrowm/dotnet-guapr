using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Guapr.ClientHosting.Internal
{
  /// <summary>
  ///  A proxy for <see cref="IHostedEntryPoint"/> that exists in the AppDomain where the entry-
  ///  point was created.  Provides additional functionality for interacting with the given instance.
  /// </summary>
  [DomainUsage(DomainId.Client, CreatedBy = DomainId.Client)]
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
  }
}