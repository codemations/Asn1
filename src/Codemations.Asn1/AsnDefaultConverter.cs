using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    internal class AsnDefaultConverter: IAsnConverter
    {
        private static IAsnConverter GetTypeConverter(Type type)
        {
            try
            {
                return new AsnTypeConverterFactory().CreateTypeConverter(type);
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Type '{type.Name}' is not handled by '{nameof(AsnDefaultConverter)}'", nameof(type), e);
            }
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return GetTypeConverter(type).Read(reader, tag, type);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            GetTypeConverter(value.GetType()).Write(writer, tag, value);
        }
    }
}