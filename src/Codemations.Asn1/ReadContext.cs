using System;
using System.Formats.Asn1;

namespace Codemations.Asn1
{
    //public class AsnReaderWrapper
    //{
    //    private Stack<AsnReader> readerStack = new ();
    //    private Stack<
    //        Type> typeStack = new ();

    //    public AsnReaderWrapper(AsnReader reader, Type type)
    //    {
    //        readerStack.Push(reader);
    //        typeStack.Push(type);

    //        if (type.IsClass)
    //        {

    //        }
    //    }

    //    private void HandleSequence(Asn1Tag? tag, Type type)
    //    {
    //        var sequenceReader = readerStack.Peek().ReadSequence(tag);
    //        readerStack.Push(sequenceReader);

    //        var item = Activator.CreateInstance(type) ?? throw new AsnConversionException("Failed to create object.");

    //        foreach (var propertyInfo in AsnHelper.GetAsnProperties(type))
    //        {


    //                var converter = converterResolver.Resolve(propertyInfo.Type);
    //                var value = converter.Read(sequenceReader, propertyInfo.Tag, propertyInfo.Type, converterResolver);
    //                propertyInfo.SetValue(item, value);
    //            }
    //            catch (AsnContentException e)
    //            {
    //                if (!propertyInfo.IsOptional)
    //                {
    //                    throw new AsnConversionException("Value for required element is missing.", propertyInfo.Tag, e);
    //                }
    //            }
    //        }

    //        return item;
    //    }
    //}


    public readonly struct ReadContext
    {
        public AsnReader AsnReader { get; }
        public object? ParentObject { get; }
        public Type ParentType { get; }
        public IAsnConverterResolver ConverterResolver { get; }

        public ReadContext(AsnReader asnReader, object? parentObject, Type parentType, IAsnConverterResolver converterResolver)
        {
            AsnReader = asnReader;
            ParentObject = parentObject;
            ParentType = parentType;
            ConverterResolver = converterResolver;
        }
        public void Deconstruct(out AsnReader asnReader, out object? parentObject, out Type parentType, out IAsnConverterResolver converterResolver)
        {
            asnReader = AsnReader;
            parentObject = ParentObject;
            parentType = ParentType;
            converterResolver = ConverterResolver;
        }
    }
}