using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1
{
    public class DefaultConverterResolver: IAsnConverterResolver
    {
        private readonly IAsnConverter[] builtInConverters =
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

        private readonly List<IAsnConverter> customConverters = new();

        public IAsnConverter Resolve(PropertyInfo propertyInfo)
        {
            var converterAttribute = propertyInfo.GetCustomAttribute<AsnConverterAttribute>();
            if (converterAttribute is not null)
            {
                return converterAttribute.CreateInstance();
            }

            return Resolve(propertyInfo.PropertyType);
        }

        public IAsnConverter Resolve(Type type)
        {
            try
            {
                return this.customConverters.Concat(this.builtInConverters).First(converter => converter.CanConvert(type));
            }
            catch (InvalidOperationException e)
            {
                throw new ArgumentException($"Type '{type.Name}' is not handled by '{nameof(DefaultConverterResolver)}'", nameof(type), e);
            }
        }

        public void AddConverter(IAsnConverter converter)
        {
            this.customConverters.Add(converter);
        }
    }
}