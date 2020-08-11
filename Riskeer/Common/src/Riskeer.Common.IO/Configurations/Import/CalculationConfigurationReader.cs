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
        private readonly string xmlFilePath;

        private XDocument xmlDocument;

        /// <summary>
        /// Creates a new instance of <see cref="CalculationConfigurationReader{TReadCalculation}"/>.
        /// </summary>
        /// <param name="xmlFilePath">The file path to the XML file.</param>
        /// <param name="schemaDefinitions">The array of <see cref="CalculationConfigurationSchemaDefinition"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="schemaDefinitions"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> is invalid.</item>
        /// <item>the main schema definition of <see cref="CalculationConfigurationSchemaDefinition"/> is invalid.</item>
        /// <item>the nested schema definitions of <see cref="CalculationConfigurationSchemaDefinition"/> contains invalid schema definition values.</item>
        /// <item>the main schema definition of <see cref="CalculationConfigurationSchemaDefinition"/>, all together with its referenced
        /// nested schema definitions, contains an invalid schema definition.</item>
        /// <item>nested schema definitions of <see cref="CalculationConfigurationSchemaDefinition"/> contains schema definitions that are not
        /// referenced by the main schema definition.</item>
        /// <item>the main schema definition of <see cref="CalculationConfigurationSchemaDefinition"/> does not reference the default schema definition
        /// <c>ConfiguratieSchema.xsd</c>.</item>        
        /// </list>
        /// </exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="xmlFilePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// <item>something goes wrong while migrating.</item>
        /// </list>
        /// </exception>
        protected CalculationConfigurationReader(string xmlFilePath, CalculationConfigurationSchemaDefinition[] schemaDefinitions)
        {
            if (schemaDefinitions == null)
            {
                throw new ArgumentNullException(nameof(schemaDefinitions));
            }

            IOUtils.ValidateFilePath(xmlFilePath);

            this.xmlFilePath = xmlFilePath;

            ValidateFileExists();

            xmlDocument = LoadDocument();

            CalculationConfigurationSchemaDefinition schemaDefinition = GetSchemaDefinition(schemaDefinitions);

            ValidateToSchema(schemaDefinition.MainSchemaDefinition, schemaDefinition.NestedSchemaDefinitions);

            ValidateNotEmpty();

            MigrateWhenNeeded(schemaDefinitions, schemaDefinition);
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
        /// Gets the correct schema definition depending on the version.
        /// </summary>
        /// <param name="schemaDefinitions">All the schema definitions.</param>
        /// <returns>The schema definition that corresponds to the XML file.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when the version
        /// from the XML file is not supported.</exception>
        private CalculationConfigurationSchemaDefinition GetSchemaDefinition(IEnumerable<CalculationConfigurationSchemaDefinition> schemaDefinitions)
        {
            int versionNumber;

            try
            {
                var combinedXmlSchemaDefinition = new CombinedXmlSchemaDefinition(Resources.VersieSchema, new Dictionary<string, string>());
                combinedXmlSchemaDefinition.Validate(xmlDocument);

                versionNumber = GetVersionNumber();
            }
            catch (XmlSchemaValidationException)
            {
                versionNumber = 0;
            }

            CalculationConfigurationSchemaDefinition schemaDefinition = schemaDefinitions.SingleOrDefault(sd => sd.VersionNumber == versionNumber);

            if (schemaDefinition == null)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(Resources.CalculationConfigurationReader_GetSchemaDefinition_Not_supported_version);

                throw new CriticalFileReadException(message);
            }

            return schemaDefinition;
        }

        private int GetVersionNumber()
        {
            string versionNumberString = xmlDocument.Element(ConfigurationSchemaIdentifiers.ConfigurationElement).Attribute(ConfigurationSchemaIdentifiers.VersionAttribute).Value;
            int versionNumber = int.Parse(versionNumberString);
            return versionNumber;
        }

        /// <summary>
        /// Migrates the XML document to newer versions when needed.
        /// </summary>
        /// <param name="schemaDefinitions">All the schema definitions.</param>
        /// <param name="schemaDefinition">The schema definition corresponding to the version
        /// of the XML document.</param>
        /// <exception cref="CriticalFileReadException">Thrown when something goes wrong
        /// while migrating.</exception>
        private void MigrateWhenNeeded(CalculationConfigurationSchemaDefinition[] schemaDefinitions, CalculationConfigurationSchemaDefinition schemaDefinition)
        {
            int index = Array.IndexOf(schemaDefinitions, schemaDefinition);

            for (int i = index + 1; i < schemaDefinitions.Length; i++)
            {
                MigrateToNewSchema(schemaDefinitions[i].MigrationScript);
            }
        }

        /// <summary>
        /// Migrates the <see cref="xmlDocument"/> with the given <paramref name="migrationScript"/>.
        /// </summary>
        /// <param name="migrationScript">The script to perform the migration with.</param>
        /// <exception cref="CriticalFileReadException">Thrown when something goes wrong
        /// while migrating.</exception>
        private void MigrateToNewSchema(string migrationScript)
        {
            try
            {
                xmlDocument = CalculationConfigurationMigrator.Migrate(xmlDocument, migrationScript);
            }
            catch (CalculationConfigurationMigrationException e)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(Resources.CalculationConfigurationReader_MigrateToNewSchema_An_unexpected_error_occurred);

                throw new CriticalFileReadException(message, e);
            }
        }

        /// <summary>
        /// Validates whether a file exists at the <see cref="xmlFilePath"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when no existing file is found.</exception>
        private void ValidateFileExists()
        {
            if (!File.Exists(xmlFilePath))
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);

                throw new CriticalFileReadException(message);
            }
        }

        /// <summary>
        /// Loads an XML document from the <see cref="xmlFilePath"/>.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the XML document cannot be loaded.</exception>
        private XDocument LoadDocument()
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
        /// Validates the XML document based on the provided schema definitions.
        /// </summary>
        /// <param name="mainSchemaDefinition">A <c>string</c> representing the main schema definition.</param>
        /// <param name="nestedSchemaDefinitions">A <see cref="IDictionary{TKey,TValue}"/> containing
        /// zero to more nested schema definitions.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the XML document does not match
        /// the provided schema definitions.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="mainSchemaDefinition"/> does not
        /// reference the default schema definition <c>ConfiguratieSchema.xsd</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="mainSchemaDefinition"/> is invalid.</item>
        /// <item><paramref name="nestedSchemaDefinitions"/> contains invalid schema definition values.</item>
        /// <item><paramref name="mainSchemaDefinition"/>, all together with its referenced
        /// <paramref name="nestedSchemaDefinitions"/>, contains an invalid schema definition.</item>
        /// <item><paramref name="nestedSchemaDefinitions"/> contains schema definitions that are not
        /// referenced by <see cref="mainSchemaDefinition"/>.</item>
        /// </list>
        /// </exception>

        private void ValidateToSchema(string mainSchemaDefinition, IDictionary<string, string> nestedSchemaDefinitions)
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
                combinedXmlSchemaDefinition.Validate(xmlDocument);
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
        /// Validates whether the XML document is not empty.
        /// </summary>
        /// <exception cref="CriticalFileReadException">Thrown when the XML document
        /// does not contain configuration items.</exception>
        private void ValidateNotEmpty()
        {
            if (!xmlDocument.Descendants()
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