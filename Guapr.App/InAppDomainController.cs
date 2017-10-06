using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guapr.ClientHosting;

namespace Guapr.App
{
  /// <summary>
  ///  Api that is created in the "Reload" domain and allows the "Host" app-domain to interact with
  ///  objects in the AppDomain.
  /// </summary>
  public class InAppDomainController : MarshalByRefObject
  {
    /// <summary>
    ///  Creates a Framework element with the given assembly and the given type name.
    /// </summary>
    public InDomainWatcherWrapper FindEntryPoint(string assemblyName)
    {
      var assembly = Assembly.Load(assemblyName);
      var attribute = assembly.GetCustomAttribute<HostedTargetTypeAttribute>();
      var watcherType = attribute?.TargetType;

      if (watcherType == null)
        throw new InvalidOperationException(
          $"No valid {nameof(HostedTargetTypeAttribute)} found for assembly «{assembly}».");

      var instance = (IHostedEntryPoint)Activator.CreateInstance(watcherType);

      return new InDomainWatcherWrapper(instance);
    }
  }
}