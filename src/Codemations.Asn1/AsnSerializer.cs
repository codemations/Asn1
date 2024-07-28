using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    public class AsnSerializer
    {
        public AsnEncodingRules EncodingRules { get; }
        public AsnReaderOptions ReaderOptions { get; }
        public IAsnConverterResolver ConverterResolver { get; }

        public AsnSerializer(AsnEncodingRules encodingRules, AsnReaderOptions readerOptions = default) : 
            this(encodingRules, readerOptions, new DefaultConverterResolver())
        {
        }

        public AsnSerializer(AsnEncodingRules encodingRules, AsnReaderOptions readerOptions, IAsnConverterResolver converterResolver)
        {
            this.EncodingRules = encodingRules;
            this.ReaderOptions = readerOptions;
            this.ConverterResolver = converterResolver;
        }

        public object Deserialize(ReadOnlyMemory<byte> data, Type type)
        {
            var reader = new AsnReader(data, this.EncodingRules, this.ReaderOptions);
            var deserialized = Deserialize(reader, null, type);

            if (reader.HasData)
            {
                throw new AsnConversionException("Not read data left.");
            }

            return deserialized;
        }

        public object Deserialize(AsnReader reader, Asn1Tag? tag, Type type)
        {
            var converter = this.ConverterResolver.Resolve(type);
            return converter.Read(reader, tag, type, this);
        }

        public byte[] Serialize(object value)
        {
            var writer = new AsnWriter(this.EncodingRules);
            Serialize(writer, null, value);
            return writer.Encode();
        }

        public void Serialize(AsnWriter writer, Asn1Tag? tag, object value)
        {
            var converter = this.ConverterResolver.Resolve(value.GetType());
            converter.Write(writer, tag, value, this);
        }
    }
}