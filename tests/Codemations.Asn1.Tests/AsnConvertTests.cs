using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnConvertTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var cafeElement = new AsnPrimitiveElement(0x81.ToAsn1Tag()) {Value = new byte[] {0xCA, 0xFE}};
                var deadBeefElement = new AsnPrimitiveElement(0x82.ToAsn1Tag())
                    {Value = new byte[] {0xDE, 0xAD, 0xBE, 0xEF}};
                var constructedElement = new AsnConstructedElement(0xA0.ToAsn1Tag(),
                    new AsnElement[] {cafeElement, deadBeefElement}.ToList());

                yield return new object[]
                {
                    new AsnElement[] {cafeElement, deadBeefElement},
                    new byte[] {0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF}
                };

                yield return new object[]
                {
                    new AsnElement[] {constructedElement},
                    new byte[] {0xA0, 0x0A, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF}
                };
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldSerializeAsnElements(ICollection<AsnElement> asnElements, byte[] expectedData)
        {
            // Act
            var actualData = AsnConvert.Serialize(asnElements, AsnEncodingRules.DER);

            // Assert
            Assert.Equal(expectedData, actualData.ToList());
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldDeserializeToAsnElements(ICollection<AsnElement> expectedAsnElements, byte[] data)
        {
            // Act
            var actualAsnElements = AsnConvert.Deserialize(data, AsnEncodingRules.DER);

            // Assert
            AssertAsnElements(expectedAsnElements, actualAsnElements.ToList());
        }

        private static void AssertAsnElements(ICollection<AsnElement> expectedSequence,
            ICollection<AsnElement> actualSequence)
        {
            Assert.Equal(expectedSequence.Count, actualSequence.Count);
            foreach (var (expected, actual) in expectedSequence.Zip(actualSequence, (x, y) => (x, y)))
            {
                Assert.Equal(expected.Tag, actual.Tag);
                if (expected.Tag.IsConstructed)
                {
                    AssertAsnElements(
                        ((AsnConstructedElement) expected).Elements,
                        ((AsnConstructedElement) actual).Elements);
                }
                else
                {
                    Assert.Equal(((AsnPrimitiveElement) expected).Value, ((AsnPrimitiveElement) actual).Value);
                }


            }
        }

        public class ChoiceElement
        {
            [AsnElement(TagClass.ContextSpecific, 0x00, true)]
            public SequenceElement? SequenceElement { get; set; }
        }

        public class SequenceElement
        {
            [AsnElement(TagClass.ContextSpecific, 0x01)]
            public byte[]? OctetString { get; set; }

            [AsnElement(TagClass.ContextSpecific, 0x02)]
            public BigInteger? Integer { get; set; }
        }

        public static IEnumerable<object[]> ModelData
        {
            get
            {
                yield return new object[]
                {
                    new ChoiceElement
                    {
                        SequenceElement = new SequenceElement
                        {
                            OctetString = new byte[] {0xCA, 0xFE},
                            Integer = 10
                        }
                    },
                    new byte[] {0xA0, 0x07, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x01, 0x0A}
                };
            }
        }

        [Theory]
        [MemberData(nameof(ModelData))]
        public void ShouldSerializeModel(ChoiceElement element, byte[] expectedData)
        {
            var data = AsnConvert.Serialize(element, AsnEncodingRules.DER);

            Assert.Equal(expectedData, data);
        }

        [Theory]
        [MemberData(nameof(ModelData))]
        public void ShouldDeserializeToModel(ChoiceElement expectedElement, byte[] data)
        {
            var element = AsnConvert.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER);

            Assert.Equal(expectedElement.SequenceElement!.Integer, element.SequenceElement!.Integer);
            Assert.Equal(expectedElement.SequenceElement!.OctetString, element.SequenceElement!.OctetString);
        }
    }
}
