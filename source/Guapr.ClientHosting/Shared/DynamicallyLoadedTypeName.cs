using System;
using System.Collections.Generic;
using System.Linq;

namespace Guapr.ClientHosting.Shared
{
  /// <summary> Information about a specific type to load. </summary>
  [Serializable]
  internal class DynamicallyLoadedTypeName
  {
    public DynamicallyLoadedTypeName()
    {
      
    }

    public DynamicallyLoadedTypeName(Type type)
    {
      AssemblyName = type.Assembly.FullName;
      TypeName = type.FullName;
    }

    /// <summary> The assembly of the type to load. </summary>
    public string AssemblyName { get; set; }

    /// <summary> The name of the type to load. </summary>
    public string TypeName { get; set; }
  }
}