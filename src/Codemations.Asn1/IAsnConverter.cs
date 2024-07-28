using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    public interface IAsnConverter
    {
        bool CanConvert(Type type);
        object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer);
        void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer);
    }
}