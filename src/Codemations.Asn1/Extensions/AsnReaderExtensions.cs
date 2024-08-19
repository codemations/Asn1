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

    public static AsnUtf8String ReadUtf8String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.UTF8String, tag);
    }

    public static AsnNumericString ReadNumericString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.NumericString, tag);
    }

    public static AsnPrintableString ReadPrintableString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.PrintableString, tag);
    }

    public static AsnIA5String ReadIA5String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.IA5String, tag);
    }

    public static AsnVisibleString ReadVisibleString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.VisibleString, tag);
    }

    public static AsnBmpString ReadBmpString(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.BMPString, tag);
    }

    public static AsnT61String ReadT61String(this AsnReader reader, Asn1Tag? tag = null)
    {
        return reader.ReadCharacterString(UniversalTagNumber.T61String, tag);
    }

}