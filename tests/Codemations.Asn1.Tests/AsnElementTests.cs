using NUnit.Framework;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    internal class AsnElementTests
    {
        [Test]
        public void EqualOperator_ShouldReturnTrue()
        {
            // Arrange
            AsnElement left = new AsnPrimitiveElement(Asn1Tag.Null);
            AsnElement right = new AsnPrimitiveElement(Asn1Tag.Null);

            // Act
            var result = left == right;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void NotEqualOperator_ShouldReturnFalse()
        {
            // Arrange
            AsnElement left = new AsnPrimitiveElement(Asn1Tag.Null);
            AsnElement right = new AsnPrimitiveElement(Asn1Tag.Null);

            // Act
            var result = left != right;

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_PrimitiveWithConstructed_ShouldReturnFalse()
        {
            // Arrange
            AsnElement pe = new AsnPrimitiveElement(Asn1Tag.Null);
            AsnElement ce = new AsnConstructedElement(Asn1Tag.Sequence);

            // Act
            var result = pe.Equals(ce);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ConstructedWithPrimitive_ShouldReturnFalse()
        {
            // Arrange
            AsnElement pe = new AsnPrimitiveElement(Asn1Tag.Null);
            AsnElement ce = new AsnConstructedElement(Asn1Tag.Sequence);

            // Act
            var result = ce.Equals(pe);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void Equals_ElementsAsObjects_ShouldReturnTrue()
        {
            // Arrange
            object pe1 = new AsnPrimitiveElement(Asn1Tag.Null);
            object pe2 = new AsnPrimitiveElement(Asn1Tag.Null);

            // Act
            var result = pe1.Equals(pe2);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void Equals_ElementsAsObjects_ShouldReturnFalse()
        {
            // Arrange
            object pe1 = new AsnPrimitiveElement(Asn1Tag.Null);
            object pe2 = new AsnPrimitiveElement(Asn1Tag.PrimitiveOctetString);

            // Act
            var result = pe1.Equals(pe2);

            // Assert
            Assert.That(result, Is.False);
        }
    }
}
