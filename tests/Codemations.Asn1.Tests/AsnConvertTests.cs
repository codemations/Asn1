using NUnit.Framework;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1.Tests
{
    public partial class AsnConvertTests
    {
        private static IEnumerable<TestCaseData> Data()
        {
            var cafeElement = new AsnElement(0x81) {Value = new byte[] {0xCA, 0xFE}};
            var deadBeefElement = new AsnElement(0x82)
                {Value = new byte[] {0xDE, 0xAD, 0xBE, 0xEF}};
            var constructedElement = new AsnElement(0xA0,
                new [] { cafeElement, deadBeefElement }.ToList());

            yield return new TestCaseData(
                new [] { cafeElement, deadBeefElement },
                new byte[] {0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF});

            yield return new TestCaseData(
                new [] { constructedElement },
                new byte[] {0xA0, 0x0A, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF});
        }

        [TestCaseSource(nameof(Data))]
        public void ShouldSerializeAsnElements(ICollection<AsnElement> asnElements, byte[] expectedData)
        {
            // Act
            var actualData = AsnConvert.Serialize(asnElements, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualData, Is.EqualTo(expectedData));
        }

        [TestCaseSource(nameof(Data))]
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
            Assert.That(actualSequence.Count, Is.EqualTo(expectedSequence.Count));
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

        private static IEnumerable<TestCaseData> ChoiceModelData()
        {
            yield return new TestCaseData(
                new ChoiceElement
                {
                    SequenceElement = new SequenceElement
                    {
                        OctetString = new byte[] {0xCA, 0xFE},
                        Integer = 10
                    }
                },
                new byte[] {0xA0, 0x07, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x01, 0x0A});
            yield return new TestCaseData(
                new ChoiceElement
                {
                    SequenceElement = new SequenceElement
                    {
                        Integer = 10
                    }
                },
                new byte[] {0xA0, 0x03, 0x82, 0x01, 0x0A});
            yield return new TestCaseData(
                new ChoiceElement
                {
                    BooleanElement = true
                },
                new byte[] {0x81, 0x01, 0xFF});
        }

        private static IEnumerable<TestCaseData> SequenceOfModelData()
        {
            yield return new TestCaseData(
                new List<bool> { true, false },
                new byte[] {0x30, 0x06, 0x01, 0x01, 0xFF, 0x01, 0x01, 0x00 });
            yield return new TestCaseData(
                new bool[] { true, false },
                new byte[] {0x30, 0x06, 0x01, 0x01, 0xFF, 0x01, 0x01, 0x00 });
        }

        [TestCaseSource(nameof(ChoiceModelData))]
        [TestCaseSource(nameof(SequenceOfModelData))]
        public void ShouldSerializeModel(object element, byte[] expectedData)
        {
            // Act
            var data = AsnConvert.Serialize(element, AsnEncodingRules.DER);

            // Assert
            Assert.That(data, Is.EqualTo(expectedData));
        }

        [TestCaseSource(nameof(ChoiceModelData))]
        public void ShouldDeserializeToChoiceModel(ChoiceElement expectedElement, byte[] data)
        {
            // Act
            var element = AsnConvert.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER);
            
            // Assert
            Assert.That(element.SequenceElement?.Integer, Is.EqualTo(expectedElement.SequenceElement?.Integer));
            Assert.That(element.SequenceElement?.OctetString, Is.EqualTo(expectedElement.SequenceElement?.OctetString));
            Assert.That(element.BooleanElement, Is.EqualTo(expectedElement.BooleanElement));
        }

        [TestCaseSource(nameof(SequenceOfModelData))]
        public void ShouldDeserializeToSequenceOfModel(IEnumerable<bool> expectedElement, byte[] data)
        {
            // Act
            var element = AsnConvert.Deserialize(data, expectedElement.GetType(), AsnEncodingRules.DER);

            // Assert
            Assert.That(element, Is.EqualTo(expectedElement));
        }

        [Test]
        public void ShouldSerializeOid()
        {
            // Arrange
            AsnOid? oid = new AsnOid("1.2.840.113549.1");
            var expectedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var encodedData = AsnConvert.Serialize(oid, AsnEncodingRules.DER);

            // Assert
            Assert.That(encodedData, Is.EqualTo(expectedData));
        }

        [Test]
        public void ShouldDeserializeOid()
        {
            // Arrange
            var expectedOid = new AsnOid("1.2.840.113549.1");
            var encodedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var actualOid = AsnConvert.Deserialize<AsnOid>(encodedData, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualOid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldDeserializeNullableOid()
        {
            // Arrange
            var expectedOid = new AsnOid("1.2.840.113549.1");
            var encodedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var actualOid = AsnConvert.Deserialize<AsnOid?>(encodedData, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualOid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldThrowWhenDeserializing()
        {
            // Arrange
            var data = new byte[] {0xA0, 0x03, 0x82, 0x01, 0x0A, 0x81, 0x01, 0x00};

            // Act & Assert
            Assert.Throws<AsnConversionException>(() => AsnConvert.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER));
        }

        [Test]
        public void ShouldThrowWhenSerializing()
        {
            // Arrange
            var model = new ChoiceElement()
            {
                SequenceElement = new SequenceElement(){Integer = 6},
                BooleanElement = true
            };

            // Act & Assert
            Assert.Throws<AsnConversionException>(() => AsnConvert.Serialize(model, AsnEncodingRules.DER));
        }


        [AsnSequence]
        public class UniversalSequence
        {
            [AsnElement]
            public BigInteger Integer { get; set; }
        }

        public static IEnumerable<TestCaseData> UniversalTagModelData
        {
            get
            {

                yield return new TestCaseData(
                    new UniversalSequence { Integer = 10 },
                    new byte[] {0x30, 0x03, 0x02, 0x01, 0x0A });
            }
        }

        [TestCaseSource(nameof(UniversalTagModelData))]
        public void ShouldSerializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var serialized = AsnConvert.Serialize(model, AsnEncodingRules.DER);

            // Assert
            Assert.That(serialized, Is.EqualTo(data));
        }

        [TestCaseSource(nameof(UniversalTagModelData))]
        public void ShouldDeserializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var deserialized = AsnConvert.Deserialize<UniversalSequence>(data, AsnEncodingRules.DER);

            // Assert
            Assert.That(deserialized.Integer, Is.EqualTo(model.Integer));
        }
    }
}
