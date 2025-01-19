using System;
using System.Collections.Concurrent;
using System.Formats.Asn1;
namespace Codemations.Asn1;

/// <summary>
/// Represents an Object Identifier (OID) in ASN.1 notation. An OID is a sequence of integers that uniquely identifies a specific object in a globally unique manner.
/// This struct provides methods for creating, validating, and manipulating OIDs.
/// </summary>
public static partial class Oid
{
    private static readonly ConcurrentDictionary<string, ValidationResult> _validationCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Oid"/> struct with the specified components.
    /// </summary>
    /// <param name="components">The components of the OID.</param>
    public static string FromIntArray(params int[] components)
    {
        var oid = string.Join(".", components);
        Validate(oid);
        return oid;
    }

    /// <summary>
    /// Determines whether the current <see cref="Oid"/> instance is a prefix of the specified OID string.
    /// </summary>
    /// <param name="oid">The OID to check.</param>
    /// <param name="other">The other OID to compare with.</param>
    /// <returns><c>true</c> if this OID is a prefix of the other OID; otherwise, <c>false</c>.</returns>
    public static bool IsPrefixOf(this string oid, string other)
    {
        return other.StartsWith(oid + ".");
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from an encoded OID value using the specified ASN.1 encoding rules.
    /// </summary>
    /// <param name="encodedOidValue">The encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <param name="ruleSet">The ASN.1 encoding rules to use for decoding.</param>
    /// <returns>An OID string representing the decoded OID.</returns>
    /// <exception cref="FormatException">Thrown when the OID cannot be decoded.</exception>
    public static string FromEncodedValue(ReadOnlySpan<byte> encodedOidValue, AsnEncodingRules ruleSet)
    {
        // We need to decode the OID value into its dot-delimited string representation.
        // The method ReadObjectIdentifier can be used for this purpose, but the encoded value must be in TLV (Tag-Length-Value) format.
        // Since there is no direct method to wrap the encoded value in this format, we can use WriteOctetString for this purpose.
        // To bypass the universal tag checks, we use a context-specific tag.
        var oidTag = new Asn1Tag(TagClass.ContextSpecific, 0);

        // Create encoded TLV
        var writer = new AsnWriter(ruleSet);
        writer.WriteOctetString(encodedOidValue, oidTag);
        Span<byte> encodedOid = stackalloc byte[writer.GetEncodedLength()];
        writer.Encode(encodedOid);

        try
        {
            return AsnDecoder.ReadObjectIdentifier(encodedOid, ruleSet, out _, oidTag);
        }
        catch (Exception ex) when (ex is AsnContentException || ex is ArgumentException || ex is ArgumentOutOfRangeException)
        {
            throw new FormatException("Failed to decode OID from encoded value.", ex);
        }
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from a BER-encoded OID value.
    /// </summary>
    /// <param name="encodedOidValue">The BER-encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <returns>An OID string representing the decoded OID.</returns>
    public static string FromBer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.BER);
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from a CER-encoded OID value.
    /// </summary>
    /// <param name="encodedOidValue">The CER-encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <returns>An OID string representing the decoded OID.</returns>
    public static string FromCer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.CER);
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from a DER-encoded OID value.
    /// </summary>
    /// <param name="encodedOidValue">The DER-encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <returns>An OID string representing the decoded OID.</returns>
    public static string FromDer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.DER);
    }

    /// <summary>
    /// Validates the provided OID string. If the OID is invalid, an exception is thrown.
    /// </summary>
    /// <param name="oidStr">The OID string to validate.</param>
    /// <exception cref="FormatException">Thrown when the OID string is invalid.</exception>
    public static void Validate(string oidStr)
    {
        if (!_validationCache.TryGetValue(oidStr, out var validationResult))
        {
            Validate(oidStr.AsSpan(), out validationResult);
            _validationCache[oidStr] = validationResult;
        }

        if (validationResult != ValidationResult.Success)
        {
            ThrowValidationException(validationResult);
        }
    }

    private static void Validate(ReadOnlySpan<char> oidStr, out ValidationResult validationResult)
    {
        if (oidStr.Length< 3)
        {
            validationResult = ValidationResult.TooShort;
            return;
        }

        ReadOnlySpan<char> dotSpan = stackalloc char[] { '.' };
        if (oidStr.StartsWith(dotSpan) || oidStr.EndsWith(dotSpan))
        {
            validationResult = ValidationResult.InvalidFormat;
            return;
        }

        var componentParser = new ComponentParser(oidStr);

        if (!componentParser.TryGetNext(out var firstComponentStr) ||
            !componentParser.TryGetNext(out var secondComponentStr))
        {
            validationResult = ValidationResult.TooShort;
            return;
        }

        if (!TryParseUInt32(firstComponentStr, out var firstComponent) ||
            !TryParseUInt32(secondComponentStr, out var secondComponent))
        {
            validationResult = ValidationResult.InvalidFormat;
            return;
        }

        if (firstComponent > 2)
        {
            validationResult = ValidationResult.InvalidFirstComponent;
            return;
        }

        if (firstComponent < 2 && secondComponent > 39)
        {
            validationResult = ValidationResult.InvalidSecondComponent;
            return;
        }

        while (componentParser.TryGetNext(out var nthComponent))
        {
            if (!TryParseUInt32(nthComponent, out _))
            {
                validationResult = ValidationResult.InvalidFormat;
                return;
            }
        }

        validationResult = ValidationResult.Success;
    }

    private static bool TryParseUInt32(ReadOnlySpan<char> s, out uint result)
    {
#if NET6_0_OR_GREATER
        return uint.TryParse(s, out result);
#else
        return uint.TryParse(s.ToString(), out result);
#endif
    }

    private static void ThrowValidationException(ValidationResult validationResult)
    {
        throw validationResult switch
        {
            ValidationResult.TooShort => new FormatException("The OID string is too short. It must contain at least two components."),
            ValidationResult.InvalidFormat => new FormatException("The OID string contains an invalid format or characters."),
            ValidationResult.InvalidFirstComponent => new FormatException("The first component of the OID must be between 0 and 2."),
            ValidationResult.InvalidSecondComponent => new FormatException("The second component of the OID must be between 0 and 39 when the first component is less than 2."),
            _ => new FormatException("The OID string is invalid.")
        };
    }

    private ref struct ComponentParser
    {
        private readonly ReadOnlySpan<char> _oidStr;
        private int _offset = -1;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentParser"/> struct with the specified OID string.
        /// </summary>
        /// <param name="oidStr">The OID string to parse.</param>
        public ComponentParser(ReadOnlySpan<char> oidStr)
        {
            _oidStr = oidStr;
        }

        /// <summary>
        /// Tries to get the next component of the OID as a span.
        /// </summary>
        /// <param name="componentSpan">The next component of the OID string.</param>
        /// <returns><c>true</c> if a component is found; otherwise, <c>false</c>.</returns>
        public bool TryGetNext(out ReadOnlySpan<char> componentSpan)
        {
            _offset++;

            if (_offset >= _oidStr.Length)
            {
                componentSpan = ReadOnlySpan<char>.Empty;
                return false;
            }
#if NET6_0_OR_GREATER
            var remainingSlice = _oidStr[_offset..];
#else
            var remainingSlice = _oidStr.Slice(_offset);
#endif

            var nextDelimiter = remainingSlice.IndexOf('.');

            if (nextDelimiter == -1)
            {
                componentSpan = remainingSlice;
                _offset = _oidStr.Length;
            }
            else
            {
#if NET6_0_OR_GREATER
                componentSpan = remainingSlice[..nextDelimiter];
#else
                componentSpan = remainingSlice.Slice(0, nextDelimiter);
#endif
                _offset += nextDelimiter;
            }

            return true;
        }
    }

    private enum ValidationResult
    {
        Success,
        TooShort,
        InvalidFormat,
        InvalidFirstComponent,
        InvalidSecondComponent
    }
}
