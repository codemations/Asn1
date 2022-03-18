using System.Formats.Asn1;
using System.Linq;
using Xunit;

namespace Codemations.Asn1.Tests
{
    public class AsnConvertTests
    {
        [Fact]
        public void ShouldDeserializeDataToSingleAsnNode()
        {
            // Arrange
            const byte tag0 = 0xA0;
            const byte tag1 = 0x81;
            const byte tag2 = 0x82;
            const byte value1 = 0xCA;
            const byte value2 = 0xFE;
            var data = new byte[] { tag0, 0x06, tag1, 0x01, value1, tag2, 0x01, value2 };

            // Act
            var asnNode = AsnConvert.Deserialize(data, AsnEncodingRules.DER).Single();

            // Assert
            Assert.Equal(tag0, asnNode.Tag.AsByte());
            Assert.Equal(tag1, asnNode.Nodes![0].Tag.AsByte());
            Assert.Equal(new [] { value1 }, asnNode.Nodes![0].Value.ToArray());
            Assert.Equal(tag2, asnNode.Nodes![1].Tag.AsByte());
            Assert.Equal(new [] { value2 }, asnNode.Nodes![1].Value.ToArray());
        }

        [Fact]
        public void ShouldDeserializeDataToMultipleAsnNodes()
        {
            // Arrange
            const byte tag1 = 0x81;
            const byte tag2 = 0x82;
            const byte value1 = 0xCA;
            const byte value2 = 0xFE;
            var data = new byte[] { tag1, 0x01, value1, tag2, 0x01, value2 };

            // Act
            var asnNodes = AsnConvert.Deserialize(data, AsnEncodingRules.DER).ToList();

            // Assert
            Assert.Equal(tag1, asnNodes[0].Tag.AsByte());
            Assert.Equal(new[] { value1 }, asnNodes[0].Value.ToArray());
            Assert.Equal(tag2, asnNodes[1].Tag.AsByte());
            Assert.Equal(new[] { value2 }, asnNodes[1].Value.ToArray());
        }
    }
}
