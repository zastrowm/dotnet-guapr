using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Guapr.ClientHosting
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
    /// <param name="displayName"> (Optional) A display name for the given entry point. </param>
    public HostedTargetTypeAttribute(Type entryPoint, string displayName = null)
    {
      TargetType = entryPoint;
      DisplayName = displayName ?? "default";
    }

    /// <summary> The type that should be created in order to configure the watcher. </summary>
    public Type TargetType { get; }

    /// <summary> How the entry point should be presented to the user. </summary>
    public string DisplayName { get; }
  }
}