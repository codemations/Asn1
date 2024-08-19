namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 NumericString type.
/// </summary>
public readonly struct AsnNumericString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnNumericString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnNumericString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnNumericString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnNumericString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnNumericString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnNumericString"/> value.</param>
    public static implicit operator string(AsnNumericString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnNumericString"/>.
    /// </summary>
    public override string ToString() => _value;
}
