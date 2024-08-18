using System;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1.Converters
{
    internal class AsnIntegerConverter : IAsnConverter
    {
        private static readonly Type[] AcceptedTypes =
        {
            typeof(long), typeof(ulong),
            typeof(int), typeof(uint),
            typeof(short), typeof(ushort),
            typeof(sbyte), typeof(byte),
            typeof(BigInteger)
        };

        public bool CanConvert(Type type)
        {
            return AcceptedTypes.Contains(type);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            return reader.ReadInteger(tag);
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
        {
            writer.WriteInteger((BigInteger)value, tag);
        }
    }
}