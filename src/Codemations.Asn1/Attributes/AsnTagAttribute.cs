using System;
using System.Formats.Asn1;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1.Attributes
{

    [AttributeUsage(AttributeTargets.Property)]
    public class AsnTagAttribute : AsnAttribute
    {
        public Asn1Tag Tag { get; }

        public AsnTagAttribute(byte encodedTag)
        {
            Tag = encodedTag.ToAsn1Tag();
        }

        public AsnTagAttribute(uint encodedTag)
        {
            Tag = encodedTag.ToAsn1Tag();
        }

        public AsnTagAttribute(byte[] encodedTag)
        {
            Tag = encodedTag.ToAsn1Tag();
        }

        public AsnTagAttribute(TagClass tagClass, int tagValue, bool isConstructed = false)
        {
            Tag = new Asn1Tag(tagClass, tagValue, isConstructed);
        }

        public AsnTagAttribute(UniversalTagNumber universalTagNumber, bool isConstructed = false)
        {
            Tag = new Asn1Tag(universalTagNumber, isConstructed);
        }
    }
}

