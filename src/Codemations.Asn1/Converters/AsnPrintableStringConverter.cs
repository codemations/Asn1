using Codemations.Asn1.Types;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnPrintableStringConverter : AsnConverter<AsnPrintableString>
{
    protected override AsnPrintableString ReadTyped(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        return reader.ReadPrintableString(tag);
    }

    protected override void WriteTyped(AsnWriter writer, Asn1Tag? tag, AsnPrintableString value, AsnSerializer serializer)
    {
        writer.WritePrintableString(value, tag);
    }
}
