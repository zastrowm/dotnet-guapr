using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace WpfWatcher
{
  /// <summary>
  ///  Retrieves the name of the assembly from the provided exe or dll, by loading it into an app
  ///  domain and checking the name.
  /// </summary>
  public class AssemblyNameHelper : MarshalByRefObject
  {
    private const string FriendlyTempDomainName = "$$$$_temp_getNameOfAssembly";

    /// <summary> Gets the name of the given assembly. </summary>
    public string GetAssemblyNameOf(string pathToExeOrDll)
    {
      if (AppDomain.CurrentDomain.FriendlyName == FriendlyTempDomainName)
      {
        var assembly = Assembly.ReflectionOnlyLoadFrom(pathToExeOrDll);
        return assembly.GetName().Name;
      }

      var domain = AppDomainUtils.CreateShadowAppDomain(FriendlyTempDomainName)
                                 .ResolveAssembliesFrom(Path.GetDirectoryName(pathToExeOrDll));

      var helper = domain.CreateInstanceOf<AssemblyNameHelper>();

      var name = helper.GetAssemblyNameOf(pathToExeOrDll);
      AppDomain.Unload(domain);

      return name;
    }
  }
}