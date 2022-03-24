using System;
using System.Formats.Asn1;

namespace Codemations.Asn1.TypeConverters
{
    internal class AsnSequenceConverter : AsnTypeConverter
    {
        internal override bool IsAccepted(Type type)
        {
            return type.IsClass;
        }

        public override object Read(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var value = Activator.CreateInstance(type)!;
            AsnConvert.Deserialize(reader.ReadSequence(tag), value);
            return value;
        }

        public override void Write(AsnWriter writer, Asn1Tag? tag, object value)
        {
            writer.PushSequence(tag);
            AsnConvert.Serialize(writer, value);
            writer.PopSequence(tag);
        }
    }
}