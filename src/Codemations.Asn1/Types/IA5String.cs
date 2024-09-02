using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 IA5String type.
/// </summary>
[AsnConverter(typeof(AsnIA5StringConverter))]
public readonly struct IA5String
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="IA5String"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public IA5String(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="IA5String"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator IA5String(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="IA5String"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="IA5String"/> value.</param>
    public static implicit operator string(IA5String s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="IA5String"/>.
    /// </summary>
    public override string ToString() => _value;
}
