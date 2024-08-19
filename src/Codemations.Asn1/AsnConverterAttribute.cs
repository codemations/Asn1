using System;

namespace Codemations.Asn1;

[AttributeUsage(AttributeTargets.Property)]
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

    internal AsnConverter CreateInstance()
    {
        return (Activator.CreateInstance(this.ConverterType) as AsnConverter)!;
    }
}