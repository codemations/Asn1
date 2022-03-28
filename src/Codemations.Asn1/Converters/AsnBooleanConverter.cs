using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters
{
    internal class AsnBooleanConverter : AsnElementConverter
    {
        public AsnBooleanConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        protected override Type[] AcceptedTypes => new []
        {
            typeof(bool), typeof(bool?)
        };

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return reader.ReadBoolean(tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.WriteBoolean((bool)value, tag);
        }
    }
}