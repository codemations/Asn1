using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    [AttributeUsage(AttributeTargets.Property)]
    public class AsnElementAttribute : Attribute
    {
        private Type? converterType;

        public Asn1Tag Tag { get; }
        public bool Optional { get; set; }
        public Type? ConverterType
        {
            get => this.converterType;
            set
            {
                if (value is not null && !typeof(IAsnConverter).IsAssignableFrom(value))
                {
                    throw new ArgumentException();
                }

                this.converterType = value;
            }
        }

        public AsnElementAttribute(byte tag)
        {
            this.Tag = tag.ToAsn1Tag();
        }

        public AsnElementAttribute(TagClass tagClass, int tagValue, bool isConstructed = false)
        {
            this.Tag = new Asn1Tag(tagClass, tagValue, isConstructed);
        }

        public AsnElementAttribute(UniversalTagNumber universalTagNumber, bool isConstructed = false)
        {
            this.Tag = new Asn1Tag(universalTagNumber, isConstructed);
        }
    }
}

