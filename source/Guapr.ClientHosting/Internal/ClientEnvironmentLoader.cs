using System;
using System.Collections.Generic;
using System.IO;
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
  [DomainUsage(DomainId.Client, CreatedBy = DomainId.Server)]
  internal class ClientHostingBootstrapper : MarshalByRefObject
  {
    /// <summary>
    ///  The initialization data passed-in when the environment was loaded via <see cref="Initialize"/>.
    /// </summary>
    public static IClientInitializationData InitializationData { get; private set; }

    public InDomainWatcherWrapper FindEntryPoint(IClientInitializationData data)
    {
      var environmentBootstrapper = new ClientEnvironmentLoader(data);
      return environmentBootstrapper.FindEntryPoint();
    }

    internal class ClientEnvironmentLoader : MarshalByRefObject
    {
      /// <summary> Constructor. </summary>
      /// <param name="initData"> Information required for initialization. </param>
      public ClientEnvironmentLoader(IClientInitializationData initData)
      {
        InitializationData = initData;
        EntryPointUtilities.Implementation = new ExtraUtils(initData);

        new Loader(initData.ClientAssemblyDirectory);
        new Loader(initData.ExtraAssembliesDirectory);
      }

      /// <summary>
      ///  Creates a Framework element with the given assembly and the given type name.
      /// </summary>
      internal InDomainWatcherWrapper FindEntryPoint()
      {
        var assembly = Assembly.Load(InitializationData.TargetAssembly);
        var attribute = assembly.GetCustomAttribute<HostedTargetTypeAttribute>();
        var watcherType = attribute?.TargetType;

        if (watcherType == null)
          throw new InvalidOperationException(
            $"No valid {nameof(HostedTargetTypeAttribute)} found for assembly «{assembly}».");

        var instance = (IHostedEntryPoint)Activator.CreateInstance(watcherType);

        return new InDomainWatcherWrapper(instance);
      }

      /// <summary> Resolves assemblies by loading them out of a specific directory. </summary>
      internal class Loader : MarshalByRefObject
      {
        private readonly string _directory;

        public Loader(string directory)
        {
          _directory = directory;
          AppDomain.CurrentDomain.AssemblyResolve += ResolveAssembly;
        }

        private Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
          var partialName = args.Name.Split(new[] { ',' }, 2).First();

          var fullPath = GetFullPath(partialName);
          if (fullPath == null)
            return null;

          return Assembly.LoadFrom(fullPath);
        }

        private string GetFullPath(string assemblyName)
        {
          var fullPath = Path.Combine(_directory, assemblyName + ".dll");
          if (File.Exists(fullPath))
            return fullPath;

          fullPath = Path.Combine(_directory, assemblyName + ".exe");
          if (File.Exists(fullPath))
            return fullPath;

          return null;
        }
      }
    }
  }

}