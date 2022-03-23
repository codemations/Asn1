using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    public class AsnConversionException : Exception
    {
        public Asn1Tag? Tag { get; }

        public AsnConversionException(string? message) : base(message)
        {
        }

        public AsnConversionException(string? message, Exception exception) : base(message, exception)
        {
        }

        public AsnConversionException(string? message, Asn1Tag tag) : base(FormatMessage(message, tag))
        {
            this.Tag = tag;
        }

        public AsnConversionException(string? message, Asn1Tag tag, Exception exception) : base(FormatMessage(message, tag), exception)
        {
            this.Tag = tag;
        }

        private static string FormatMessage(string? message, Asn1Tag tag)
        {
            return $"{message} (Tag '0x{tag.ToByte():X2}')";
        }
    }
}