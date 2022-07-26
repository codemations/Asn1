using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnEnumeratedValueConverter : AsnElementConverter
    {
        public AsnEnumeratedValueConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type.IsEnum || IsNullableEnum(type);
        }

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var enumType = IsNullableEnum(type) ? Nullable.GetUnderlyingType(type)! : type;
            return reader.ReadEnumeratedValue(enumType, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.WriteEnumeratedValue((Enum)value, tag);
        }

        private static bool IsNullableEnum(Type type)
        {
            return Nullable.GetUnderlyingType(type)?.IsEnum ?? false;
        }
    }
}