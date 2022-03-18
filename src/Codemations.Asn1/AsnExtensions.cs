using System.Formats.Asn1;

namespace Codemations.Asn1
{
    /// <summary>
    ///   <see cref="System.Formats.Asn1"/> extension methods.
    /// </summary>
    public static class AsnExtensions
    {
        /// <summary>
        ///   Converts <see cref="Asn1Tag"/> to value encoded as <see cref="byte"/>.
        /// </summary>
        /// <returns>Encoded tag.</returns>
        public static byte AsByte(this Asn1Tag tag)
        {
            return (byte)((int)tag.TagClass | (tag.IsConstructed ? 0x01 : 0x00) << 5 | tag.TagValue);
        }
    }
}