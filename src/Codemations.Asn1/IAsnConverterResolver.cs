using System;
using System.Reflection;

namespace Codemations.Asn1
{
    public interface IAsnConverterResolver
    {
        IAsnConverter Resolve(PropertyInfo propertyInfo);
        IAsnConverter Resolve(Type type);
    }
}