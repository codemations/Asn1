using System;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.TypeConverters;

namespace Codemations.Asn1
{
    internal class AsnDefaultConverter: IAsnConverter
    {
        private readonly AsnTypeConverter[] typeConverters =
        {
            new AsnBooleanConverter(),
            new AsnEnumeratedValueConverter(),
            new AsnIntegerConverter(),
            new AsnOctetStringConverter(),
            new AsnCharacterStringConverter()
        };

        private IAsnConverter GetTypeConverter(Type type)
        {
            try
            {
                return this.typeConverters.Single(x => x.IsAccepted(type));
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Type '{type.Name}' is not handled by '{nameof(AsnDefaultConverter)}'", nameof(type), e);
            }
        }

        public object Read(AsnReader reader, Asn1Tag tag, Type type)
        {
            return GetTypeConverter(type).Read(reader, tag, type);
        }

        public void Write(AsnWriter writer, Asn1Tag tag, object value)
        {
            GetTypeConverter(value.GetType()).Write(writer, tag, value);
        }
    }
}