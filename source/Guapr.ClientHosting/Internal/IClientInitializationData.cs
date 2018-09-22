using System;
using System.Collections.Generic;
using System.Linq;
using Guapr.ClientHosting.Shared;

namespace Guapr.ClientHosting.Internal
{
  /// <summary> All data required to perform initialization. </summary>
  internal interface IClientInitializationData
  {
    /// <summary> The assembly that is being loaded. </summary>
    string TargetAssembly { get; }

    /// <summary> The directory where the client assembly being loaded is located. </summary>
    string ClientAssemblyDirectory { get; }

    /// <summary> The directory where additional assemblies can be loaded from. </summary>
    string ExtraAssembliesDirectory { get; }

    /// <summary> The type name of the <see cref="IEasySerializer"/> can be loaded from. </summary>
    DynamicallyLoadedTypeName EasySerializerTypeInfo { get; }
  }
}