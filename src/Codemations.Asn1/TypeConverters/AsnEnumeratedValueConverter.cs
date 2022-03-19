using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnEnumeratedValueConverter : AsnTypeConverter
    {
        internal override bool IsAccepted(Type type)
        {
            return type.IsEnum;
        }

        public override object Read(AsnReader reader, Asn1Tag tag, Type type)
        {
            return reader.ReadEnumeratedValue(type, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag tag, object value)
        {
            writer.WriteEnumeratedValue((Enum)value, tag);
        }
    }
}