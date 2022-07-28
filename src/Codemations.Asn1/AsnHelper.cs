using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1
{
    internal static class AsnHelper
    {
        public static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            return type.GetProperties().Where(x => x.GetCustomAttribute<AsnElementAttribute>() is not null);
        }
    }
}
