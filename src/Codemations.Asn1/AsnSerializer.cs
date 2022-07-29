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
            var converter = this.ConverterResolver.Resolve(type);
            var deserialized = converter.Read(reader, null, type, this.ConverterResolver);

            if (reader.HasData)
            {
                throw new AsnConversionException("Not read data left.");
            }

            return deserialized;
        }

        public byte[] Serialize(object element)
        {
            var writer = new AsnWriter(this.EncodingRules);
            var converter = this.ConverterResolver.Resolve(element.GetType());
            converter.Write(writer, null, element, this.ConverterResolver);
            return writer.Encode();
        }
    }
}