using System;
using System.Collections;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Linq;

namespace Codemations.Asn1.Converters
{
    internal class AsnSequenceOfConverter : AsnConstructedConverter
    {
        public AsnSequenceOfConverter(AsnConverterFactory converterFactory) : base(converterFactory)
        {
        }

        public override bool IsAccepted(Type type)
        {
            return type != typeof(string) && type != typeof(byte[]) && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var genericArgType = type.GetGenericArguments().Single();
            var sequenceReader = reader.ReadSequence(tag);
            var sequence = (Activator.CreateInstance(typeof(List<>).MakeGenericType(genericArgType)) as IList)!;
            while (sequenceReader.HasData)
            {
                var converter = new AsnConverterFactory().CreateElementConverter(genericArgType);
                sequence.Add(converter.Read(sequenceReader, null, genericArgType));
            }
            return sequence;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.PushSequence(tag);
            foreach (var element in (value as IEnumerable)!)
            {
                var converter = new AsnConverterFactory().CreateElementConverter(element.GetType());
                converter.Write(writer, null, element);
            }
            writer.PopSequence(tag);
        }
    }
}