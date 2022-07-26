using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    public class TagTestData
    {
        public static IEnumerable<object[]> ByteData
        {
            get
            {
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x00, false), 0x80 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x1E, false), 0x9E };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x00, true), 0xA0 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x1E, true), 0xBE };
                yield return new object[] { new Asn1Tag(TagClass.Universal, 0x00, false), 0x00 };
                yield return new object[] { new Asn1Tag(TagClass.Universal, 0x1E, false), 0x1E };
                yield return new object[] { new Asn1Tag(TagClass.Private, 0x00, true), 0xE0 };
                yield return new object[] { new Asn1Tag(TagClass.Private, 0x1E, true), 0xFE };
            }
        }

        public static IEnumerable<object[]> UIntData
        {
            get
            {
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x00, false), 0x80 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x1E, false), 0x9E };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x1F, false), 0x9F1F };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x7F, false), 0x9F7F };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x80, false), 0x9F8100 };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x3FFF, false), 0x9FFF7F };
                yield return new object[] { new Asn1Tag(TagClass.ContextSpecific, 0x4000, false), 0x9F818000 };
            }
        }
    }
}