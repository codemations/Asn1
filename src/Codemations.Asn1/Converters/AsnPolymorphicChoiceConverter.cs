using Codemations.Asn1.Attributes;
using Codemations.Asn1.Extensions;
using System;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

namespace Codemations.Asn1.Converters;

internal class AsnPolymorphicChoiceConverter : AsnSequenceConverter
{
    public override bool CanConvert(Type type)
    {
        return GetChoices(type).Any();
    }

    public override object Read(AsnReader reader, Asn1Tag? tag, Type type, AsnSerializer serializer)
    {
        var choiceTag = reader.PeekTag();

        var selectedChoice = GetChoices(type).FirstOrDefault(x => x.Tag == choiceTag) ?? throw new Exception();

        return base.Read(reader, choiceTag, selectedChoice.ChoiceType, serializer);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        var baseType = value.GetType().BaseType!;

        var selectedChoice = GetChoices(baseType).FirstOrDefault(x => x.ChoiceType == value.GetType()) ?? throw new Exception();

        base.Write(writer, selectedChoice.Tag, value, serializer);
    }

    private static AsnPolymorphicChoiceAttribute[] GetChoices(Type type)
    {
        return type.GetCustomAttributes<AsnPolymorphicChoiceAttribute>(true).ToArray();
    }
}