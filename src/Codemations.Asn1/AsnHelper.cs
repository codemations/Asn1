using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Codemations.Asn1
{
    internal static class AsnHelper
    {
        internal static IEnumerable<AsnPropertyInfo> GetAsnProperties(this Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                if (propertyInfo.GetCustomAttribute<AsnElementAttribute>() is not AsnElementAttribute asnElementAttribute)
                {
                    continue;
                }

                yield return new AsnPropertyInfo(asnElementAttribute.Tag, asnElementAttribute.Optional, propertyInfo);
            }
        }

        internal static void WriteProperty(this AsnWriter writer, AsnPropertyInfo propertyInfo, object propertyValue, IAsnConverterResolver converterResolver)
        {
            var converter = propertyInfo.CustomConverter ?? converterResolver.Resolve(propertyInfo.Type);
            converter.Write(writer, propertyInfo.Tag, propertyValue, converterResolver);
        }

        internal static object ReadProperty(this AsnReader reader, AsnPropertyInfo propertyInfo, IAsnConverterResolver converterResolver)
        {
            var converter = propertyInfo.CustomConverter ?? converterResolver.Resolve(propertyInfo.Type);
            return converter.Read(reader, propertyInfo.Tag, propertyInfo.Type, converterResolver);
        }

        internal readonly struct AsnPropertyInfo
        {
            private readonly PropertyInfo _propertyInfo;
            private readonly Lazy<AsnConverterAttribute?> _asnConverterAttribute;

            public AsnPropertyInfo(Asn1Tag? tag, bool isOptional, PropertyInfo propertyInfo)
            {
                Tag = tag;
                IsOptional = isOptional;
                _propertyInfo = propertyInfo;
                _asnConverterAttribute = new Lazy<AsnConverterAttribute?>(propertyInfo.GetCustomAttribute<AsnConverterAttribute>);
            }

            public readonly Asn1Tag? Tag { get; }
            public readonly Type Type => _propertyInfo.PropertyType;
            public readonly bool IsOptional { get; }
            // TODO: Change to Type? ConverterType
            public readonly IAsnConverter? CustomConverter => _asnConverterAttribute.Value?.CreateInstance();

            public object? GetValue(object obj) => _propertyInfo.GetValue(obj);

            public void SetValue(object obj, object value)
            {
                _propertyInfo.SetValue(obj, value);
            }
        }
    }


}
