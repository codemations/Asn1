namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 IA5String type.
/// </summary>
public readonly struct AsnIA5String
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnIA5String"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnIA5String(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnIA5String"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnIA5String(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnIA5String"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnIA5String"/> value.</param>
    public static implicit operator string(AsnIA5String s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnIA5String"/>.
    /// </summary>
    public override string ToString() => _value;
}
