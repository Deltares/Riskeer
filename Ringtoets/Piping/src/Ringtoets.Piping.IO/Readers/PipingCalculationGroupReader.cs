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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Common.Utils.Reflection;
using Ringtoets.Piping.IO.Properties;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping configuration from XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
    /// </summary>
    public class PipingCalculationGroupReader
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationGroupReader"/>.
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
        public PipingCalculationGroupReader(string xmlFilePath)
        {
            IOUtils.ValidateFilePath(xmlFilePath);

            ValidateFileExists(xmlFilePath);

            XDocument xmlDocument = LoadDocument(xmlFilePath);

            ValidateToSchema(xmlDocument);
        }

        /// <summary>
        /// Reads the piping configuration from the XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
        /// </summary>
        /// <returns>A collection of read <see cref="IReadPipingCalculationItem"/>.</returns>
        public IEnumerable<IReadPipingCalculationItem> Read()
        {
            return Enumerable.Empty<IReadPipingCalculationItem>();
        }

        /// <summary>
        /// Validates whether a file exists at the provided <paramref name="xmlFilePath"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to validate.</param>
        /// <exception cref="CriticalFileReadException">Thrown when no file is found.</exception>
        private static void ValidateFileExists(string xmlFilePath)
        {
            if (!File.Exists(xmlFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath).Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Loads a XML document from the provided <see cref="xmlFilePath"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to load the XML document from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the XML document cannot be loaded.</exception>
        private static XDocument LoadDocument(string xmlFilePath)
        {
            try
            {
                return XDocument.Load(xmlFilePath);
            }
            catch (Exception exception)
                when (exception is ArgumentNullException
                      || exception is XmlException
                      || exception is InvalidOperationException)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath).Build(CoreCommonUtilsResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Validates the provided XML document based on a predefined XML Schema Definition (XSD).
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not match the predefined XML Schema Definition (XSD).</exception>
        private static void ValidateToSchema(XDocument document)
        {
            XmlSchemaSet schema = LoadXmlSchema();

            try
            {
                document.Validate(schema, null);
            }
            catch (XmlSchemaValidationException e)
            {
                throw new CriticalFileReadException(Resources.PipingCalculationGroupReader_Configuration_contains_no_valid_xml, e);
            }
        }

        private static XmlSchemaSet LoadXmlSchema()
        {
            Stream schemaFile = AssemblyUtils.GetAssemblyResourceStream(typeof(PipingCalculationGroupReader).Assembly,
                                                                        "Ringtoets.Piping.IO.Readers.XMLPipingConfigurationSchema.xsd");

            var xmlSchema = new XmlSchemaSet();
            xmlSchema.Add(XmlSchema.Read(schemaFile, null));

            return xmlSchema;
        }
    }
}