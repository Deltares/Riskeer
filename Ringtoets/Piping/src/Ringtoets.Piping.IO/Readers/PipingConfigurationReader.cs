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
using System.Linq;
using System.Xml.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Schema;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping configuration from XML and creates a collection of corresponding
    /// <see cref="IReadConfigurationItem"/>, typically containing one or more <see cref="ReadPipingCalculation"/>.
    /// </summary>
    public class PipingConfigurationReader : ConfigurationReader<ReadPipingCalculation>
    {
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string hydraulicBoundaryLocationSchemaName = "HrLocatieSchema.xsd";

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
        /// <item><paramref name="xmlFilePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        internal PipingConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   Resources.PipingConfiguratieSchema,
                   new Dictionary<string, string>
                   {
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       },
                       {
                           hydraulicBoundaryLocationSchemaName, RingtoetsCommonIOResources.HrLocatieSchema
                       }
                   }) {}

        protected override ReadPipingCalculation ParseCalculationElement(XElement calculationElement)
        {
            var constructionProperties = new ReadPipingCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute)?.Value,
                AssessmentLevel = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                           PipingConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocation = ConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                                     ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLine = ConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                       PipingConfigurationSchemaIdentifiers.SurfaceLineElement),
                EntryPointL = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                       PipingConfigurationSchemaIdentifiers.EntryPointLElement),
                ExitPointL = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(calculationElement,
                                                                                      PipingConfigurationSchemaIdentifiers.ExitPointLElement),
                StochasticSoilModel = ConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                               PipingConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfile = ConfigurationReaderHelper.GetStringValueFromDescendantElement(calculationElement,
                                                                                                 PipingConfigurationSchemaIdentifiers.StochasticSoilProfileElement)
            };

            XElement phreaticLevelExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName);
            if (phreaticLevelExitElement != null)
            {
                constructionProperties.PhreaticLevelExitMean = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(phreaticLevelExitElement,
                                                                                                                        ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.PhreaticLevelExitStandardDeviation = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(phreaticLevelExitElement,
                                                                                                                                     ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            XElement dampingFactorExitElement = GetStochastChildElement(calculationElement, PipingConfigurationSchemaIdentifiers.DampingFactorExitStochastName);
            if (dampingFactorExitElement != null)
            {
                constructionProperties.DampingFactorExitMean = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(dampingFactorExitElement,
                                                                                                                        ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.DampingFactorExitStandardDeviation = ConfigurationReaderHelper.GetDoubleValueFromDescendantElement(dampingFactorExitElement,
                                                                                                                                     ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            return new ReadPipingCalculation(constructionProperties);
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