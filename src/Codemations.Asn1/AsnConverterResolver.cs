﻿using Codemations.Asn1.Converters;
using Codemations.Asn1.Extensions;
using System;
using System.Collections.Concurrent;

namespace Codemations.Asn1;

/// <summary>
/// Resolves the appropriate ASN.1 converter for a given type or property, utilizing custom, built-in, and cached converters.
/// </summary>
internal class AsnConverterResolver
{
    /// <summary>
    /// An array of built-in ASN.1 converters that are available by default.
    /// </summary>
    private readonly IAsnConverter[] _builtInConverters;

    /// <summary>
    /// An array of custom ASN.1 converters provided by the user.
    /// </summary>
    private readonly IAsnConverter[] _customConverters;

    /// <summary>
    /// A thread-safe cache that stores resolved converters for types to improve performance on subsequent lookups.
    /// </summary>
    private readonly ConcurrentDictionary<Type, IAsnConverter> _cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnConverterResolver"/> class with default built-in converters.
    /// </summary>
    public AsnConverterResolver() :
        this(GetDefaultBuiltInConverters(), Array.Empty<IAsnConverter>())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnConverterResolver"/> class with a specified array of custom converters.
    /// </summary>
    /// <param name="customConverters">An array of custom converters provided by the user.</param>
    public AsnConverterResolver(params IAsnConverter[] customConverters)
        : this(GetDefaultBuiltInConverters(), customConverters)
    {
    }

    private AsnConverterResolver(IAsnConverter[] builtInConverters, IAsnConverter[] customConverters)
    {
        _builtInConverters = builtInConverters;
        _customConverters = customConverters;
    }

    /// <summary>
    /// Resolves the ASN.1 converter for the specified property info, considering custom and built-in converters.
    /// </summary>
    /// <param name="asnPropertyInfo">The property info for which the converter is to be resolved.</param>
    /// <param name="resolvedType">The resolved underlying type of the property.</param>
    /// <returns>The resolved ASN.1 converter for the specified property info.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="asnPropertyInfo"/> is null.</exception>
    public IAsnConverter Resolve(AsnPropertyInfo asnPropertyInfo, out Type resolvedType)
    {
        if (asnPropertyInfo is null)
        {
            throw new ArgumentNullException(nameof(asnPropertyInfo));
        }

        resolvedType = ResolveType(asnPropertyInfo.PropertyType);

        return asnPropertyInfo.GetAsnConverter() ?? ResolveInternal(resolvedType);
    }

    /// <summary>
    /// Resolves the ASN.1 converter for the specified type, considering custom and built-in converters.
    /// </summary>
    /// <param name="type">The type for which the converter is to be resolved.</param>
    /// <param name="resolvedType">The resolved underlying type.</param>
    /// <returns>The resolved ASN.1 converter for the specified type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is null.</exception>
    public IAsnConverter Resolve(Type type, out Type resolvedType)
    {
        if (type is null)
        {
            throw new ArgumentNullException(nameof(type));
        }

        resolvedType = ResolveType(type);

        return ResolveInternal(resolvedType);
    }

    private IAsnConverter ResolveInternal(Type type)
    {
        return _cache.GetOrAdd(type, ResolveConverter(type));
    }

    private IAsnConverter ResolveConverter(Type type)
    {       
        return Array.Find(_customConverters, converter => converter.CanConvert(type))
            ?? type.GetAsnConverter()
            ?? Array.Find(_builtInConverters, converter => converter.CanConvert(type))
            ?? throw new ArgumentException($"Type '{type.FullName}' cannot be resolved.", nameof(type));
    }

    private static Type ResolveType(Type type)
    {
        return type.IsNullable() ? Nullable.GetUnderlyingType(type)! : type;
    }

    private static IAsnConverter[] GetDefaultBuiltInConverters()
    {
        return new IAsnConverter[]
        {
            new AsnBooleanConverter(),
            new AsnEnumeratedValueConverter(),
            new AsnIntegerConverter.Byte(),
            new AsnIntegerConverter.SByte(),
            new AsnIntegerConverter.UShort(),
            new AsnIntegerConverter.Short(),
            new AsnIntegerConverter.Int32(),
            new AsnIntegerConverter.UInt32(),
            new AsnIntegerConverter.Int64(),
            new AsnIntegerConverter.UInt64(),
            new AsnIntegerConverter.Big(),
            new AsnOctetStringConverter(),
            new AsnOidConverter(),
            new AsnSequenceOfConverter(),
            new AsnChoiceConverter(),
            new AsnPolymorphicChoiceConverter(),
            new AsnSequenceConverter()
        };
    }
}