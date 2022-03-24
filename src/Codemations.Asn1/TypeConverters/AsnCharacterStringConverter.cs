using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnCharacterStringConverter : AsnTypeConverter
    {
        internal override Type[] AcceptedTypes => new []
        {
            typeof(string)
        };

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return reader.ReadCharacterString(UniversalTagNumber.IA5String, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.WriteCharacterString(UniversalTagNumber.IA5String, (string)value, tag);
        }
    }
}