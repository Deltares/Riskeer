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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.Schema;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// This class reads a piping calculation configuration from XML and creates a collection of corresponding
    /// <see cref="IConfigurationItem"/>, typically containing one or more <see cref="ReadPipingCalculation"/>.
    /// </summary>
    public class PipingCalculationConfigurationReader : CalculationConfigurationReader<ReadPipingCalculation>
    {
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfigurationReader"/>.
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
        internal PipingCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   Resources.PipingConfiguratieSchema,
                   new Dictionary<string, string>
                   {
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       },
                       {
                           stochastStandaardafwijkingSchemaName, RingtoetsCommonIOResources.StochastStandaardafwijkingSchema
                       }
                   }) {}

        protected override ReadPipingCalculation ParseCalculationElement(XElement calculationElement)
        {
            var constructionProperties = new ReadPipingCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value,
                AssessmentLevel = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.AssessmentLevelElement),
                HydraulicBoundaryLocation = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                SurfaceLine = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.SurfaceLineElement),
                EntryPointL = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.EntryPointLElement),
                ExitPointL = calculationElement.GetDoubleValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.ExitPointLElement),
                StochasticSoilModel = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilModelElement),
                StochasticSoilProfile = calculationElement.GetStringValueFromDescendantElement(PipingCalculationConfigurationSchemaIdentifiers.StochasticSoilProfileElement)
            };

            XElement phreaticLevelExitElement = calculationElement.GetStochastElement(PipingCalculationConfigurationSchemaIdentifiers.PhreaticLevelExitStochastName);
            if (phreaticLevelExitElement != null)
            {
                constructionProperties.PhreaticLevelExitMean = phreaticLevelExitElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.PhreaticLevelExitStandardDeviation = phreaticLevelExitElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            XElement dampingFactorExitElement = calculationElement.GetStochastElement(PipingCalculationConfigurationSchemaIdentifiers.DampingFactorExitStochastName);
            if (dampingFactorExitElement != null)
            {
                constructionProperties.DampingFactorExitMean = dampingFactorExitElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.DampingFactorExitStandardDeviation = dampingFactorExitElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            return new ReadPipingCalculation(constructionProperties);
        }
    }
}