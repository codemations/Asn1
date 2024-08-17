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
            return type.GetProperties().Select(propertyInfo => new AsnPropertyInfo(propertyInfo));
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
    }
}
