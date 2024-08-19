namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 PrintableString type.
/// </summary>
public readonly struct AsnPrintableString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnPrintableString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnPrintableString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnPrintableString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnPrintableString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnPrintableString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnPrintableString"/> value.</param>
    public static implicit operator string(AsnPrintableString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnPrintableString"/>.
    /// </summary>
    public override string ToString() => _value;
}
