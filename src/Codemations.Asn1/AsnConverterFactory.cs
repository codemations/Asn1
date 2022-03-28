using System;
using System.Linq;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1
{
    internal class AsnConverterFactory
    {
        private readonly Func<AsnConverterFactory,AsnElementConverter>[] converters =
        {
            factory => new AsnBooleanConverter(factory),
            factory => new AsnEnumeratedValueConverter(factory),
            factory => new AsnIntegerConverter(factory),
            factory => new AsnOctetStringConverter(factory),
            factory => new AsnCharacterStringConverter(factory),
            factory => new AsnSequenceOfConverter(factory),
            factory => new AsnChoiceConverter(factory),
            factory => new AsnSequenceConverter(factory)
        };

        public IAsnConverter CreateElementConverter(Type type)
        {
            return CommonExceptionHandler(type, () =>
            {
                return this.converters.Select(func => func(this)).First(converter => converter.IsAccepted(type));
            });
        }

        public AsnRootConverter CreateRootConverter(Type type)
        {
            return CommonExceptionHandler(type, () =>
            {
                var converter = this.converters.Select(func => func(this))
                    .First(converter => converter is AsnRootConverter && converter.IsAccepted(type));
                return (converter as AsnRootConverter)!;
            });
        }

        private static T CommonExceptionHandler<T>(Type type, Func<T> func)
        {
            try
            {
                return func();
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Type '{type.Name}' is not handled by '{nameof(AsnConverterFactory)}'", nameof(type), e);
            }
        }
    }
}