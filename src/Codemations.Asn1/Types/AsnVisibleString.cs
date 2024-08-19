namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 VisibleString type.
/// </summary>
public readonly struct AsnVisibleString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnVisibleString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnVisibleString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnVisibleString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnVisibleString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnVisibleString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnVisibleString"/> value.</param>
    public static implicit operator string(AsnVisibleString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnVisibleString"/>.
    /// </summary>
    public override string ToString() => _value;
}
