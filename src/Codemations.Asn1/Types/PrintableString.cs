using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 PrintableString type.
/// </summary>
[AsnConverter(typeof(AsnPrintableStringConverter))]
public readonly struct PrintableString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrintableString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public PrintableString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="PrintableString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator PrintableString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="PrintableString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="PrintableString"/> value.</param>
    public static implicit operator string(PrintableString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="PrintableString"/>.
    /// </summary>
    public override string ToString() => _value;
}
