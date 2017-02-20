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
using System.Globalization;
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
using Ringtoets.Piping.IO.Schema;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping configuration from XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
    /// </summary>
    public class PipingConfigurationReader
    {
        private readonly XDocument xmlDocument;

        /// <summary>
        /// Creates a new instance of <see cref="PipingConfigurationReader"/>.
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
        public PipingConfigurationReader(string xmlFilePath)
        {
            IOUtils.ValidateFilePath(xmlFilePath);

            ValidateFileExists(xmlFilePath);

            xmlDocument = LoadDocument(xmlFilePath);

            ValidateToSchema(xmlDocument, xmlFilePath);
        }

        /// <summary>
        /// Reads the piping configuration from the XML and creates a collection of corresponding <see cref="IReadPipingCalculationItem"/>.
        /// </summary>
        /// <returns>A collection of read <see cref="IReadPipingCalculationItem"/>.</returns>
        public IEnumerable<IReadPipingCalculationItem> Read()
        {
            return ParseReadPipingCalculationItems(xmlDocument.Root?.Elements());
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
                return XDocument.Load(xmlFilePath, LoadOptions.PreserveWhitespace | LoadOptions.SetLineInfo | LoadOptions.SetBaseUri);
            }
            catch (Exception exception)
                when (exception is ArgumentNullException
                      || exception is InvalidOperationException
                      || exception is XmlException
                      || exception is IOException)
            {
                string message = new FileReaderErrorMessageBuilder(xmlFilePath).Build(CoreCommonUtilsResources.Error_General_IO_Import_ErrorMessage);
                throw new CriticalFileReadException(message, exception);
            }
        }

        /// <summary>
        /// Validates the provided XML document based on a predefined XML Schema Definition (XSD).
        /// </summary>
        /// <param name="document">The XML document to validate.</param>
        /// <param name="xmlFilePath">The file path the XML document is loaded from.</param>
        /// <exception cref="CriticalFileReadException">Thrown when the provided XML document does not match the predefined XML Schema Definition (XSD).</exception>
        private static void ValidateToSchema(XDocument document, string xmlFilePath)
        {
            XmlSchemaSet schema = LoadXmlSchema();

            try
            {
                document.Validate(schema, null);
            }
            catch (XmlSchemaValidationException exception)
            {
                string message = string.Format(Resources.PipingConfigurationReader_Configuration_contains_no_valid_xml_line_0_position_1_reason_2,
                                               exception.LineNumber,
                                               exception.LinePosition,
                                               exception.Message);

                throw new CriticalFileReadException(new FileReaderErrorMessageBuilder(xmlFilePath).Build(message), exception);
            }
        }

        private static XmlSchemaSet LoadXmlSchema()
        {
            Stream schemaFile = AssemblyUtils.GetAssemblyResourceStream(typeof(PipingConfigurationReader).Assembly,
                                                                        "Ringtoets.Piping.IO.Readers.XMLPipingConfigurationSchema.xsd");

            var xmlSchema = new XmlSchemaSet();
            xmlSchema.Add(XmlSchema.Read(schemaFile, null));

            return xmlSchema;
        }

        private static IEnumerable<IReadPipingCalculationItem> ParseReadPipingCalculationItems(IEnumerable<XElement> elements)
        {
            foreach (XElement element in elements)
            {
                if (element.Name == PipingConfigurationSchemaIdentifiers.CalculationElement)
                {
                    yield return ParseReadPipingCalculation(element);
                }

                if (element.Name == PipingConfigurationSchemaIdentifiers.FolderElement)
                {
                    yield return ParseReadPipingCalculationGroup(element);
                }
            }
        }

        private static ReadPipingCalculationGroup ParseReadPipingCalculationGroup(XElement folderElement)
        {
            return new ReadPipingCalculationGroup(folderElement.Attribute(PipingConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                                                  ParseReadPipingCalculationItems(folderElement.Elements()));
        }

        private static ReadPipingCalculation ParseReadPipingCalculation(XElement calculationElement)
        {
            var constructionProperties = new ReadPipingCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(PipingConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                AssessmentLevel = GetDoubleValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocation = GetStringValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLine = GetStringValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.SurfaceLineElement),
                EntryPointL = GetDoubleValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.EntryPointElement),
                ExitPointL = GetDoubleValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.ExitPointElement),
                StochasticSoilModel = GetStringValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfile = GetStringValueFromChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.StochasticSoilProfileElement)
            };

            XElement phreaticLevelExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName);
            if (phreaticLevelExitElement != null)
            {
                constructionProperties.PhreaticLevelExitMean = GetDoubleValueFromChildElement(phreaticLevelExitElement, "verwachtingswaarde");
                constructionProperties.PhreaticLevelExitStandardDeviation = GetDoubleValueFromChildElement(phreaticLevelExitElement, "standaardafwijking");
            }

            XElement dampingFactorExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.DampingFactorExitStochastName);
            if (dampingFactorExitElement != null)
            {
                constructionProperties.DampingFactorExitMean = GetDoubleValueFromChildElement(dampingFactorExitElement, "verwachtingswaarde");
                constructionProperties.DampingFactorExitStandardDeviation = GetDoubleValueFromChildElement(dampingFactorExitElement, "standaardafwijking");
            }

            return new ReadPipingCalculation(constructionProperties);
        }

        private static double? GetDoubleValueFromChildElement(XElement parentElement, string childElementName)
        {
            XElement childElement = parentElement.Elements(childElementName).FirstOrDefault();

            return childElement != null
                       ? (double?) Convert.ToDouble(childElement.Value, CultureInfo.InvariantCulture)
                       : null;
        }

        private static string GetStringValueFromChildElement(XElement parentElement, string childElementName)
        {
            XElement childElement = parentElement.Elements(childElementName).FirstOrDefault();

            return childElement?.Value;
        }

        private static XElement GetStochastChildElement(XElement parentElement, string stochastName)
        {
            return parentElement.Elements(PipingConfigurationSchemaIdentifiers.StochastElement)
                                .FirstOrDefault(e => e.Attribute(PipingConfigurationSchemaIdentifiers.NameAttribute)?.Value == stochastName);
        }
    }
}