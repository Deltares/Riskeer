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
using System.Xml.Linq;
using Core.Common.Base.IO;
using Riskeer.Common.IO.Configurations;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.Common.IO.Configurations.Import;
using Riskeer.GrassCoverErosionInwards.IO.Configurations.Helpers;
using Riskeer.GrassCoverErosionInwards.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// This class reads a grass cover erosion inwards configuration from XML and creates
    /// a collection of corresponding <see cref="IConfigurationItem"/>, typically
    /// containing one or more <see cref="GrassCoverErosionInwardsCalculationConfiguration"/>.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfigurationReader : CalculationConfigurationReader<GrassCoverErosionInwardsCalculationConfiguration>
    {
        private const string hbLocatieSchemaName = "HbLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";
        private const string scenarioSchemaName = "ScenarioSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionInwardsCalculationConfigurationReader"/>.
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
        public GrassCoverErosionInwardsCalculationConfigurationReader(string xmlFilePath)
            : base(xmlFilePath,
                   new CalculationConfigurationSchemaDefinitions(
                       Resources.GEKBConfiguratieSchema,
                       new Dictionary<string, string>
                       {
                           {
                               hbLocatieSchemaName, RiskeerCommonIOResources.HbLocatieSchema
                           },
                           {
                               orientatieSchemaName, RiskeerCommonIOResources.OrientatieSchema
                           },
                           {
                               golfReductieSchemaName, RiskeerCommonIOResources.GolfReductieSchema
                           },
                           {
                               stochastSchemaName, RiskeerCommonIOResources.StochastSchema
                           },
                           {
                               stochastStandaardafwijkingSchemaName, RiskeerCommonIOResources.StochastStandaardafwijkingSchema
                           },
                           {
                               scenarioSchemaName, RiskeerCommonIOResources.ScenarioSchema
                           }
                       }))
        {}

        protected override GrassCoverErosionInwardsCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new GrassCoverErosionInwardsCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                HydraulicBoundaryLocationName = calculationElement.GetHydraulicBoundaryLocationName(),
                DikeProfileId = calculationElement.GetStringValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeProfileElement),
                Orientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation),
                DikeHeight = calculationElement.GetDoubleValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightElement),
                DikeHeightCalculationType = (ConfigurationHydraulicLoadsCalculationType?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationHydraulicLoadsCalculationTypeConverter>(
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightCalculationTypeElement),
                OvertoppingRateCalculationType = (ConfigurationHydraulicLoadsCalculationType?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationHydraulicLoadsCalculationTypeConverter>(
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.OvertoppingRateCalculationTypeElement),
                WaveReduction = calculationElement.GetWaveReductionParameters(),
                CriticalFlowRate = calculationElement.GetStochastConfiguration(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName),
                ShouldOvertoppingOutputIllustrationPointsBeCalculated = calculationElement.GetBoolValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldOvertoppingOutputIllustrationPointsBeCalculatedElement),
                ShouldDikeHeightIllustrationPointsBeCalculated = calculationElement.GetBoolValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldDikeHeightIllustrationPointsBeCalculatedElementElement),
                ShouldOvertoppingRateIllustrationPointsBeCalculated = calculationElement.GetBoolValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.ShouldOvertoppingRateIllustrationPointsBeCalculatedElement),
                Scenario = calculationElement.GetScenarioConfiguration()
            };

            return configuration;
        }
    }
}