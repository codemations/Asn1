using Codemations.Asn1.Extensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        private static IEnumerable<TestCaseData> GetBuiltInPrimitiveDataTestCases()
        {
            yield return new TestCaseData(false, (byte)0x80, new byte[] { 0x80, 0x01, 000 });
            yield return new TestCaseData(true, (byte)0x81, new byte[] { 0x81, 0x01, 0xFF });
            yield return new TestCaseData(new byte[] { 0xCA, 0xFE }, (byte)0x83, new byte[] { 0x83, 0x02, 0xCA, 0xFE });
            yield return new TestCaseData(new byte[] { 0xCA, 0xFE }, (byte)0x84, new byte[] { 0x84, 0x02, 0xCA, 0xFE });
            yield return new TestCaseData(TestEnum.Success, (byte)0x85, new byte[] { 0x85, 0x01, 0x7F });
            yield return new TestCaseData(TestEnum.Failure, (byte)0x86, new byte[] { 0x86, 0x02, 0x00, 0x80 });
            yield return new TestCaseData(@"Arek", (byte)0x87, new byte[] { 0x87, 0x04, 0x41, 0x72, 0x65, 0x6B });

            foreach(var testCase in GetIntegerDataTestCases())
            {
                yield return testCase;
            }
        }

        private static IEnumerable<TestCaseData> GetIntegerDataTestCases()
        {
            yield return new TestCaseData((BigInteger)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((int)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((uint)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((long)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((ulong)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((short)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((ushort)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((byte)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
            yield return new TestCaseData((sbyte)10, (byte)0x82, new byte[] { 0x82, 0x01, 0x0A });
        }

        [TestCaseSource(nameof(GetBuiltInPrimitiveDataTestCases))]
        public void Serialize_PrimitiveData_ShouldReturnExpectedEncodedValue(object value, byte tag, byte[] expectedEncodedValue)
        {
            // Arrange
            var asnTag = tag.ToAsn1Tag();

            // Act
            var actualEncodedValue = AsnSerializer.SerializeBer(value, asnTag);

            // Assert
            Assert.That(actualEncodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetBuiltInPrimitiveDataTestCases))]
        public void Deserialize_PrimitiveData_ShouldReturnExpectedValue(object expectedValue, byte expectedTag, byte[] encodedValue)
        {
            // Act
            var actualValue = AsnSerializer.DeserializeBer(encodedValue, expectedValue.GetType(), expectedTag.ToAsn1Tag());

            // Assert
            Assert.That(actualValue, Is.EqualTo(expectedValue));
        }

        private static IEnumerable<Type> GetIntegerOutOfRangeTestCases()
        {
            foreach (var integerType in GetIntegerDataTestCases()
                .Select(x => x.Arguments[0]!.GetType())
                .Where(x => x != typeof(BigInteger)))
            {
                yield return integerType;
            }
        }

        [TestCaseSource(nameof(GetIntegerOutOfRangeTestCases))]
        public void Deserialize_IntegerOutOfRange_ShouldThrowAsnConversionException(Type type)
        {
            // Arrange
            var encodedInteger = new byte[] { 0x02, 0x09, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

            // Act & Assert
            Assert.Throws<AsnConversionException>(() => AsnSerializer.DeserializeBer(encodedInteger, type));
        }
    }
}
