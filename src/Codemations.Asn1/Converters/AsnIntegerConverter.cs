using System;
using System.Formats.Asn1;
using System.Numerics;

namespace Codemations.Asn1.Converters;

internal static class AsnIntegerConverter
{
    internal abstract class AsnIntegerConverterBase<T> : AsnConverter<T> where T : notnull
    {
        public abstract bool TryRead(AsnReader reader, Asn1Tag? tag, out T value);

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            if (TryRead(reader, tag, out var value))
            {
                return value;
            }
            throw new AsnConversionException($"Failed to convert ASN.1 integer to {typeof(T).Name}. The value is outside the valid range for this type.");
        }

        public static bool IsInRange(int value, int min, int max)
        {
            return (value >= min && value <= max);
        }

        public static bool IsInRange(uint value, uint min, uint max)
        {
            return (value >= min && value <= max);
        }
    }

    internal class Byte : AsnIntegerConverterBase<byte>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out byte value)
        {
            if(reader.TryReadUInt32(out var uintValue, tag) && IsInRange(uintValue, byte.MinValue, byte.MaxValue))
            {
                value = (byte)uintValue;
                return true;
            }

            value = default;
            return false;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, byte value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class SByte : AsnIntegerConverterBase<sbyte>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out sbyte value)
        {
            if (reader.TryReadInt32(out var intValue, tag) && IsInRange(intValue, sbyte.MinValue, sbyte.MaxValue))
            {
                value = (sbyte)intValue;
                return true;
            }

            value = default;
            return false;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, sbyte value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class UShort : AsnIntegerConverterBase<ushort>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out ushort value)
        {
            if (reader.TryReadUInt32(out var uintValue, tag) && IsInRange(uintValue, ushort.MinValue, ushort.MaxValue))
            {
                value = (ushort)uintValue;
                return true;
            }

            value = default;
            return false;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, ushort value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class Short : AsnIntegerConverterBase<short>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out short value)
        {
            if (reader.TryReadInt32(out var intValue, tag) && IsInRange(intValue, short.MinValue, short.MaxValue))
            {
                value = (short)intValue;
                return true;
            }

            value = default;
            return false;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, short value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class UInt32 : AsnIntegerConverterBase<uint>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out uint value)
        {
            return reader.TryReadUInt32(out value, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, uint value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class Int32 : AsnIntegerConverterBase<int>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out int value)
        {
            return reader.TryReadInt32(out value, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, int value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class UInt64 : AsnIntegerConverterBase<ulong>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out ulong value)
        {
            return reader.TryReadUInt64(out value, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, ulong value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class Int64 : AsnIntegerConverterBase<long>
    {
        public override bool TryRead(AsnReader reader, Asn1Tag? tag, out long value)
        {
            return reader.TryReadInt64(out value, tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, long value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }

    internal class Big : AsnConverter<BigInteger>
    {
        public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
        {
            return reader.ReadInteger(tag);
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, BigInteger value, AsnSerializer serializer)
        {
            writer.WriteInteger(value, tag);
        }
    }
}

