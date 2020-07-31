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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Riskeer.Common.IO.Properties;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Riskeer.Common.IO.Configurations.Import
{
    /// <summary>
    /// Base class for reading a calculation configuration from XML and creating a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="TReadCalculation"/>.
    /// </summary>
    /// <typeparam name="TReadCalculation">The type of calculation items read from XML.</typeparam>
    public abstract class CalculationConfigurationReader<TReadCalculation>
        where TReadCalculation : IConfigurationItem
    {
        private const string defaultSchemaName = "ConfiguratieSchema.xsd";

        private readonly XDocument xmlDocument;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationReader{TReadCalculation}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="schemaDefinitions">The <see cref="CalculationConfigurationSchemaDefinition"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nestedSchemaDefinitions"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> is invalid.</item>
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        protected CalculationConfigurationReader(string xmlFilePath, IEnumerable<CalculationConfigurationSchemaDefinition> schemaDefinitions)
        {
            IOUtils.ValidateFilePath(xmlFilePath);

            ValidateFileExists(xmlFilePath);

            xmlDocument = LoadDocument(xmlFilePath);

            // Versie check XSD Versie 2 | Full XSD Versie 2 (inclusief nested) | Versie 1 naar Versie 2 XSLT
            // Versie check XSD Versie 1 | Full XSD Versie 1 (inclusief nested) | Versie 0 naar Versie 1 XSLT
            // Versie check XSD Versie 0 | Full XSD Versie 0 (inclusief nested) | -
            
            // < xs:element name = "configuratie"  >
            //    
            //      attribute versie op 2
            //
            //
            //     </ xs:element >

            CalculationConfigurationSchemaDefinition matchingSchemaDefinition = GetMatchingSchemaDefinition(schemaDefinitions);
            if (matchingSchemaDefinition == null)
            {
                return;
            }

            ValidateToSchema(xmlDocument, xmlFilePath, matchingSchemaDefinition.MainSchemaDefinition, matchingSchemaDefinition.NestedSchemaDefinitions);
            
            ValidateNotEmpty(xmlDocument, xmlFilePath);

            // Migrate
            // ...
        }

        private CalculationConfigurationSchemaDefinition GetMatchingSchemaDefinition(IEnumerable<CalculationConfigurationSchemaDefinition> schemaDefinitions)
        {
            for (var i = 0; i < schemaDefinitions.Count(); i++)
            {
                try
                {
                    CalculationConfigurationSchemaDefinition current = schemaDefinitions.ElementAt(i);

                    var combinedXmlSchemaDefinition = new CombinedXmlSchemaDefinition(current.VersionSchemaDefinition, new Dictionary<string, string>());

                    combinedXmlSchemaDefinition.Validate(xmlDocument);

                    return current;
                }
                catch (XmlSchemaValidationException)
                {
                    // Do nothing and continue search
                }
            }

            return null;
        }

        /// <summary>
        /// Reads the calculation configuration from the XML and creates a collection of corresponding <see cref="IConfigurationItem"/>.
        /// </summary>
        /// <returns>A collection of read <see cref="IConfigurationItem"/>.</returns>
        public IEnumerable<IConfigurationItem> Read()
        {
            return ParseElements(xmlDocument.Root?.Elements());
        }

        /// <summary>
        /// Parses a read calculation element.
        /// </summary>
        /// <param name="calculationElement">The read calculation element to parse.</param>
        /// <returns>A parsed <see cref="TReadCalculation"/>.</returns>
        protected abstract TReadCalculation ParseCalculationElement(XElement calculationElement);

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
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);

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
            catch (XmlException exception)
            {
                string exceptionMessage = string.Format(Resources.CalculationConfigurationReader_Configuration_contains_no_valid_xml_Reason_0_,
                                                        exception.Message);

                throw new CriticalFileReadException(new FileReaderErrorMessageBuilder(xmlFilePath).Build(exceptionMessage), exception);
            }
            catch (Exception exception)
                when (exception is InvalidOperationException || exception is IOException)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(CoreCommonUtilResources.Error_General_IO_Import_ErrorMessage);

                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Validates the provided XML document based on the provided schema definitions.
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <param name="xmlFilePath">The file path the XML document is loaded from.</param>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <param name="nestedSchemaDefinitions">A <see cref="IDictionary{TKey,TValue}"/> containing
        /// zero to more nested schema definitions</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not match
        /// the provided schema definitions.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mainSchemaDefinition"/> does not
        /// reference the default schema definition <c>ConfiguratieSchema.xsd</c>.</exception>
        private static void ValidateToSchema(XDocument document, string xmlFilePath, string mainSchemaDefinition,
                                             IDictionary<string, string> nestedSchemaDefinitions)
        {
            if (!mainSchemaDefinition.Contains(defaultSchemaName))
            {
                throw new ArgumentException($"'{nameof(mainSchemaDefinition)}' does not reference the default schema '{defaultSchemaName}'.");
            }

            IDictionary<string, string> extendedNestedSchemaDefinitions = new Dictionary<string, string>(nestedSchemaDefinitions);
            extendedNestedSchemaDefinitions.Add(defaultSchemaName, Resources.ConfiguratieSchema);

            var combinedXmlSchemaDefinition = new CombinedXmlSchemaDefinition(mainSchemaDefinition, extendedNestedSchemaDefinitions);

            try
            {
                combinedXmlSchemaDefinition.Validate(document);
            }
            catch (XmlSchemaValidationException exception)
            {
                string message = string.Format(Resources.CalculationConfigurationReader_Configuration_contains_no_valid_xml_line_0_position_1_reason_2,
                                               exception.LineNumber,
                                               exception.LinePosition,
                                               exception.Message);

                throw new CriticalFileReadException(new FileReaderErrorMessageBuilder(xmlFilePath).Build(message), exception);
            }
        }

        /// <summary>
        /// Validates whether the provided XML document is not empty.
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <param name="xmlFilePath">The file path the XML document is loaded from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not contain configuration items.</exception>
        private static void ValidateNotEmpty(XDocument document, string xmlFilePath)
        {
            if (!document.Descendants()
                         .Any(d => d.Name == ConfigurationSchemaIdentifiers.CalculationElement
                                   || d.Name == ConfigurationSchemaIdentifiers.FolderElement))
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(Resources.CalculationConfigurationReader_No_configuration_items_found);

                throw new CriticalFileReadException(message);
            }
        }

        private IEnumerable<IConfigurationItem> ParseElements(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements)
            {
                if (element.Name == ConfigurationSchemaIdentifiers.CalculationElement)
                {
                    yield return ParseCalculationElement(element);
                }

                if (element.Name == ConfigurationSchemaIdentifiers.FolderElement)
                {
                    yield return ParseFolderElement(element);
                }
            }
        }

        private CalculationGroupConfiguration ParseFolderElement(XElement folderElement)
        {
            return new CalculationGroupConfiguration(folderElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                                                     ParseElements(folderElement.Elements()));
        }
    }
}