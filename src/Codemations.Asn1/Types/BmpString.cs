using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 BMPString type.
/// </summary>
[AsnConverter(typeof(AsnBmpStringConverter))]
public readonly struct BmpString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="BmpString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public BmpString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="BmpString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator BmpString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="BmpString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="BmpString"/> value.</param>
    public static implicit operator string(BmpString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="BmpString"/>.
    /// </summary>
    public override string ToString() => _value;
}
