﻿using System;
using System.Diagnostics;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   <see cref="System.Formats.Asn1"/> extension methods.
    /// </summary>
    public static class AsnExtensions
    {
        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="byte"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static byte ToByte(this Asn1Tag tag)
        {
            Span<byte> encodedTag = stackalloc byte[sizeof(byte)];
            if(tag.TryEncode(encodedTag, out int bytesWritter))
            {              
                return encodedTag[0];
            }

            Debug.Assert(bytesWritter == 0);
            throw new ArgumentOutOfRangeException(nameof(tag), tag,
                    $"Tag value '{tag.TagValue}' cannot be encoded as byte.");
        }

        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="int"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static uint ToUInt(this Asn1Tag tag)
        {          
            Span<byte> encodedTagBytes = stackalloc byte[sizeof(uint)];
            if(tag.TryEncode(encodedTagBytes, out int bytesWritten))
            {
                uint encodedTag = 0;
                foreach (var encodedByte in encodedTagBytes.Slice(0, bytesWritten))
                {
                    encodedTag <<= 8;
                    encodedTag += encodedByte;
                }

                return encodedTag;
            }

            Debug.Assert(bytesWritten == 0);
            throw new ArgumentOutOfRangeException(nameof(tag), tag,
                $"Tag value '{tag.TagValue}' cannot be encoded as uint.");
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="byte"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this byte encodedTag)
        {
            Span<byte> encodedTagBytes = stackalloc byte[] { encodedTag };
            return Asn1Tag.Decode(encodedTagBytes, out _);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="uint"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this uint encodedTag)
        {
            const uint threeBytesMaxValue = 0x00FFFFFF;

            var encodedSize = encodedTag switch
            {
                <= byte.MaxValue => 1,
                <= ushort.MaxValue => 2,
                <= threeBytesMaxValue => 3,
                _ => 4
            };

            Span<byte> bytes = stackalloc byte[encodedSize];

            for (var offset = encodedSize - 1; offset >= 0; offset--)
            {
                bytes[offset] = (byte)encodedTag;
                encodedTag >>= 8;
            }

            return Asn1Tag.Decode(bytes, out _);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="T:byte[]"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this Span<byte> encodedTag)
        {
            return Asn1Tag.Decode(encodedTag, out _);
        }

        /// <summary>
        ///   Converts tag encoded as <see cref="T:byte[]"/> to <see cref="Asn1Tag"/>.
        /// </summary>
        /// <returns><see cref="Asn1Tag"/>.</returns>
        public static Asn1Tag ToAsn1Tag(this byte[] encodedTag)
        {
            return encodedTag.AsSpan().ToAsn1Tag();
        }

        public static ReadOnlyMemory<byte> ReadContentBytes(this AsnReader reader, out Asn1Tag tag)
        {
            var contentBytes = reader.ReadEncodedValue();
            tag = AsnDecoder.ReadEncodedValue(contentBytes.Span, reader.RuleSet, out var contentOffset, out var contentLength, out _);
            return contentBytes.Slice(contentOffset, contentLength);
        }
    }
}