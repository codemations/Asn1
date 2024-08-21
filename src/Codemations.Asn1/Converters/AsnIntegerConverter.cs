using System;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1.Converters;

internal class AsnIntegerConverter : AsnConverter
{
    private static readonly Type[] AcceptedTypes =
    {
        typeof(long), typeof(ulong),
        typeof(int), typeof(uint),
        typeof(short), typeof(ushort),
        typeof(sbyte), typeof(byte),
        typeof(BigInteger)
    };

    public override bool CanConvert(Type type)
    {
        return AcceptedTypes.Contains(type);
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadInteger(tag);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        writer.WriteInteger((BigInteger)value, tag);
    }
}