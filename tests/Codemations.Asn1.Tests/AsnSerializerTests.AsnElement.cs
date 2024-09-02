using NUnit.Framework;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Codemations.Asn1.Extensions;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        private static IEnumerable<TestCaseData> GetAsnElementTestCases()
        {
            var cafeElement = new AsnElement(0x81u.ToAsn1Tag()) {Value = new byte[] {0xCA, 0xFE}};
            var deadBeefElement = new AsnElement(0x82u.ToAsn1Tag())
                {Value = new byte[] {0xDE, 0xAD, 0xBE, 0xEF}};
            var constructedElement = new AsnElement(0xA0u.ToAsn1Tag(),
                new [] { cafeElement, deadBeefElement }.ToList());

            yield return new TestCaseData(
                new [] { cafeElement, deadBeefElement },
                new byte[] {0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF});

            yield return new TestCaseData(
                new [] { constructedElement },
                new byte[] {0xA0, 0x0A, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF});
        }

        [TestCaseSource(nameof(GetAsnElementTestCases))]
        public void ShouldSerializeAsnElements(ICollection<AsnElement> asnElements, byte[] expectedData)
        {
            // Act
            var actualData = AsnSerializer.Serialize(asnElements, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualData, Is.EqualTo(expectedData));
        }

        [TestCaseSource(nameof(GetAsnElementTestCases))]
        public void ShouldDeserializeToAsnElements(ICollection<AsnElement> expectedAsnElements, byte[] data)
        {
            // Act
            var actualAsnElements = AsnSerializer.Deserialize(data, AsnEncodingRules.DER);

            // Assert
            AssertAsnElements(expectedAsnElements, actualAsnElements.ToList());
        }

        private static void AssertAsnElements(ICollection<AsnElement> expectedSequence,
            ICollection<AsnElement> actualSequence)
        {
            Assert.That(actualSequence, Has.Count.EqualTo(expectedSequence.Count));
            foreach (var (expected, actual) in expectedSequence.Zip(actualSequence, (x, y) => (x, y)))
            {
                Assert.That(actual.Tag, Is.EqualTo(expected.Tag));
                if (expected.Tag.IsConstructed)
                {
                    AssertAsnElements(expected.Elements.ToList(), actual.Elements.ToList());
                }
                else
                {
                    Assert.That(actual.Value?.ToArray(), Is.EqualTo(expected.Value?.ToArray()));
                }
            }
        }
    }
}
