using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnConvertTests
    {
        public static IEnumerable<object[]> Data
        {
            get
            {
                var cafeNode = new AsnNode { Tag = 0x81.ToAsn1Tag(), Value = new byte[] { 0xCA, 0xFE } };
                var deadBeefNode = new AsnNode { Tag = 0x82.ToAsn1Tag(), Value = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF } };

                yield return new object[] {
                    new []{ cafeNode,  deadBeefNode },
                    new byte[] { 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF }
                };

                yield return new object[] {
                    new AsnNode[]{
                        new() {
                            Tag = 0xA0.ToAsn1Tag(),
                            Value = new byte[]{ 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF },
                            Nodes = new []{ cafeNode,  deadBeefNode }.ToList()
                        }
                    },
                    new byte[] { 0xA0, 0x0A, 0x81, 0x02, 0xCA, 0xFE, 0x82, 0x04, 0xDE, 0xAD, 0xBE, 0xEF }
                };
            }
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldSerializeData(ICollection<AsnNode> asnNodes, byte[] expectedData)
        {
            // Act
            var actualData = AsnConvert.Serialize(asnNodes, AsnEncodingRules.DER).ToList();

            // Assert
            Assert.Equal(expectedData, actualData);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void ShouldDeserializeData(ICollection<AsnNode> expectedAsnNodes, byte[] data)
        {
            // Act
            var actualAsnNodes = AsnConvert.Deserialize(data, AsnEncodingRules.DER).ToList();

            // Assert
            AssertAsnNodes(expectedAsnNodes, actualAsnNodes);
        }

        private static void AssertAsnNodes(ICollection<AsnNode> expectedSequence, ICollection<AsnNode> actualSequence)
        {
            Assert.Equal(expectedSequence.Count, actualSequence.Count);
            foreach(var (expected, actual) in expectedSequence.Zip(actualSequence, (x, y) => (x, y)))
            {
                Assert.Equal(expected.Tag,actual.Tag);
                Assert.Equal(expected.Value, actual.Value);
                if (expected.Nodes is not null && actual.Nodes is not null)
                {
                    AssertAsnNodes(expected.Nodes, actual.Nodes);
                }
                else
                {
                    Assert.Null(expected.Nodes);
                    Assert.Null(actual.Nodes);
                }
            }
        }
    }
}
