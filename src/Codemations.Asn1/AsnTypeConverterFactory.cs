using System;
using System.Linq;
using Codemations.Asn1.TypeConverters;

namespace Codemations.Asn1
{
    internal class AsnTypeConverterFactory
    {
        private readonly AsnTypeConverter[] typeConverters =
        {
            new AsnBooleanConverter(),
            new AsnEnumeratedValueConverter(),
            new AsnIntegerConverter(),
            new AsnOctetStringConverter(),
            new AsnCharacterStringConverter(),
            new AsnSequenceOfConverter(),
            new AsnChoiceConverter(),
            new AsnSequenceConverter()
        };

        public IAsnConverter CreateTypeConverter(Type type)
        {
            try
            {
                return this.typeConverters.First(x => x.IsAccepted(type));
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Type '{type.Name}' is not handled by '{nameof(AsnDefaultConverter)}'", nameof(type), e);
            }
        }

    }
}