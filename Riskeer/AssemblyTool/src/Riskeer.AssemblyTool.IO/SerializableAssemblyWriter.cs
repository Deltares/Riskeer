// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.IO;
using System.Xml.Serialization;
using Core.Common.IO.Exceptions;
using Riskeer.AssemblyTool.IO.Model;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.AssemblyTool.IO
{
    /// <summary>
    /// Writer for saving instances of <see cref="SerializableAssembly"/> to a file.
    /// </summary>
    public static class SerializableAssemblyWriter
    {
        /// <summary>
        /// Writes a <see cref="SerializableAssembly"/> to a file.
        /// </summary>
        /// <param name="serializableAssembly">The <see cref="SerializableAssembly"/> to be written to the file.</param>
        /// <param name="filePath">The path to the file.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when unable to write to <paramref name="filePath"/>.</exception>
        public static void WriteAssembly(SerializableAssembly serializableAssembly, string filePath)
        {
            if (serializableAssembly == null)
            {
                throw new ArgumentNullException(nameof(serializableAssembly));
            }

            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            StreamWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(SerializableAssembly));
                writer = new StreamWriter(filePath);
                var xmlns = new XmlSerializerNamespaces();
                xmlns.Add(AssemblyXmlIdentifiers.GmlNamespaceIdentifier, AssemblyXmlIdentifiers.GmlNamespace);
                xmlns.Add(AssemblyXmlIdentifiers.AssemblyNamespaceIdentifier, AssemblyXmlIdentifiers.AssemblyNamespace);
                serializer.Serialize(writer, serializableAssembly, xmlns);
            }
            catch (SystemException e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilResources.Error_General_output_error_0, filePath), e);
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}