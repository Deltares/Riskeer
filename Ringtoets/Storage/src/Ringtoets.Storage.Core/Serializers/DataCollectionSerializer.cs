// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace Ringtoets.Storage.Core.Serializers
{
    /// <summary>
    /// Converter class that converts between a collection of <see cref="TSerializedData"/> and 
    /// an XML representation of that data.
    /// </summary>
    internal abstract class DataCollectionSerializer<TData, TSerializedData>
        where TSerializedData : class
    {
        private static readonly Type serializationRootType = typeof(TSerializedData[]);
        private readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// Converts the collection of <see cref="TData"/> to XML data.
        /// </summary>
        /// <param name="elements">The elements to be serialized.</param>
        /// <returns>The XML data.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="elements"/> is <c>null</c>.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="InvalidDataContractException">Thrown when <see cref="TSerializedData"/> does not conform to data contract rules.
        /// E.g., the <see cref="DataContractAttribute"/> has not been applied to the <see cref="TSerializedData"/>.</exception>
        /// <exception cref="SerializationException">Thrown when an error occurs during serialization.</exception>
        public string ToXml(IEnumerable<TData> elements)
        {
            if (elements == null)
            {
                throw new ArgumentNullException(nameof(elements));
            }

            var memoryStream = new MemoryStream();

            using (XmlDictionaryWriter writer = XmlDictionaryWriter.CreateTextWriter(memoryStream, encoding, false))
            {
                var formatter = new DataContractSerializer(serializationRootType);
                formatter.WriteObject(writer, ToSerializableData(elements));
                writer.Flush();
            }

            using (var streamReader = new StreamReader(memoryStream))
            {
                memoryStream.Seek(0, SeekOrigin.Begin);
                return streamReader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts XML to an array of<see cref="TData"/>.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <returns>An array of <see cref="TData"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xml"/> is <c>null</c> or empty.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs.</exception>
        /// <exception cref="SerializationException">Thrown when an error occurs during deserialization.</exception>
        public TData[] FromXml(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentException(@"xml cannot be empty.", nameof(xml));
            }

            var stream = new MemoryStream();
            using (var streamWriter = new StreamWriter(stream, encoding))
            {
                streamWriter.Write(xml);
                streamWriter.Flush();

                stream.Seek(0, SeekOrigin.Begin);
                using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(stream, XmlDictionaryReaderQuotas.Max))
                {
                    var serializer = new DataContractSerializer(serializationRootType);
                    return FromSerializableData((TSerializedData[]) serializer.ReadObject(reader));
                }
            }
        }

        /// <summary>
        /// Converts <paramref name="elements"/> to the serializable representation.
        /// </summary>
        /// <param name="elements">The elements to convert.</param>
        /// <returns>The serialized elements.</returns>
        protected abstract TSerializedData[] ToSerializableData(IEnumerable<TData> elements);

        /// <summary>
        /// Converts the <paramref name="serializedElements"/> to <see cref="TData"/>.
        /// </summary>
        /// <param name="serializedElements">The serialized data to convert.</param>
        /// <returns>The deserialized elements</returns>
        protected abstract TData[] FromSerializableData(IEnumerable<TSerializedData> serializedElements);
    }
}