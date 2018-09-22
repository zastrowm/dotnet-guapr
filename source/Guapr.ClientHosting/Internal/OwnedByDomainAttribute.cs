using System;
using System.Collections.Generic;
using System.Linq;

namespace Guapr.ClientHosting.Internal
{
  /// <summary> Helper attribute that merely indicates who creates/uses a given class. </summary>
  internal class OwnedByDomainAttribute : System.Attribute
  {
    public OwnedByDomainAttribute(DomainAttribute originDomain)
    {
      
    }

    public DomainAttribute CreatedBy { get; set; }
  }
}