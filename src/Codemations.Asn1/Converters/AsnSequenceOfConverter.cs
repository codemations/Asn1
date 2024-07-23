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
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public object Read(AsnReader reader, Asn1Tag? tag, Type type, IAsnConverterResolver converterResolver)
        {
            var genericArgType = type.GetGenericArguments().Single();
            var sequenceReader = reader.ReadSequence(tag);
            var sequence = (Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArgType)) as IList)!;
            while (sequenceReader.HasData)
            {
                var converter = converterResolver.Resolve(genericArgType);
                sequence.Add(converter.Read(sequenceReader, null, genericArgType, converterResolver));
            }
            return sequence;
        }

        public void Write(AsnWriter writer, Asn1Tag? tag, object value, IAsnConverterResolver converterResolver)
        {
            writer.PushSequence(tag);
            IAsnConverter? converter = null;
            foreach (var element in (value as IEnumerable)!)
            {
                converter ??= converterResolver.Resolve(element.GetType());
                converter.Write(writer, null, element, converterResolver);
            }
            writer.PopSequence(tag);
        }
    }
}