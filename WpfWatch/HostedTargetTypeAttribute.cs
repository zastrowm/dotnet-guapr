using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WpfHosting
{
  /// <summary>
  ///  An attribute that designated the entry point class to construct an instance of in order to
  ///  allow the host to create an instance of a controls that is hosted in the host.
  /// </summary>
  [AttributeUsage(AttributeTargets.Assembly)]
  public class HostedTargetTypeAttribute : Attribute
  {
    /// <summary> Constructor. </summary>
    /// <param name="entryPoint"> The entry point. </param>
    public HostedTargetTypeAttribute(Type entryPoint)
    {
      TargetType = entryPoint;
    }

    /// <summary> The type that should be created in order to configure the watcher. </summary>
    public Type TargetType { get; private set; }
  }
}