using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    [AttributeUsage(AttributeTargets.Property)]
    public class Asn1TagAttribute : Attribute
    {
        public Asn1Tag Tag { get; }

        public Asn1TagAttribute(TagClass tagClass, int tagValue, bool isConstructed = false)
        {
            this.Tag = new Asn1Tag(tagClass, tagValue, isConstructed);
        }

        public Asn1TagAttribute(UniversalTagNumber universalTagNumber, bool isConstructed = false)
        {
            this.Tag = new Asn1Tag(universalTagNumber, isConstructed);
        }
    }
}

