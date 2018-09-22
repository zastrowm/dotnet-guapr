using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Guapr.ClientHosting.Shared;

namespace Guapr.ClientHosting.Internal
{
  /// <summary>
  ///  Allows accessing various apis that are implemented via the ExtraUtils assembly.
  /// </summary>
  internal class ExtraUtils
  {
    private readonly Dictionary<string, Assembly> _loadedAssemblies = new Dictionary<string, Assembly>();
    private readonly Dictionary<Type, Type> _cachedTypeLookups = new Dictionary<Type, Type>();

    private readonly DynamicallyLoadedTypeName _easySerializationName;

    /// <summary> Constructor. </summary>
    public ExtraUtils(IClientInitializationData initializationData)
    {
      _easySerializationName = initializationData.EasySerializerTypeInfo;
    }

    /// <summary>
    ///  Creates the implementation of <see cref="IEasySerializer"/> from the ExtraUtils library.
    /// </summary>
    public IEasySerializer CreateEasySerializer(IStateDirectoryOwner stateOwner)
      => CreateInstance<IEasySerializer>(_easySerializationName, stateOwner);

    /// <summary> Creates an instance of the given dynamically loaded type. </summary>
    private T CreateInstance<T>(DynamicallyLoadedTypeName typeData, params object[] parameters)
    {
      if (!_cachedTypeLookups.TryGetValue(typeof(T), out var typeToLoad))
      {
        var fullyQualifiedName = $"{typeData.TypeName}, {typeData.AssemblyName}";
        typeToLoad = Type.GetType(fullyQualifiedName, throwOnError: true);
        _cachedTypeLookups[typeof(T)] = typeToLoad;
      }

      return (T)Activator.CreateInstance(typeToLoad, parameters, null);
    }
  }
}