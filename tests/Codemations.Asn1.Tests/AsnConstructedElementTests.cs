using NUnit.Framework;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    internal class AsnConstructedElementTests
    {
        [Test]
        public void Equals_ElementsWithoutChilds_ShouldReturnTrue()
        {
            // Arrange
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence);
            var ce2 = new AsnConstructedElement(Asn1Tag.Sequence);

            // Act
            var result = ce1.Equals(ce2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ElementsWithSameChild_ShouldReturnTrue()
        {
            // Arrange
            var child = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence) { child };
            var ce2 = new AsnConstructedElement(Asn1Tag.Sequence) { child };

            // Act
            var result = ce1.Equals(ce2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ElementsWithEqualChilds_ShouldReturnTrue()
        {
            // Arrange
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence)
            { 
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 }) 
            };
            var ce2 = new AsnConstructedElement(Asn1Tag.Sequence)
            {
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 })
            };

            // Act
            var result = ce1.Equals(ce2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ElementsWithDifferentTags_ShouldReturnFalse()
        {
            // Arrange
            var e1 = new AsnConstructedElement(Asn1Tag.Sequence);
            var e2 = new AsnConstructedElement(Asn1Tag.SetOf);

            // Act
            var result = e1.Equals(e2);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void GetHashCode_SameElement_ShouldReturnSameValue()
        {
            // Arrange
            var pe = new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 });
            var ce = new AsnConstructedElement(Asn1Tag.Integer) { pe };

            // Act
            var hc1 = ce.GetHashCode();
            var hc2 = ce.GetHashCode();

            // Assert
            Assert.That(hc1, Is.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_EqualElements_ShouldReturnSameValue()
        {
            // Arrange
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence)
            {
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 })
            };
            var ce2 = new AsnConstructedElement(Asn1Tag.Sequence)
            {
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 })
            };

            // Act
            var hc1 = ce1.GetHashCode();
            var hc2 = ce2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_ElementsWithDifferentChilds_ShouldReturnDifferentValue()
        {
            // Arrange
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence)
            {
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2 })
            };
            var ce2 = new AsnConstructedElement(Asn1Tag.Sequence)
            {
                new AsnPrimitiveElement(Asn1Tag.Integer, new byte[] { 1, 2, 3 })
            };

            // Act
            var hc1 = ce1.GetHashCode();
            var hc2 = ce2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.Not.EqualTo(hc2));
        }

        [Test]
        public void GetHashCode_ElementsWithDifferentTags_ShouldReturnDifferentValue()
        {
            // Arrange
            var ce1 = new AsnConstructedElement(Asn1Tag.Sequence);
            var ce2 = new AsnConstructedElement(Asn1Tag.SetOf);

            // Act
            var hc1 = ce1.GetHashCode();
            var hc2 = ce2.GetHashCode();

            // Assert
            Assert.That(hc1, Is.Not.EqualTo(hc2));
        }
    }
}
