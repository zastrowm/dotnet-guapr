using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Guapr.App
{
  /// <summary> Utility helper methods for interacting with AppDomains. </summary>
  public static class AppDomainUtils
  {
    /// <summary>
    ///  Creates an instance of the given type in the given app-domain.
    /// </summary>
    public static T CreateInstanceOf<T>(this AppDomain domain)
      where T : MarshalByRefObject
    {
      var instance = domain.CreateInstance(typeof(T).Assembly.FullName, typeof(T).FullName);
      return (T)instance.Unwrap();
    }

    /// <summary>
    ///  Creates an instance of the given type in the given app-domain.
    /// </summary>
    public static T CreateInstanceOf<T>(this AppDomain domain, params object[] parameters)
      where T : MarshalByRefObject
    {
      return
        (T)
        domain.CreateInstanceAndUnwrap(typeof(T).Assembly.FullName,
                                       typeof(T).FullName,
                                       false,
                                       BindingFlags.CreateInstance,
                                       null,
                                       parameters,
                                       null,
                                       null);
    }

    /// <summary>
    ///  Creates an appdomain that loads its assembly via shadow-copy.
    /// </summary>
    public static AppDomain CreateShadowAppDomain(string appDomainName)
    {
      var domain = AppDomain.CreateDomain(appDomainName,
                                          null,
                                          new AppDomainSetup()
                                          {
                                            ShadowCopyFiles = "true",
                                          });
      return domain;
    }

    /// <summary> Have the AppDomain load assemblies from the given directory. </summary>
    public static AppDomain ResolveAssembliesFrom(this AppDomain appDomain, string path)
    {
      appDomain.CreateInstanceOf<Loader>(path);
      return appDomain;
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
        var partialName = args.Name.Split(new []{','}, 2).First();

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