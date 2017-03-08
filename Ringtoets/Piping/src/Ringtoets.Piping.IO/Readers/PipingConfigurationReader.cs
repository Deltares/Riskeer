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
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Core.Common.IO.Exceptions;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Schema;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping configuration from XML and creates a collection of corresponding
    /// <see cref="IReadConfigurationItem"/>, typically containing one or more <see cref="ReadPipingCalculation"/>.
    /// </summary>
    internal class PipingConfigurationReader : ConfigurationReader<ReadPipingCalculation>
    {
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
        internal PipingConfigurationReader(string xmlFilePath)
            : base(xmlFilePath, Resources.PipingConfigurationSchema) {}

        protected override ReadPipingCalculation ParseCalculationElement(XElement calculationElement)
        {
            var constructionProperties = new ReadPipingCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                AssessmentLevel = GetDoubleValueFromChildElement(calculationElement,
                                                                 PipingConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocation = GetStringValueFromChildElement(calculationElement,
                                                                           ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLine = GetStringValueFromChildElement(calculationElement,
                                                             PipingConfigurationSchemaIdentifiers.SurfaceLineElement),
                EntryPointL = GetDoubleValueFromChildElement(calculationElement,
                                                             PipingConfigurationSchemaIdentifiers.EntryPointLElement),
                ExitPointL = GetDoubleValueFromChildElement(calculationElement,
                                                            PipingConfigurationSchemaIdentifiers.ExitPointLElement),
                StochasticSoilModel = GetStringValueFromChildElement(calculationElement,
                                                                     PipingConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfile = GetStringValueFromChildElement(calculationElement,
                                                                       PipingConfigurationSchemaIdentifiers.StochasticSoilProfileElement)
            };

            XElement phreaticLevelExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName);
            if (phreaticLevelExitElement != null)
            {
                constructionProperties.PhreaticLevelExitMean = GetDoubleValueFromChildElement(phreaticLevelExitElement,
                                                                                              ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.PhreaticLevelExitStandardDeviation = GetDoubleValueFromChildElement(phreaticLevelExitElement,
                                                                                                           ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            XElement dampingFactorExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.DampingFactorExitStochastName);
            if (dampingFactorExitElement != null)
            {
                constructionProperties.DampingFactorExitMean = GetDoubleValueFromChildElement(dampingFactorExitElement,
                                                                                              ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.DampingFactorExitStandardDeviation = GetDoubleValueFromChildElement(dampingFactorExitElement,
                                                                                                           ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            return new ReadPipingCalculation(constructionProperties);
        }

        private static double? GetDoubleValueFromChildElement(XElement parentElement, string childElementName)
        {
            XElement childElement = parentElement.Elements(childElementName).FirstOrDefault();

            return childElement != null
                       ? (double?) XmlConvert.ToDouble(childElement.Value)
                       : null;
        }

        private static string GetStringValueFromChildElement(XElement parentElement, string childElementName)
        {
            XElement childElement = parentElement.Elements(childElementName).FirstOrDefault();

            return childElement?.Value;
        }

        private static XElement GetStochastChildElement(XElement parentElement, string stochastName)
        {
            return parentElement.Elements(ConfigurationSchemaIdentifiers.StochastsElement)
                                .FirstOrDefault()?
                                .Elements(ConfigurationSchemaIdentifiers.StochastElement)
                                .FirstOrDefault(e => e.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value == stochastName);
        }
    }
}