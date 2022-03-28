using System;
using System.Formats.Asn1;
using System.Numerics;

namespace Codemations.Asn1.Converters
{
    internal class AsnIntegerConverter : AsnElementConverter
    {
        public AsnIntegerConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        protected override Type[] AcceptedTypes => new []
        {
            typeof(long), typeof(long?),
            typeof(ulong), typeof(ulong?),
            typeof(int), typeof(int?),
            typeof(uint), typeof(uint?),
            typeof(short), typeof(short?),
            typeof(ushort), typeof(ushort?),
            typeof(sbyte), typeof(sbyte?),
            typeof(byte), typeof(byte?),
            typeof(BigInteger), typeof(BigInteger?)
        };

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            return reader.ReadInteger(tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.WriteInteger((BigInteger)value, tag);
        }
    }
}