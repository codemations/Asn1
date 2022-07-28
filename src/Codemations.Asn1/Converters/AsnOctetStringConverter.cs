using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnOctetStringConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return typeof(byte[]) == type;
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            return reader.ReadOctetString(tag);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            writer.WriteOctetString((byte[])value, tag);
        }
    }
}