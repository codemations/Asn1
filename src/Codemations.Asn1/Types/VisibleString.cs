using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 VisibleString type.
/// </summary>
[AsnConverter(typeof(AsnVisibleStringConverter))]
public readonly struct VisibleString
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="VisibleString"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public VisibleString(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="VisibleString"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator VisibleString(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="VisibleString"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="VisibleString"/> value.</param>
    public static implicit operator string(VisibleString s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="VisibleString"/>.
    /// </summary>
    public override string ToString() => _value;
}
