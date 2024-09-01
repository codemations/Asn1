using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AsnPolymorphicChoiceAttribute : AsnAttribute
{
    public Asn1Tag Tag { get; }
    public Type ChoiceType { get; }

    public AsnPolymorphicChoiceAttribute(Type choiceType, int contextSpecificTag)
    {
        ChoiceType = choiceType;
        Tag = new Asn1Tag(TagClass.ContextSpecific, contextSpecificTag, true);
    }
}
