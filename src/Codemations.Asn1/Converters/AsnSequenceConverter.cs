using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Converters;

internal class AsnSequenceConverter : IAsnConverter
{
    public virtual bool CanConvert(Type type)
    {
        return type.IsClass && type != typeof(object);
    }

    public virtual object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
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
                if (asnPropertyInfo.IsRequired)
                {
                    throw new AsnConversionException("Value for required element is missing.", asnPropertyInfo.Tag, e);
                }
            }
        }

        return item;
    }

    public virtual void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        writer.PushSequence(tag);
        foreach (var asnPropertyInfo in value.GetType().GetAsnProperties())
        {
            if(asnPropertyInfo.GetValue(value) is object propertyValue)
            {
                serializer.Serialize(writer, asnPropertyInfo, propertyValue);
            }
            else if (asnPropertyInfo.IsRequired)
            {
                throw new AsnConversionException("Value for required element is missing.");
            }
        }
        writer.PopSequence(tag);
    }
}