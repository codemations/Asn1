using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnBooleanConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type == typeof(bool);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            return reader.ReadBoolean(tag);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
        {
            writer.WriteBoolean((bool)value, tag);
        }
    }
}