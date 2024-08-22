using System;
using System.Diagnostics;
using System.Formats.Asn1;

namespace Codemations.Asn1.Extensions;

/// <summary>
///   <see cref="Asn1Tag"/> extension methods.
/// </summary>
public static class AsnTagExtensions
{
    /// <summary>
    ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="byte"/>.
    /// </summary>
    /// <returns>Encoded tag.</returns>
    public static byte ToByte(this Asn1Tag tag)
    {
        Span<byte> encodedTag = stackalloc byte[sizeof(byte)];
        if (tag.TryEncode(encodedTag, out int bytesWritter))
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
        if (tag.TryEncode(encodedTagBytes, out int bytesWritten))
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
        return encodedTagBytes.ToAsn1Tag();
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

        Span<byte> encodedTagBytes = stackalloc byte[encodedSize];

        for (var offset = encodedSize - 1; offset >= 0; offset--)
        {
            encodedTagBytes[offset] = (byte)encodedTag;
            encodedTag >>= 8;
        }

        return encodedTagBytes.ToAsn1Tag();
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
}