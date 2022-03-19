using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnObjectIdentifierConverter : AsnTypeConverter
    {
        internal override Type[] AcceptedTypes => new []
        {
            typeof(string)
        };

        public override object Read(AsnReader reader, Asn1Tag tag, Type type)
        {
            return reader.ReadObjectIdentifier(tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag tag, object value)
        {
            writer.WriteObjectIdentifier((string)value, tag);
        }
    }
}