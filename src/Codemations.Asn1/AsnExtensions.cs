using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1
{
    /// <summary>
    ///   <see cref="System.Formats.Asn1"/> extension methods.
    /// </summary>
    public static class AsnExtensions
    {
        private const byte ClassMask = 0b1100_0000;
        private const byte ConstructedMask = 0b0010_0000;
        private const byte TagNumberMask = 0b0001_1111;
        private const byte HighTagNumberIdentifier = TagNumberMask;
        private const byte MaxLowTagNumber = HighTagNumberIdentifier - 1;
        private const int MaxUIntEncodedTagNumber = 0b0001_1111_1111_1111_1111_1111;
        private const byte Base128Mask = 0b0111_1111;
        private const byte HighTagLastOctetMask = 0b1000_0000;
        private const byte HighTagNotLastOctet = HighTagLastOctetMask;

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="byte"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static byte ToByte(this Asn1Tag tag)
        {
            if (tag.TagValue > MaxLowTagNumber)
            {
                throw new ArgumentOutOfRangeException(nameof(tag), tag,
                    $"Tag value '{tag.TagValue}' cannot be encoded as byte.");
            }

            return (byte)((int)tag.TagClass | (tag.IsConstructed ? ConstructedMask : 0) | tag.TagValue);
        }

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="int"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static uint ToUInt(this Asn1Tag tag)
        {
            switch (tag.TagValue)
            {
                case > MaxUIntEncodedTagNumber:
                    throw new ArgumentOutOfRangeException(nameof(tag), tag,
                        $"Tag value '{tag.TagValue}' cannot be encoded as uint.");

                case <= MaxLowTagNumber:
                    return tag.ToByte();
            }

            var tagValue = 0u;
            var exponent = 0;

            foreach (var encodedByte in tag.ToOctetString().Reverse())
            {
                tagValue += encodedByte * (uint)Math.Pow(256, exponent);
                exponent++;
            }

            return tagValue;
        }

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="T:byte[]"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static byte[] ToOctetString(this Asn1Tag tag)
        {
            if (tag.TagValue <= MaxLowTagNumber)
            {
                return new[] { tag.ToByte() };
            }

            var tagValue = tag.TagValue;
            var tagOctetString = new List<byte>
            {
                (byte)(tagValue & Base128Mask)
            };

            while (tagValue > Base128Mask)
            {
                tagValue >>= 7;
                tagOctetString.Add((byte)(HighTagNotLastOctet | (tagValue & Base128Mask)));
            }

            tagOctetString.Add((byte)((int) tag.TagClass | (tag.IsConstructed ? ConstructedMask : 0) | HighTagNumberIdentifier));

            tagOctetString.Reverse();
            return tagOctetString.ToArray();
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="byte"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this byte tag)
        {
            return new Asn1Tag((TagClass)(tag & ClassMask), tag & TagNumberMask, (tag & ConstructedMask) != 0);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="uint"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this uint tag)
        {
            if (tag == 0)
            {
                return ((byte)0).ToAsn1Tag();
            }

            IEnumerable<byte> tagBytes = BitConverter.GetBytes(tag);

            if (BitConverter.IsLittleEndian)
            {
                tagBytes = tagBytes.Reverse();
            }

            return ToAsn1Tag(tagBytes.SkipWhile(x => x == 0).ToArray());
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="T:byte[]"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this byte[] tag)
        {
            switch (tag.Length)
            {
                case 0:
                    throw new ArgumentException("Tag cannot be empty.", nameof(tag));

                case 1:
                    return tag.Single().ToAsn1Tag();

                default:
                    var first = tag.First();
                    var tagClass = (TagClass)(first & ClassMask);
                    var isConstructed = (first & ConstructedMask) != 0;

                    if ((first & TagNumberMask) != HighTagNumberIdentifier)
                    {
                        throw new ArgumentException("Invalid high tag number form.", nameof(tag));
                    }

                    var tagValue = GetTagValueFromBase128(tag.Skip(1));

                    if (tagValue > int.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(tag), tagValue, $"Tag values greater than {int.MaxValue} are not supported.");
                    }

                    return new Asn1Tag(tagClass, (int)tagValue, isConstructed);
            }
        }

        public static ReadOnlyMemory<byte> ReadContentBytes(this AsnReader reader, out Asn1Tag tag)
        {
            var contentBytes = reader.ReadEncodedValue();
            tag = AsnDecoder.ReadEncodedValue(contentBytes.Span, reader.RuleSet, out var contentOffset, out var contentLength, out _);
            return contentBytes.Slice(contentOffset, contentLength);
        }

        private static BigInteger GetTagValueFromBase128(IEnumerable<byte> base128TagValue)
        {
            BigInteger tagValue = 0;
            var exponent = 0;

            foreach (var tagNumberByte in base128TagValue.Reverse())
            {
                tagValue += (tagNumberByte & Base128Mask) * (int)Math.Pow(128, exponent);
                exponent++;
            }

            return tagValue;
        }
    }
}