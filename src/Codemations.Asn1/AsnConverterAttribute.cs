using System;

namespace Codemations.Asn1;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Struct | AttributeTargets.Class)]
public class AsnConverterAttribute : Attribute
{
    public Type ConverterType { get; }

    public AsnConverterAttribute(Type converterType)
    {
        if (!typeof(AsnConverter).IsAssignableFrom(converterType))
        {
            throw new ArgumentException($"Type '{converterType.Name}' does not implement '{nameof(AsnConverter)}' interface.");
        }

        this.ConverterType = converterType;
    }
}