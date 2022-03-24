using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    public interface IAsnConverter
    {
        object Read(AsnReader reader, Asn1Tag? tag, Type type);
        void Write(AsnWriter writer, Asn1Tag? tag, object value);
    }
}