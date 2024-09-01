using Codemations.Asn1.Attributes;
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

        var selectedChoice = GetChoice(type, attribute => attribute.Tag == choiceTag)
            ?? throw new AsnConversionException($"No matching choice found for ASN.1 tag {choiceTag} in type '{type.FullName}'.");

        return base.Read(reader, choiceTag, selectedChoice.ChoiceType, serializer);
    }

    public override void Write(AsnWriter writer, Asn1Tag? tag, object value, AsnSerializer serializer)
    {
        var selectedChoice = GetChoice(value.GetType(), x => x.ChoiceType == value.GetType())
            ?? throw new AsnConversionException($"No matching ASN.1 choice found for the type '{value.GetType().FullName}'.");

        base.Write(writer, selectedChoice.Tag, value, serializer);
    }

    private static AsnPolymorphicChoiceAttribute? GetChoice(Type type, Predicate<AsnPolymorphicChoiceAttribute> predicate)
    {
        var choices = GetChoices(type);
        return Array.Find(choices, predicate);
    }

    private static AsnPolymorphicChoiceAttribute[] GetChoices(Type type)
    {
        return type.GetCustomAttributes<AsnPolymorphicChoiceAttribute>(true).ToArray();
    }
}
