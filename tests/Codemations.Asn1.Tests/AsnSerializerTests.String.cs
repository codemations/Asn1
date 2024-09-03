using Codemations.Asn1.Converters;
using NUnit.Framework;
using System.Collections.Generic;
using System.Formats.Asn1;

namespace Codemations.Asn1.Tests
{
    internal partial class AsnSerializerTests
    {
        private static IEnumerable<TestCaseData> GetStringTestCases()
        {
            yield return new TestCaseData(@"Arek", new byte[] { 0x0C, 0x04, 0x41, 0x72, 0x65, 0x6B }, new AsnUtf8StringConverter());
            yield return new TestCaseData(@"Arek", new byte[] { 0x16, 0x04, 0x41, 0x72, 0x65, 0x6B }, new AsnIA5StringConverter());
            yield return new TestCaseData(@"Arek", new byte[] { 0x1A, 0x04, 0x41, 0x72, 0x65, 0x6B }, new AsnVisibleStringConverter());
            yield return new TestCaseData(@"Arek", new byte[] { 0x13, 0x04, 0x41, 0x72, 0x65, 0x6B }, new AsnPrintableStringConverter());
            yield return new TestCaseData(@"Arek", new byte[] { 0x14, 0x04, 0x41, 0x72, 0x65, 0x6B }, new AsnT61StringConverter());
            yield return new TestCaseData(@"Arek", new byte[] { 0x1E, 0x08, 0x00, 0x41, 0x00, 0x72, 0x00, 0x65, 0x00, 0x6B }, new AsnBmpStringConverter());
            yield return new TestCaseData(@"1234", new byte[] { 0x12, 0x04, 0x31, 0x32, 0x33, 0x34 }, new AsnNumericStringConverter());
        }

        [TestCaseSource(nameof(GetStringTestCases))]
        public void Serialize_StringData_ShouldReturnExpectedEncodedValue(object value, byte[] expectedEncodedValue, IAsnConverter converter)
        {
            // Arrange
            var writer = new AsnWriter(AsnEncodingRules.BER);
            var serializer = new AsnSerializer();

            // Act
            serializer.Serialize(writer, value, converter);
            var encodedValue = writer.Encode();

            // Assert
            Assert.That(encodedValue, Is.EqualTo(expectedEncodedValue));
        }

        [TestCaseSource(nameof(GetStringTestCases))]
        public void Deserialize_StringData_ShouldReturnExpectedValue(object expectedValue, byte[] encodedValue, IAsnConverter converter)
        {
            // Arrange
            var reader = new AsnReader(encodedValue, AsnEncodingRules.BER);
            var serializer = new AsnSerializer();

            // Act
            var value = serializer.Deserialize(reader, expectedValue.GetType(), converter);

            // Assert
            Assert.That(value, Is.EqualTo(expectedValue));
        }
    }
}
