using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Application.Ringtoets.Storage.Serializers
{
    internal abstract class SimpleDataCollectionSerializer<TData, TSerializedData>
    {
        private static readonly Type serializationRootType = typeof(TSerializedData[]);
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Converts the collection of <see cref="TData"/> to XML data.
        /// </summary>
        /// <param name="elements">The elements to be serialized.</param>
        /// <returns>The XML data.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="elements"/> is null.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="SerializationException">An error occurs during serialization.</exception>
        public string ToXml(IEnumerable<TData> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            using (MemoryStream memoryStream = new MemoryStream())
            using (var writer = XmlDictionaryWriter.CreateTextWriter(memoryStream, encoding, false))
            using (var streamReader = new StreamReader(memoryStream))
            {
                var formatter = new DataContractSerializer(serializationRootType);
                formatter.WriteObject(writer, ToSerializableData(elements));
                writer.Flush();

                memoryStream.Seek(0, SeekOrigin.Begin);
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts XML to an array of<see cref="TData"/>.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>An array of <see cref="TData"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="xml"/> is null.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        /// <exception cref="SerializationException">An error occurs during deserialization.</exception>
        public TData[] FromXml(string xml)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            using (var stream = new MemoryStream())
            using (var streamWriter = new StreamWriter(stream, encoding))
            {
                streamWriter.Write(xml);
                streamWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                using (var writer = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    var serializer = new DataContractSerializer(serializationRootType);
                    return FromSerializableData((TSerializedData[])serializer.ReadObject(writer));
                }
            }
        }

        protected abstract TSerializedData[] ToSerializableData(IEnumerable<TData> elements);

        protected abstract TData[] FromSerializableData(IEnumerable<TSerializedData> serializedElements);
    }
}