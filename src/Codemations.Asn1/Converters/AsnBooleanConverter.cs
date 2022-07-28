using System;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.Converters
{
    internal class AsnBooleanConverter : IAsnConverter
    {
        private static readonly Type[] AcceptedTypes = 
        {
            typeof(bool), typeof(bool?)
        };

        public bool CanConvert(Type type)
        {
            return AcceptedTypes.Contains(type);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            return reader.ReadBoolean(tag);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            writer.WriteBoolean((bool)value, tag);
        }
    }
}