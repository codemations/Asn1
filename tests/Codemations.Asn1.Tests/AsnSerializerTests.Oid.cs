using NUnit.Framework;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        [Test]
        public void Serialize_AsnOid_ShouldPass()
        {
            // Arrange
            AsnOid? oid = new AsnOid("1.2.840.113549.1");
            var expectedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var encodedData = AsnSerializer.Serialize(oid, AsnEncodingRules.DER);

            // Assert
            Assert.That(encodedData, Is.EqualTo(expectedData));
        }

        [Test]
        public void Deserialize_AsnOid_ShouldPass()
        {
            // Arrange
            var expectedOid = new AsnOid("1.2.840.113549.1");
            var encodedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var actualOid = AsnSerializer.Deserialize<AsnOid>(encodedData, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualOid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void Deserialize_NullableAsnOid_ShouldPass()
        {
            // Arrange
            var expectedOid = new AsnOid("1.2.840.113549.1");
            var encodedData = new byte[] { 6, 7, 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var actualOid = AsnSerializer.Deserialize<AsnOid?>(encodedData, AsnEncodingRules.DER);

            // Assert
            Assert.That(actualOid, Is.EqualTo(expectedOid));
        }
    }
}
