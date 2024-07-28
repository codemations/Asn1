using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnEnumeratedValueConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return type.IsEnum || IsNullableEnum(type);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            var enumType = IsNullableEnum(type) ? Nullable.GetUnderlyingType(type)! : type;
            return reader.ReadEnumeratedValue(enumType, tag);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
        {
            writer.WriteEnumeratedValue((Enum)value, tag);
        }

        private static bool IsNullableEnum(Type type)
        {
            return Nullable.GetUnderlyingType(type)?.IsEnum ?? false;
        }
    }
}