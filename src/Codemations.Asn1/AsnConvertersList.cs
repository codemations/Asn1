using Codemations.Asn1.Converters;
using System;
using System.Collections.Generic;

namespace Codemations.Asn1
{
    internal class AsnConvertersList : List<IAsnConverter>
    {
        private readonly Dictionary<Type, IAsnConverter> _cache = new ();

        public static AsnConvertersList CreateDefault()
        {
            return new AsnConvertersList()
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
        }

        public IAsnConverter Get(Type type)
        {
            if (_cache.TryGetValue(type, out var cachedConverter))
            {
                return cachedConverter;
            }
            foreach (var converter in this)
            {
                if (converter.CanConvert(type))
                {
                    _cache[type] = converter;
                    return converter;
                }
            }

            throw new ArgumentException($"Type '{type.FullName}' is not supported by any of converters.", nameof(type));
        }
    }
}