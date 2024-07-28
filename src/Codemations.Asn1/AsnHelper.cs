using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Reflection;

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

        public static T CreateInstance<T>(this Type type)
        {
            if (!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Type {type.FullName} is not assignable to {typeof(T).FullName}.");
            }

            return (T)type.CreateInstance();
        }

        public static object CreateInstance(this Type type)
        {
            var ctor = type.GetConstructor(Type.EmptyTypes) ?? 
                throw new InvalidOperationException($"Type {type.FullName} does not have a parameterless constructor.");
            return ctor.Invoke(null);
        }

        public static T CreateInstance<T>(this Type type, params object[] parameters)
        {
            if(!typeof(T).IsAssignableFrom(type))
            {
                throw new InvalidOperationException($"Type {type.FullName} is not assignable to {typeof(T).FullName}.");
            }

            return (T)type.CreateInstance(parameters);
        }

        public static object CreateInstance(this Type type, params object[] parameters)
        {
            // Get the constructor with the specified parameters
            var ctor = type.GetConstructor(parameters.Select(x => x.GetType()).ToArray()) ??
                throw new InvalidOperationException($"Type {type.FullName} does not have a constructor with the specified parameters.");

            // Create an instance using the constructor
            return ctor.Invoke(parameters);
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
