
using NUnit.Framework;
using System;

namespace Codemations.Asn1.Tests
{
    internal class AsnOidTests
    {

        [Test]
        public void ShouldCreateOidFromBerEncodedValued()
        {
            // Arrange
            var expectedOid = (AsnOid)"1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = AsnOid.FromBer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldCreateOidFromCerEncodedValued()
        {
            // Arrange
            var expectedOid = (AsnOid)"1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = AsnOid.FromCer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [Test]
        public void ShouldCreateOidFromDerEncodedValued()
        {
            // Arrange
            var expectedOid = (AsnOid)"1.2.840.113549.1";
            var encodedValue = new byte[] { 42, 134, 72, 134, 247, 13, 1 };

            // Act
            var oid = AsnOid.FromDer(encodedValue);

            // Assert
            Assert.That(oid, Is.EqualTo(expectedOid));
        }

        [TestCase("1.2.840.113549.1", "1.2.840.113549.1", false)]
        [TestCase("1.2.840.113548", "1.2.840.113549.1", false)]
        [TestCase("1.2.840.113549", "1.2.840.113549.1", true)]
        [TestCase("1.2", "1.2.840.113549.1", true)]
        public void ShouldCheckIfIsPrefixOfOtherOid(string oid, string otherOid,  bool expectedIsPrefix)
        {
            // Arrange
            var asnOid = (AsnOid)oid;

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
            var oid = new AsnOid(components);

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
            Assert.Throws<FormatException>(() => new AsnOid(oid));
        }

        [TestCase("1.2", "1.2")]
        public void ShouldBeEqual(string left, string right)
        {
            // Arrange
            var leftOid = new AsnOid(left);
            var rightOid = new AsnOid(right);

            // Act
            var areEqual = leftOid == rightOid;

            //
            Assert.That(areEqual, Is.True);
        }

        [TestCase("1.2", "1.3")]
        [TestCase("2.2", "1.2")]
        [TestCase("1.2", "1.2.3")]
        public void ShouldNotBeEqual(string left, string right)
        {
            // Arrange
            var leftOid = new AsnOid(left);
            var rightOid = new AsnOid(right);

            // Act
            var areNotEqual = leftOid != rightOid;

            //
            Assert.That(areNotEqual, Is.True);
        }

        [Test]
        public void GetHashCode_ShouldBeConsistent()
        {
            // Arrange
            var oid = new AsnOid("1.2.3.4");

            // Act
            var hash1 = oid.GetHashCode();
            var hash2 = oid.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2), "GetHashCode should return the same value on multiple invocations for the same object.");
        }

        [Test]
        public void GetHashCode_EqualObjects_ShouldHaveSameHashCode()
        {
            // Arrange
            var oid1 = new AsnOid("1.2.3.4");
            var oid2 = new AsnOid("1.2.3.4");

            // Act
            var hash1 = oid1.GetHashCode();
            var hash2 = oid2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.EqualTo(hash2), "Equal AsnOid objects should return the same hash code.");
        }

        [Test]
        public void GetHashCode_DifferentObjects_ShouldHaveDifferentHashCodes()
        {
            // Arrange
            var oid1 = new AsnOid("1.2.3.4");
            var oid2 = new AsnOid("1.2.3.5");

            // Act
            var hash1 = oid1.GetHashCode();
            var hash2 = oid2.GetHashCode();

            // Assert
            Assert.That(hash1, Is.Not.EqualTo(hash2), "Different AsnOid objects should ideally return different hash codes.");
        }

        [Test]
        public void AsnOidCtor_NullOidStr_ShouldThrow()
        {
            // Arrange
            string? oidStr = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new AsnOid(oidStr!));
        }
    }
}
