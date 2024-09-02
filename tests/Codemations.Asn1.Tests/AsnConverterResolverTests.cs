using Codemations.Asn1.Attributes;
using Codemations.Asn1.Converters;
using Codemations.Asn1.Types;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace Codemations.Asn1.Tests
{
    internal class AsnConverterResolverTests
    {
        private enum TestEnum {}
        private class TestSequenceClass
        {
            public byte PropertyWithoutConverter { get; set; }

            [AsnConverter(typeof(PropertyIntegerConverter))]
            public byte PropertyWithConverter { get; set; }
        }

        [AsnChoice]
        private class TestChoiceClass
        {
        }

        private class PropertyIntegerConverter : AsnIntegerConverter.Byte
        {
        }

        private class CustomIntegerConverter : AsnIntegerConverter.Byte
        {
        }

        private static IEnumerable<TestCaseData> GetBuiltInMappingTestCases()
        {
            // Integer
            yield return new TestCaseData(typeof(byte), typeof(AsnIntegerConverter.Byte), typeof(byte));
            yield return new TestCaseData(typeof(byte?), typeof(AsnIntegerConverter.Byte), typeof(byte));
            yield return new TestCaseData(typeof(sbyte), typeof(AsnIntegerConverter.SByte), typeof(sbyte));
            yield return new TestCaseData(typeof(sbyte?), typeof(AsnIntegerConverter.SByte), typeof(sbyte));
            yield return new TestCaseData(typeof(short), typeof(AsnIntegerConverter.Short), typeof(short));
            yield return new TestCaseData(typeof(short?), typeof(AsnIntegerConverter.Short), typeof(short));
            yield return new TestCaseData(typeof(ushort), typeof(AsnIntegerConverter.UShort), typeof(ushort));
            yield return new TestCaseData(typeof(ushort?), typeof(AsnIntegerConverter.UShort), typeof(ushort));
            yield return new TestCaseData(typeof(int), typeof(AsnIntegerConverter.Int32), typeof(int));
            yield return new TestCaseData(typeof(int?), typeof(AsnIntegerConverter.Int32), typeof(int));
            yield return new TestCaseData(typeof(uint), typeof(AsnIntegerConverter.UInt32), typeof(uint));
            yield return new TestCaseData(typeof(uint?), typeof(AsnIntegerConverter.UInt32), typeof(uint));
            yield return new TestCaseData(typeof(long), typeof(AsnIntegerConverter.Int64), typeof(long));
            yield return new TestCaseData(typeof(long?), typeof(AsnIntegerConverter.Int64), typeof(long));
            yield return new TestCaseData(typeof(ulong), typeof(AsnIntegerConverter.UInt64), typeof(ulong));
            yield return new TestCaseData(typeof(ulong?), typeof(AsnIntegerConverter.UInt64), typeof(ulong));
            yield return new TestCaseData(typeof(BigInteger), typeof(AsnIntegerConverter.Big), typeof(BigInteger));
            yield return new TestCaseData(typeof(BigInteger?), typeof(AsnIntegerConverter.Big), typeof(BigInteger));

            // Boolean
            yield return new TestCaseData(typeof(bool), typeof(AsnBooleanConverter), typeof(bool));
            yield return new TestCaseData(typeof(bool?), typeof(AsnBooleanConverter), typeof(bool));

            // Enumerated value
            yield return new TestCaseData(typeof(Enum), typeof(AsnEnumeratedValueConverter), typeof(Enum));
            yield return new TestCaseData(typeof(TestEnum), typeof(AsnEnumeratedValueConverter), typeof(TestEnum));
            yield return new TestCaseData(typeof(TestEnum?), typeof(AsnEnumeratedValueConverter), typeof(TestEnum));

            // Octet string
            yield return new TestCaseData(typeof(byte[]), typeof(AsnOctetStringConverter), typeof(byte[]));

            // Sequence
            yield return new TestCaseData(typeof(TestSequenceClass), typeof(AsnSequenceConverter), typeof(TestSequenceClass));
            
            // Choice
            yield return new TestCaseData(typeof(TestChoiceClass), typeof(AsnChoiceConverter), typeof(TestChoiceClass));

            // Sequence of
            yield return new TestCaseData(typeof(ICollection), typeof(AsnSequenceOfConverter), typeof(ICollection));
            yield return new TestCaseData(typeof(List<>), typeof(AsnSequenceOfConverter), typeof(List<>));
            yield return new TestCaseData(typeof(int[]), typeof(AsnSequenceOfConverter), typeof(int[]));
        }

        private static IEnumerable<TestCaseData> GetTypeSpecificMappingTestCases()
        {
            yield return new TestCaseData(typeof(AsnBmpString), typeof(AsnBmpStringConverter), typeof(AsnBmpString));
            yield return new TestCaseData(typeof(AsnBmpString?), typeof(AsnBmpStringConverter), typeof(AsnBmpString));

            yield return new TestCaseData(typeof(AsnIA5String), typeof(AsnIA5StringConverter), typeof(AsnIA5String));
            yield return new TestCaseData(typeof(AsnIA5String?), typeof(AsnIA5StringConverter), typeof(AsnIA5String));

            yield return new TestCaseData(typeof(AsnNumericString), typeof(AsnNumericStringConverter), typeof(AsnNumericString));
            yield return new TestCaseData(typeof(AsnNumericString?), typeof(AsnNumericStringConverter), typeof(AsnNumericString));

            yield return new TestCaseData(typeof(AsnPrintableString), typeof(AsnPrintableStringConverter), typeof(AsnPrintableString));
            yield return new TestCaseData(typeof(AsnPrintableString?), typeof(AsnPrintableStringConverter), typeof(AsnPrintableString));

            yield return new TestCaseData(typeof(AsnT61String), typeof(AsnT61StringConverter), typeof(AsnT61String));
            yield return new TestCaseData(typeof(AsnT61String?), typeof(AsnT61StringConverter), typeof(AsnT61String));

            yield return new TestCaseData(typeof(AsnUtf8String), typeof(AsnUtf8StringConverter), typeof(AsnUtf8String));
            yield return new TestCaseData(typeof(AsnUtf8String?), typeof(AsnUtf8StringConverter), typeof(AsnUtf8String));

            yield return new TestCaseData(typeof(AsnVisibleString), typeof(AsnVisibleStringConverter), typeof(AsnVisibleString));
            yield return new TestCaseData(typeof(AsnVisibleString?), typeof(AsnVisibleStringConverter), typeof(AsnVisibleString));

            yield return new TestCaseData(typeof(AsnOid), typeof(AsnOidConverter), typeof(AsnOid));
            yield return new TestCaseData(typeof(AsnOid?), typeof(AsnOidConverter), typeof(AsnOid));
        }

        private static Type GetNullableType(Type type)
        {
            return typeof(Nullable<>).MakeGenericType(type);
        }

        private static AsnPropertyInfo GetTestPropertyInfo<T>(string propertyName) where T : class
        {
            return typeof(T).GetProperty(propertyName)!;
        }

        [TestCaseSource(nameof(GetBuiltInMappingTestCases))]
        [TestCaseSource(nameof(GetTypeSpecificMappingTestCases))]
        public void Resolve_Type_ShouldReturnExpectedConverter(Type typeToResolve, Type expectedConverterType, Type expectedResolvedType)
        {
            // Arrange
            var convertersResolver = new AsnConverterResolver();

            // Act
            var converter = convertersResolver.Resolve(typeToResolve, out var resolvedType);

            // Assert
            Assert.Multiple(() =>
            {                
                Assert.That(converter.GetType(), Is.EqualTo(expectedConverterType));
                Assert.That(resolvedType, Is.EqualTo(expectedResolvedType));
            });
        }

        [Test]
        public void Resolve_AsnPropertyInfo_ShouldReturnConverterDefinedInAttribute()
        {
            // Arrange
            var asnPropertyInfo = GetTestPropertyInfo<TestSequenceClass>(nameof(TestSequenceClass.PropertyWithConverter));
            var convertersResolver = new AsnConverterResolver(new CustomIntegerConverter());
            var expectedResolvedType = typeof(byte);
            var expectedConverterType = typeof(PropertyIntegerConverter);

            // Act
            var converter = convertersResolver.Resolve(asnPropertyInfo, out var resolvedType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(converter.GetType(), Is.EqualTo(expectedConverterType));
                Assert.That(resolvedType, Is.EqualTo(expectedResolvedType));
            });
        }

        [Test]
        public void Resolve_AsnPropertyInfo_ShouldReturnBuiltInConverter()
        {
            // Arrange
            var asnPropertyInfo = GetTestPropertyInfo<TestSequenceClass>(nameof(TestSequenceClass.PropertyWithoutConverter));
            var convertersResolver = new AsnConverterResolver();
            var expectedResolvedType = typeof(byte);
            var expectedConverterType = typeof(AsnIntegerConverter.Byte);

            // Act
            var converter = convertersResolver.Resolve(asnPropertyInfo, out var resolvedType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(converter.GetType(), Is.EqualTo(expectedConverterType));
                Assert.That(resolvedType, Is.EqualTo(expectedResolvedType));
            });
        }

        [Test]
        public void Resolve_AsnPropertyInfo_ShouldReturnCustomConverter()
        {
            // Arrange
            var asnPropertyInfo = GetTestPropertyInfo<TestSequenceClass>(nameof(TestSequenceClass.PropertyWithoutConverter));
            var convertersResolver = new AsnConverterResolver(new CustomIntegerConverter());
            var expectedResolvedType = typeof(byte);
            var expectedConverterType = typeof(CustomIntegerConverter);

            // Act
            var converter = convertersResolver.Resolve(asnPropertyInfo, out var resolvedType);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(converter.GetType(), Is.EqualTo(expectedConverterType));
                Assert.That(resolvedType, Is.EqualTo(expectedResolvedType));
            });
        }

        [Test]
        public void Resolve_UnresolvalbleType_ShouldThrowArgumentException()
        {
            // Arrange
            var unresolvableType = typeof(object);
            var convertersResolver = new AsnConverterResolver();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => convertersResolver.Resolve(unresolvableType, out _));
        }
    }
}
