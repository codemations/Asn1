namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 T61String type.
/// </summary>
public readonly struct AsnT61String
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnT61String"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnT61String(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnT61String"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnT61String(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnT61String"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnT61String"/> value.</param>
    public static implicit operator string(AsnT61String s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnT61String"/>.
    /// </summary>
    public override string ToString() => _value;
}
