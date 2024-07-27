using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceOfConverter : IAsnConverter
    {
        public bool CanConvert(Type type)
        {
            return typeof(ICollection).IsAssignableFrom(type);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            var elementType = GetElementType(type);
            var collection = ReadCollection(reader, tag, elementType, converterResolver);

            return type switch
            {
                _ when IsList(type) => BuildList(type, collection),              
                _ when type.IsArray => BuildArray(type, collection),
                _ => throw new NotSupportedException()
            };
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            using var sequenceScope = writer.PushSequence(tag);
            var elementType = GetElementType(value.GetType());
            var converter = converterResolver.Resolve(elementType);
            foreach (var element in (value as ICollection)!)
            {
                converter.Write(writer, null, element, converterResolver);
            }
        }

        private IEnumerable<object> ReadCollection(AsnReader reader, Asn1Tag? tag, Type elementType, IAsnConverterResolver converterResolver)
        {
            var sequenceReader = reader.ReadSequence(tag);
            var converter = converterResolver.Resolve(elementType);

            while (sequenceReader.HasData)
            {
                yield return converter.Read(sequenceReader, null, elementType, converterResolver);
            }
        }

        private bool IsList(Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        private object BuildList(Type type, IEnumerable<object> elements)
        {
            var list = type.CreateInstance<IList>();
            foreach (var element in elements)
            {
                list.Add(element);

            }
            return list;
        }

        private object BuildArray(Type type, IEnumerable<object> elements)
        {
            var srcArray = elements.ToArray();
            var dstArray = type.CreateInstance<Array>(srcArray.Length);
            srcArray.CopyTo(dstArray, 0);
            return dstArray;
        }

        private Type GetElementType(Type collectionType)
        {
            return collectionType switch
            {
                { IsGenericType: true } => collectionType.GetGenericArguments()[0],
                { IsArray: true } => collectionType.GetElementType()!,
                _ => throw new NotSupportedException()
            };
        }
    }
}