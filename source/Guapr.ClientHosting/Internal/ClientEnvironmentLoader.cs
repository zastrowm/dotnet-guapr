using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Guapr.ClientHosting.Internal
{
  /// <summary>
  ///  Prepares the client-environment for the hosting domain.
  ///  
  ///  Created in the Client Domain by the Server Domain.
  /// </summary>
  [OwnedByDomain(DomainAttribute.Client, CreatedBy = DomainAttribute.Server)]
  public class ClientEnvironmentLoader : MarshalByRefObject
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