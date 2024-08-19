using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnSequenceConverter : AsnConverter
{
    public override bool CanConvert(Type type)
    {
        return type.IsClass;
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        var sequenceReader = reader.ReadSequence(tag);

        var item = type.CreateInstance();

        foreach (var asnPropertyInfo in type.GetAsnProperties())
        {
            try
            {
                var value = serializer.Deserialize(sequenceReader, asnPropertyInfo);
                asnPropertyInfo.SetValue(item, value);
            }
            catch (AsnContentException e)
            {
                if (!asnPropertyInfo.IsOptional)
                {
                    throw new AsnConversionException("Value for required element is missing.", asnPropertyInfo.Tag, e);
                }
            }
        }

        return item;
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        writer.PushSequence(tag);
        foreach (var asnPropertyInfo in value.GetType().GetAsnProperties())
        {
            if(asnPropertyInfo.GetValue(value) is object propertyValue)
            {
                serializer.Serialize(writer, asnPropertyInfo, propertyValue);
            }
            else if (!asnPropertyInfo.IsOptional)
            {
                throw new AsnConversionException("Value for required element is missing.");
            }
        }
        writer.PopSequence(tag);
    }
}