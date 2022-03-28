using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnOctetStringConverter : AsnTypeConverter
    {
        protected override Type[] AcceptedTypes => new []
        {
            typeof(byte[])
        };

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return reader.ReadOctetString(tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.WriteOctetString((byte[])value, tag);
        }
    }
}