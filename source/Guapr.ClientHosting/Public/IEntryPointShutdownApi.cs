using System;
using System.Collections.Generic;
using System.Linq;

namespace Guapr.ClientHosting
{
  /// <summary>
  ///  API for functionality that can be invoked when the entry-point is being shutdown.
  /// </summary>
  public interface IEntryPointShutdownApi : IStateDirectoryOwner
  {
   
  }
}