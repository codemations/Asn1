namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 BMPString type.
/// </summary>
public readonly struct AsnBmpString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnBmpString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnBmpString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnBmpString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnBmpString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnBmpString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnBmpString"/> value.</param>
    public static implicit operator string(AsnBmpString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnBmpString"/>.
    /// </summary>
    public override string ToString() => _value;
}
