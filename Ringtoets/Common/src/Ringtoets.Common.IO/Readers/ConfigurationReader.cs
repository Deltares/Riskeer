// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Xml;
using System.Xml.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Readers
{
    /// <summary>
    /// Base class for reading a configuration from XML and creating a collection of corresponding <see cref="TCalculationItem"/>.
    /// </summary>
    /// <typeparam name="TCalculationItem">The type of calculation items read from XML.</typeparam>
    public abstract class ConfigurationReader<TCalculationItem>
    {
        protected readonly XDocument xmlDocument;

        /// <summary>
        /// Creates a new instance of <see cref="ConfigurationReader{TCalculationItem}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// </list>
        /// </exception>
        protected ConfigurationReader(string xmlFilePath)
        {
            IOUtils.ValidateFilePath(xmlFilePath);

            ValidateFileExists(xmlFilePath);

            xmlDocument = LoadDocument(xmlFilePath);
        }

        /// <summary>
        /// Validates whether a file exists at the provided <paramref name="xmlFilePath"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to validate.</param>
        /// <exception cref="CriticalFileReadException">Thrown when no existing file is found.</exception>
        private static void ValidateFileExists(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);

                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Loads an XML document from the provided <see cref="xmlFilePath"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to load the XML document from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the XML document cannot be loaded.</exception>
        private static XDocument LoadDocument(string xmlFilePath)
        {
            try
            {
                return XDocument.Load(xmlFilePath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo | LoadOptions.SetBaseUri);
            }
            catch (Exception exception)
                when (exception is InvalidOperationException
                      || exception is XmlException
                      || exception is IOException)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(CoreCommonUtilsResources.Error_General_IO_Import_ErrorMessage);

                throw new CriticalFileReadException(message, exception);
            }
        }
    }
}