using System;

namespace Codemations.Asn1.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class)]
public class AsnConverterAttribute : AsnAttribute
{
    public Type ConverterType { get; }

    public AsnConverterAttribute(Type converterType)
    {
        if (!typeof(AsnConverter).IsAssignableFrom(converterType))
        {
            throw new ArgumentException($"Type '{converterType.Name}' does not implement '{nameof(AsnConverter)}' interface.");
        }

        ConverterType = converterType;
    }
}