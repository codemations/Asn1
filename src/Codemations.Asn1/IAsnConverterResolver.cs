using System;

namespace Codemations.Asn1
{
    public interface IAsnConverterResolver
    {
        IAsnConverter Resolve(Type type);
    }
}