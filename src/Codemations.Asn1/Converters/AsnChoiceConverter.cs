using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters;

internal class AsnChoiceConverter : AsnConverter
{
    public override bool CanConvert(Type type)
    {
        return type.GetCustomAttribute<AsnChoiceAttribute>() is not null;
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        var innerTag = reader.PeekTag();

        var asnPropertyInfo = GetReadChoiceProperty(innerTag, type);
        var item = type.CreateInstance();
        var value = serializer.Deserialize(reader, asnPropertyInfo);
        asnPropertyInfo.SetValue(item, value);

        return item;
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        var propertyInfo = GetWriteChoiceProperty(value);
        var propertyValue = propertyInfo.GetValue(value)!;
        serializer.Serialize(writer, propertyInfo, propertyValue);
    }

    private static AsnPropertyInfo GetReadChoiceProperty(Asn1Tag tag, Type type)
    {
        var propertyInfos = type.GetAsnProperties()
            .Where(propertyInfo => propertyInfo.Tag == tag)
            .ToArray();

        return propertyInfos.Length switch
        {
            0 => throw new AsnConversionException("No choice element to matching the tag.", tag),
            1 => propertyInfos[0],
            _ => throw new AsnConversionException("Multiple choice elements with the same tag.", tag),
        };
    }
    private static AsnPropertyInfo GetWriteChoiceProperty(object value)
    {
        var propertyInfos = value.GetType().GetProperties()
            .Where(propertyInfo => propertyInfo.GetValue(value) is not null)
            .Select(proertyInfo => new AsnPropertyInfo(proertyInfo))
            .ToArray();

        return propertyInfos.Length switch
        {
            0 => throw new AsnConversionException("No choice element to serialize."),
            1 => propertyInfos[0],
            _ => throw new AsnConversionException("Multiple non-null choice elements."),
        };
    }
}