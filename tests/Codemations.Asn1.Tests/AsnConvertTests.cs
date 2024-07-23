﻿using System.Collections.Generic;
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
                var cafeElement = new AsnElement(0x81) {Value = new byte[] {0xCA, 0xFE}};
                var deadBeefElement = new AsnElement(0x82)
                    {Value = new byte[] {0xDE, 0xAD, 0xBE, 0xEF}};
                var constructedElement = new AsnElement(0xA0,
                    new [] { cafeElement, deadBeefElement }.ToList());

                yield return new object[]
                {
                    new [] { cafeElement, deadBeefElement },
                    new byte[] {0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF}
                };

                yield return new object[]
                {
                    new [] { constructedElement },
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
            Assert.Equal(expectedData, actualData);
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
                    AssertAsnElements(expected.Elements.ToList(), actual.Elements.ToList());
                }
                else
                {
                    Assert.Equal(expected.Value?.ToArray(), actual.Value?.ToArray());
                }
            }
        }

        [AsnChoice]
        public class ChoiceElement
        {
            [AsnElement(0xA0)]
            public SequenceElement? SequenceElement { get; set; }

            [AsnElement(0x81)]
            public bool? BooleanElement { get; set; }
        }

        [AsnSequence]
        public class SequenceElement
        {
            [AsnElement(0x81, Optional = true)]
            public byte[]? OctetString { get; set; }

            [AsnElement(0x82)]
            public BigInteger? Integer { get; set; }
        }

        public static IEnumerable<object[]> ChoiceModelData
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
                yield return new object[]
                {
                    new ChoiceElement
                    {
                        SequenceElement = new SequenceElement
                        {
                            Integer = 10
                        }
                    },
                    new byte[] {0xA0, 0x03, 0x82, 0x01, 0x0A}
                };
                yield return new object[]
                {
                    new ChoiceElement
                    {
                        BooleanElement = true
                    },
                    new byte[] {0x81, 0x01, 0xFF}
                };
            }
        }
        public static IEnumerable<object[]> SequenceOfModelData
        {
            get
            {

                yield return new object[]
                {
                    new List<bool> { true, false },
                    new byte[] {0x30, 0x06, 0x01, 0x01, 0xFF, 0x01, 0x01, 0x00 }
                };
            }
        }

        [Theory]
        [MemberData(nameof(ChoiceModelData))]
        [MemberData(nameof(SequenceOfModelData))]
        public void ShouldSerializeModel(object element, byte[] expectedData)
        {
            // Act
            var data = AsnConvert.Serialize(element, AsnEncodingRules.DER);

            // Assert
            Assert.Equal(expectedData, data);
        }

        [Theory]
        [MemberData(nameof(ChoiceModelData))]
        public void ShouldDeserializeToChoiceModel(ChoiceElement expectedElement, byte[] data)
        {
            // Act
            var element = AsnConvert.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER);
            
            // Assert
            Assert.Equal(expectedElement.SequenceElement?.Integer, element.SequenceElement?.Integer);
            Assert.Equal(expectedElement.SequenceElement?.OctetString, element.SequenceElement?.OctetString);
            Assert.Equal(expectedElement.BooleanElement, element.BooleanElement);
        }

        [Theory]
        [MemberData(nameof(SequenceOfModelData))]
        public void ShouldDeserializeToSequenceOfModel(List<bool> expectedElement, byte[] data)
        {
            // Act
            var element = AsnConvert.Deserialize<List<bool>>(data, AsnEncodingRules.DER);

            // Assert
            Assert.Equal(expectedElement, element);
        }

        [Fact]
        public void ShouldThrowWhenDeserializing()
        {
            // Arrange
            var data = new byte[] {0xA0, 0x03, 0x82, 0x01, 0x0A, 0x81, 0x01, 0x00};

            // Act & Assert
            Assert.Throws<AsnConversionException>(() => AsnConvert.Deserialize<ChoiceElement>(data, AsnEncodingRules.DER));
        }

        [Fact]
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

        public static IEnumerable<object[]> UniversalTagModelData
        {
            get
            {

                yield return new object[]
                {
                    new UniversalSequence { Integer = 10 },
                    new byte[] {0x30, 0x03, 0x02, 0x01, 0x0A }
                };
            }
        }

        [Theory]
        [MemberData(nameof(UniversalTagModelData))]
        public void ShouldSerializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var serialized = AsnConvert.Serialize(model, AsnEncodingRules.DER);

            // Assert
            Assert.Equal(data, serialized);
        }

        [Theory]
        [MemberData(nameof(UniversalTagModelData))]
        public void ShouldDeserializeUniversalSequence(UniversalSequence model, byte[] data)
        {
            // Act
            var deserialized = AsnConvert.Deserialize<UniversalSequence>(data, AsnEncodingRules.DER);

            // Assert
            Assert.Equal(model.Integer, deserialized.Integer);
        }
    }
}
