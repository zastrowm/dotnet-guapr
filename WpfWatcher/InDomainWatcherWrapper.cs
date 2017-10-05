using System;
using System.AddIn.Contract;
using System.AddIn.Pipeline;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using WpfHosting;

namespace WpfWatcher
{
  /// <summary>
  ///  A proxy for <see cref="HostedEntryPoint"/> that exists in the AppDomain where the proxy was
  ///  created.  Provides additional functionality for interacting with the given instance.
  /// </summary>
  public class InDomainWatcherWrapper : MarshalByRefObject
  {
    private readonly HostedEntryPoint _entryPoint;
    private FrameworkElement _element;

    /// <summary> Constructor. </summary>
    /// <param name="entryPoint"> The entry point to wrap. </param>
    public InDomainWatcherWrapper(HostedEntryPoint entryPoint)
    {
      _entryPoint = entryPoint;
    }

    /// <summary>
    ///  Initializes the given entry point by calling <see crefHostedEntryPointnt.Initialize"/>
    ///  returning the given framework as an INativeHandleContract.
    /// </summary>
    public INativeHandleContract Initialize(Dictionary<string, string> configuration,
                                            out Dictionary<string, string> returnedConfiguration)
    {
      _element = _entryPoint.Initialize(configuration);
      returnedConfiguration = configuration;
      return FrameworkElementAdapters.ViewToContractAdapter(_element);
    }

    /// <summary> Allows the entry point time to save any data that it wants to store. </summary>
    public Dictionary<string, string> Shutdown(Dictionary<string, string> configuration)
    {
      _entryPoint.Shutdown(_element, configuration);
      return configuration;
    }
  }
}