using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 NumericString type.
/// </summary>
[AsnConverter(typeof(AsnNumericStringConverter))]
public readonly struct NumericString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public NumericString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="NumericString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator NumericString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="NumericString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="NumericString"/> value.</param>
    public static implicit operator string(NumericString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="NumericString"/>.
    /// </summary>
    public override string ToString() => _value;
}
