#define DEBUG

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Guapr.ClientHosting.Internal
{
  /// <summary> A class whose implementation calls methods that are Conditional("DEBUG")'d out </summary>
  internal static class AlwaysDebug
  {
    /// <summary> Calls <see cref="Debug.WriteLine(string)"/> </summary>
    public static void DebugWriteLine(string message) 
      => Debug.WriteLine(message);
  }
}

