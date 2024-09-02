using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Extensions;

public static class AsnReaderExtensions
{
    public static ReadOnlyMemory<byte> ReadContentBytes(this AsnReader reader, out Asn1Tag tag)
    {
        var contentBytes = reader.ReadEncodedValue();
        tag = AsnDecoder.ReadEncodedValue(contentBytes.Span, reader.RuleSet, out var contentOffset, out var contentLength, out _);
        return contentBytes.Slice(contentOffset, contentLength);
    }

    public static string ReadUtf8String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.UTF8String, tag);
    }

    public static NumericString ReadNumericString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.NumericString, tag);
    }

    public static PrintableString ReadPrintableString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.PrintableString, tag);
    }

    public static IA5String ReadIA5String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.IA5String, tag);
    }

    public static VisibleString ReadVisibleString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.VisibleString, tag);
    }

    public static BmpString ReadBmpString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.BMPString, tag);
    }

    public static T61String ReadT61String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.T61String, tag);
    }

}