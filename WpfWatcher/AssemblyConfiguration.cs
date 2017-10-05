using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfWatcher
{
  /// <summary> A configuration associated with a specific assembly. </summary>
  public class AssemblyConfiguration
  {
    public string PathToAssembly { get; set; }

    public Dictionary<string, string> Data { get; set; }

    /// <summary> Default constructor. </summary>
    public AssemblyConfiguration()
    {
      Data = new Dictionary<string, string>();
    }
  }
}