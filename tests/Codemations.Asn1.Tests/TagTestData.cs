using NUnit.Framework;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    public class TagTestData
    {
        public static IEnumerable<TestCaseData> ByteData()
        {
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x00, false), (byte)0x80 );
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x1E, false), (byte)0x9E );
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x00, true), (byte)0xA0 );
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x1E, true), (byte)0xBE);
            yield return new TestCaseData(new Asn1Tag(TagClass.Universal, 0x00, false), (byte)0x00);
            yield return new TestCaseData(new Asn1Tag(TagClass.Universal, 0x1E, false), (byte)0x1E);
            yield return new TestCaseData(new Asn1Tag(TagClass.Private, 0x00, true), (byte)0xE0);
            yield return new TestCaseData(new Asn1Tag(TagClass.Private, 0x1E, true), (byte)0xFE);
        }

        public static IEnumerable<TestCaseData> UIntData()
        {
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x00, false), 0x80u);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x1E, false), 0x9Eu);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x1F, false), 0x9F1Fu);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x7F, false), 0x9F7Fu);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x80, false), 0x9F8100u);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x3FFF, false), 0x9FFF7Fu);
            yield return new TestCaseData(new Asn1Tag(TagClass.ContextSpecific, 0x4000, false), 0x9F818000u);
        }
    }
}