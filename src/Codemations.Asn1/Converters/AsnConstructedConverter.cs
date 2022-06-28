using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters
{
    internal abstract class AsnConstructedConverter : AsnElementConverter
    {
        protected AsnConstructedConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        protected static IEnumerable<PropertyInfo> GetPropertyInfos(Type type)
        {
            return type.GetProperties().Where(x => x.GetCustomAttribute<AsnElementAttribute>() is not null);
        }
    }
}