using System;
using System.Formats.Asn1;
using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1;

/// <summary>
/// Represents an Object Identifier (OID) in ASN.1 notation. An OID is a sequence of integers that uniquely identifies a specific object in a globally unique manner.
/// This struct provides methods for creating, validating, and manipulating OIDs.
/// </summary>
[AsnConverter(typeof(AsnOidConverter))]
public readonly partial struct Oid : IEquatable<Oid>
{
    private readonly string _oidString;

    /// <summary>
    /// Initializes a new instance of the <see cref="Oid"/> struct with the specified components.
    /// </summary>
    /// <param name="components">The components of the OID.</param>
    public Oid(params int[] components)
    {
        _oidString = string.Join(".", components);
        Validate(_oidString);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Oid"/> struct with the specified OID string.
    /// </summary>
    /// <param name="oidStr">The dot-delimited OID string.</param>
    public Oid(string oidStr) : this(oidStr, validate: true)
    {
    }

    internal Oid(string oidStr, bool validate)
    {
        if (oidStr is null)
        {
            throw new ArgumentNullException(nameof(oidStr));
        }

        _oidString = oidStr;

        if (validate)
        {
            Validate(oidStr);
        }
    }

    /// <summary>
    /// Implicitly converts an <see cref="Oid"/> instance to its string representation.
    /// </summary>
    /// <param name="oid">The <see cref="Oid"/> instance.</param>
    public static implicit operator string(Oid oid) => oid._oidString;

    /// <summary>
    /// Explicitely converts a string to an <see cref="Oid"/> instance.
    /// </summary>
    /// <param name="oidStr">The OID string to convert.</param>
    public static explicit operator Oid(string oidStr) => new(oidStr);

    /// <summary>
    /// Returns the dot-delimited string representation of the OID.
    /// </summary>
    /// <returns>The OID string.</returns>
    public override string ToString()
    {
        return _oidString;
    }

    /// <summary>
    /// Determines whether this instance and a specified object are equal.
    /// </summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        return obj is Oid other && Equals(other);
    }

    /// <summary>
    /// Indicates whether the current <see cref="Oid"/> is equal to another <see cref="Oid"/>.
    /// </summary>
    /// <param name="other">An <see cref="Oid"/> to compare with this <see cref="Oid"/>.</param>
    /// <returns><c>true</c> if the current <see cref="Oid"/> is equal to the other <see cref="Oid"/>; otherwise, <c>false</c>.</returns>
    public bool Equals(Oid other)
    {
        return _oidString == other._oidString;
    }

    /// <summary>
    /// Determines whether two <see cref="Oid"/> instances are equal.
    /// </summary>
    /// <param name="left">The first <see cref="Oid"/> to compare.</param>
    /// <param name="right">The second <see cref="Oid"/> to compare.</param>
    /// <returns><c>true</c> if the two OIDs are equal; otherwise, <c>false</c>.</returns>
    public static bool operator ==(Oid left, Oid right) => left.Equals(right);

    /// <summary>
    /// Determines whether two <see cref="Oid"/> instances are not equal.
    /// </summary>
    /// <param name="left">The first <see cref="Oid"/> to compare.</param>
    /// <param name="right">The second <see cref="Oid"/> to compare.</param>
    /// <returns><c>true</c> if the two OIDs are not equal; otherwise, <c>false</c>.</returns>
    public static bool operator !=(Oid left, Oid right) => !left.Equals(right);

    /// <summary>
    /// Returns the hash code for this <see cref="Oid"/>.
    /// </summary>
    /// <returns>A 32-bit signed integer hash code.</returns>
    public override int GetHashCode()
    {
        return _oidString.GetHashCode();
    }


    /// <summary>
    /// Determines whether the current <see cref="Oid"/> instance is a prefix of the specified OID string.
    /// </summary>
    /// <param name="other">The OID string to compare with.</param>
    /// <returns>
    /// <c>true</c> if the current <see cref="Oid"/> instance is a prefix of the specified OID string; otherwise, <c>false</c>.
    /// </returns>
    /// <exception cref="FormatException">Thrown if the provided string cannot be converted to an <see cref="Oid"/>.</exception>
    public bool IsPrefixOf(string other)
    {
        return IsPrefixOf((Oid)other);
    }

    /// <summary>
    /// Determines whether the current <see cref="Oid"/> instance is a prefix of the specified OID string.
    /// </summary>
    /// <param name="other">The other <see cref="Oid"/> to compare with.</param>
    /// <returns><c>true</c> if this OID is a prefix of the other OID; otherwise, <c>false</c>.</returns>
    public bool IsPrefixOf(Oid other)
    {
        return other._oidString.StartsWith(_oidString + ".");
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from an encoded OID value using the specified ASN.1 encoding rules.
    /// </summary>
    /// <param name="encodedOidValue">The encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <param name="ruleSet">The ASN.1 encoding rules to use for decoding.</param>
    /// <returns>An <see cref="Oid"/> instance representing the decoded OID.</returns>
    /// <exception cref="FormatException">Thrown when the OID cannot be decoded.</exception>
    public static Oid FromEncodedValue(ReadOnlySpan<byte> encodedOidValue, AsnEncodingRules ruleSet)
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
            var oidString = AsnDecoder.ReadObjectIdentifier(encodedOid, ruleSet, out _, oidTag);
            return new Oid(oidString);
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
    /// <returns>An <see cref="Oid"/> instance representing the decoded OID.</returns>
    public static Oid FromBer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.BER);
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from a CER-encoded OID value.
    /// </summary>
    /// <param encodedOidValue="encodedOidValue">The CER-encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <returns>An <see cref="Oid"/> instance representing the decoded OID.</returns>
    public static Oid FromCer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.CER);
    }

    /// <summary>
    /// Creates an <see cref="Oid"/> instance from a DER-encoded OID value.
    /// </summary>
    /// <param name="encodedOidValue">The DER-encoded OID value as a <see cref="ReadOnlySpan{T}"/> of bytes.</param>
    /// <returns>An <see cref="Oid"/> instance representing the decoded OID.</returns>
    public static Oid FromDer(ReadOnlySpan<byte> encodedOidValue)
    {
        return FromEncodedValue(encodedOidValue, AsnEncodingRules.DER);
    }
}
