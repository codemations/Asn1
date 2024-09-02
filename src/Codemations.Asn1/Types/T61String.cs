using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;

namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 T61String type.
/// </summary>
[AsnConverter(typeof(AsnT61StringConverter))]
public readonly struct T61String
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="T61String"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public T61String(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="T61String"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator T61String(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="T61String"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="T61String"/> value.</param>
    public static implicit operator string(T61String s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="T61String"/>.
    /// </summary>
    public override string ToString() => _value;
}
