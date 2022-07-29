using System;

namespace Codemations.Asn1
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AsnConverterAttribute : Attribute
    {
        public Type ConverterType { get; }

        public AsnConverterAttribute(Type converterType)
        {
            if (!typeof(IAsnConverter).IsAssignableFrom(converterType))
            {
                throw new ArgumentException($"Type '{converterType.Name}' does not implement 'IAsnConverter' interface.");
            }

            this.ConverterType = converterType;
        }

        internal IAsnConverter CreateInstance()
        {
            return (Activator.CreateInstance(this.ConverterType) as IAsnConverter)!;
        }
    }
}