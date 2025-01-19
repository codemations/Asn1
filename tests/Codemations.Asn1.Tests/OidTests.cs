
using NUnit.Framework;
using System;

namespace Codemations.Asn1.Tests
{
    internal class OidTests
    {

        [Test]
        public void ShouldCreateOidFromBerEncodedValued()
        {
            // Arrange
            var expectedOid = "1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = Oid.FromBer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldCreateOidFromCerEncodedValued()
        {
            // Arrange
            var expectedOid = "1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = Oid.FromCer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldCreateOidFromDerEncodedValued()
        {
            // Arrange
            var expectedOid = "1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = Oid.FromDer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void FromEncodedValue_InvalidEncodedValued_ShouldThrowFormatException()
        {
            // Arrange
            var invalidEncodedValue = new byte[] { 0x80 };

            // Act & Assert
            Assert.Throws<FormatException>(() => Oid.FromEncodedValue(invalidEncodedValue, System.Formats.Asn1.AsnEncodingRules.BER));
        }

        [TestCase("1.2.840.113549.1", "1.2.840.113549.1", false)]
        [TestCase("1.2.840.113548", "1.2.840.113549.1", false)]
        [TestCase("1.2.840.113549", "1.2.840.113549.1", true)]
        [TestCase("1.2", "1.2.840.113549.1", true)]
        public void ShouldCheckIfIsPrefixOfOtherOid(string oid, string otherOid,  bool expectedIsPrefix)
        {
            // Arrange
            var asnOid = oid;

            // Act
            var isPrefix = asnOid.IsPrefixOf(otherOid);

            // Assert
            Assert.That(isPrefix, Is.EqualTo(expectedIsPrefix));
        }

        [Test]
        public void ShouldCreateOidFromIntComponents()
        {
            // Arrange
            var components = new int[] { 1, 2, 3 };
            var expectedOid = "1.2.3";

            // Act
            var oid = Oid.FromIntArray(components);

            // Assert
            Assert.That(oid.ToString(), Is.EqualTo(expectedOid));
        }

        [TestCase("")]
        [TestCase("1")]
        [TestCase("123")]
        [TestCase("1.2.")]
        [TestCase("...")]
        [TestCase(".1.2")]
        [TestCase("...")]
        [TestCase("A.B")]
        [TestCase("1.B")]
        [TestCase("3.1")]
        [TestCase("1.40")]
        [TestCase("1.2.C")]
        public void ShouldThrowFormatException(string oid)
        {
            // Act & Assert
            Assert.Throws<FormatException>(() => Oid.Validate(oid));
        }
    }
}
