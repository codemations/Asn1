using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Extensions;

public static class AsnWriterExtensions
{
    public static void WriteUtf8String(this AsnWriter writer, string value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.UTF8String, value, tag);
    }

    public static void WriteNumericString(this AsnWriter writer, NumericString value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.NumericString, value, tag);
    }

    public static void WritePrintableString(this AsnWriter writer, PrintableString value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.PrintableString, value, tag);
    }

    public static void WriteIA5String(this AsnWriter writer, IA5String value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.IA5String, value, tag);
    }

    public static void WriteVisibleString(this AsnWriter writer, VisibleString value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.VisibleString, value, tag);
    }

    public static void WriteBmpString(this AsnWriter writer, BmpString value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.BMPString, value, tag);
    }

    public static void WriteT61String(this AsnWriter writer, T61String value, Asn1Tag? tag = null)
    {
        writer.WriteCharacterString(UniversalTagNumber.T61String, value, tag);
    }
}