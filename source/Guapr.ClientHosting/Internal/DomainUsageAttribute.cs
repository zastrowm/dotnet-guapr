using System;
using System.Collections.Generic;
using System.Linq;

namespace Guapr.ClientHosting.Internal
{
  /// <summary> Helper attribute that merely indicates who creates/uses a given class. </summary>
  internal class DomainUsageAttribute : Attribute
  {
    public DomainUsageAttribute(DomainId originDomain)
    {
      
    }

    public DomainId CreatedBy { get; set; }

    public DomainId LoadedBy { get; set; }
  }
}