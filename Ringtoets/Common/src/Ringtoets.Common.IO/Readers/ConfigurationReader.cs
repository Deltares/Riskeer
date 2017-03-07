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
using Ringtoets.Common.IO.Properties;
using Ringtoets.Common.IO.Schema;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Readers
{
    /// <summary>
    /// Base class for reading a configuration from XML and creating a collection of corresponding <see cref="TCalculationItem"/>.
    /// </summary>
    /// <typeparam name="TCalculationItem">The type of calculation items read from XML.</typeparam>
    public abstract class ConfigurationReader<TCalculationItem>
        where TCalculationItem : IReadCalculationItem
    {
        private readonly XDocument xmlDocument;

        /// <summary>
        /// Creates a new instance of <see cref="ConfigurationReader{TCalculationItem}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="schemaResXFileRef">A file reference towards an XML Schema Definition (XSD).</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xmlFilePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// </list>
        /// </exception>
        protected ConfigurationReader(string xmlFilePath, string schemaResXFileRef)
        {
            IOUtils.ValidateFilePath(xmlFilePath);

            ValidateFileExists(xmlFilePath);

            xmlDocument = LoadDocument(xmlFilePath);

            ValidateToSchema(xmlDocument, schemaResXFileRef, xmlFilePath);

            ValidateNotEmpty(xmlDocument, xmlFilePath);
        }

        /// <summary>
        /// Reads the configuration from the XML and creates a collection of corresponding <see cref="IReadCalculationItem"/>.
        /// </summary>
        /// <returns>A collection of read <see cref="IReadCalculationItem"/>.</returns>
        public IEnumerable<IReadCalculationItem> Read()
        {
            return ParseReadCalculationItems(xmlDocument.Root?.Elements());
        }

        /// <summary>
        /// Parses a read calculation element.
        /// </summary>
        /// <param name="calculationElement">The read calculation element to parse.</param>
        /// <returns>A parsed <see cref="TCalculationItem"/>.</returns>
        protected abstract TCalculationItem ParseReadCalculation(XElement calculationElement);

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

        /// <summary>
        /// Validates the provided XML document based on the provided XML Schema Definition (XSD).
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <param name="schemaResXFileRef">A file reference towards the XML Schema Definition (XSD) to use for the validation.</param>
        /// <param name="xmlFilePath">The file path the XML document is loaded from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not match the provided XML Schema Definition (XSD).</exception>
        private static void ValidateToSchema(XDocument document, string schemaResXFileRef, string xmlFilePath)
        {
            var xmlSchemaSet = new XmlSchemaSet();
            xmlSchemaSet.Add(XmlSchema.Read(new StringReader(schemaResXFileRef), null));

            try
            {
                document.Validate(xmlSchemaSet, null);
            }
            catch (XmlSchemaValidationException exception)
            {
                string message = string.Format(Resources.ConfigurationReader_Configuration_contains_no_valid_xml_line_0_position_1_reason_2,
                                               exception.LineNumber,
                                               exception.LinePosition,
                                               exception.Message);

                throw new CriticalFileReadException(new FileReaderErrorMessageBuilder(xmlFilePath).Build(message), exception);
            }
        }

        /// <summary>
        /// Validates whether or not the provided XML document is empty.
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <param name="xmlFilePath">The file path the XML document is loaded from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not contain calculation items.</exception>
        private static void ValidateNotEmpty(XDocument document, string xmlFilePath)
        {
            if (!document.Descendants()
                         .Any(d => d.Name == ConfigurationSchemaIdentifiers.CalculationElement
                                   || d.Name == ConfigurationSchemaIdentifiers.FolderElement))
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(Resources.ConfigurationReader_No_calculation_items_found);

                throw new CriticalFileReadException(message);
            }
        }

        private IEnumerable<IReadCalculationItem> ParseReadCalculationItems(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements)
            {
                if (element.Name == ConfigurationSchemaIdentifiers.CalculationElement)
                {
                    yield return ParseReadCalculation(element);
                }

                if (element.Name == ConfigurationSchemaIdentifiers.FolderElement)
                {
                    yield return ParseReadCalculationGroup(element);
                }
            }
        }

        private ReadCalculationGroup ParseReadCalculationGroup(XElement folderElement)
        {
            return new ReadCalculationGroup(folderElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                                            ParseReadCalculationItems(folderElement.Elements()));
        }
    }
}