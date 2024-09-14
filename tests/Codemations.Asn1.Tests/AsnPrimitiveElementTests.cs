using NUnit.Framework;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{

    internal class AsnPrimitiveElementTests
    {
        [Test]
        public void Equals_ElementWithSameValue_ShouldReturnTrue()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });

            // Act
            var result = e1.Equals(e2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ElementsWithDifferentTags_ShouldReturnFalse()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.PrimitiveOctetString, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });

            // Act
            var result = e1.Equals(e2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ElementsWithDifferentValues_ShouldReturnFalse()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2, 3 });

            // Act
            var result = e1.Equals(e2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetHashCode_SameElement_ShouldReturnSameValue()
        {
            // Arrange
            var element = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });

            // Act
            var hc1 = element.GetHashCode();
            var hc2 = element.GetHashCode();

            // Assert
            Assert.That(hc1, Is.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_EqualElements_ShouldReturnSameValue()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });

            // Act
            var hc1 = e1.GetHashCode();
            var hc2 = e2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_ElementsWithDifferentValues_ShouldReturnDifferentValue()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2, 3 });

            // Act
            var hc1 = e1.GetHashCode();
            var hc2 = e2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.Not.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_ElementsWithDifferentTags_ShouldReturnDifferentValue()
        {
            // Arrange
            var e1 = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var e2 = new AsnPrimitiveElement(Asn1Tag.PrimitiveOctetString, new byte[] { 1, 2 });

            // Act
            var hc1 = e1.GetHashCode();
            var hc2 = e2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.Not.EqualTo(hc2));
        }
    }
}
