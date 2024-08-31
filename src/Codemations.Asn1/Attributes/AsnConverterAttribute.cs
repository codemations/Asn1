using System;

namespace Codemations.Asn1.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class)]
public class AsnConverterAttribute : AsnAttribute
{
    public Type ConverterType { get; }

    public AsnConverterAttribute(Type converterType)
    {
        if (!typeof(IAsnConverter).IsAssignableFrom(converterType))
        {
            throw new ArgumentException($"Type '{converterType.Name}' does not implement '{nameof(IAsnConverter)}' interface.");
        }

        ConverterType = converterType;
    }
}