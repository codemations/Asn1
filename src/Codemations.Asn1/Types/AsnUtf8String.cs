namespace Codemations.Asn1.Types;

/// <summary>
/// Represents an ASN.1 UTF8String type.
/// </summary>
public readonly struct AsnUtf8String
{
    private readonly string _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="AsnUtf8String"/> struct.
    /// </summary>
    /// <param name="value">The string value.</param>
    public AsnUtf8String(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Implicitly converts a string to an <see cref="AsnUtf8String"/>.
    /// </summary>
    /// <param name="s">The string value.</param>
    public static implicit operator AsnUtf8String(string s) => new(s);

    /// <summary>
    /// Implicitly converts an <see cref="AsnUtf8String"/> to a string.
    /// </summary>
    /// <param name="s">The <see cref="AsnUtf8String"/> value.</param>
    public static implicit operator string(AsnUtf8String s) => s._value;

    /// <summary>
    /// Returns the string representation of the <see cref="AsnUtf8String"/>.
    /// </summary>
    public override string ToString()
    {
        return _value;
    }
}
