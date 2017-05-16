﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using Ringtoets.GrassCoverErosionInwards.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.IO.Readers
{
    /// <summary>
    /// This class reads a grass cover erosion inwards configuration from XML and creates
    /// a collection of corresponding <see cref="IConfigurationItem"/>, typically
    /// containing one or more <see cref="ReadGrassCoverErosionInwardsCalculation"/>.
    /// </summary>
    public class GrassCoverErosionInwardsCalculationConfigurationReader : CalculationConfigurationReader<ReadGrassCoverErosionInwardsCalculation>
    {
        private const string hrLocatieSchemaName = "HrLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";

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
                   Resources.GEKBConfiguratieSchema,
                   new Dictionary<string, string>
                   {
                       {
                           hrLocatieSchemaName, RingtoetsCommonIOResources.HrLocatieSchema
                       },
                       {
                           orientatieSchemaName, RingtoetsCommonIOResources.OrientatieSchema
                       },
                       {
                           golfReductieSchemaName, RingtoetsCommonIOResources.GolfReductieSchema
                       },
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       },
                       {
                           stochastStandaardafwijkingSchemaName, RingtoetsCommonIOResources.StochastStandaardafwijkingSchema
                       }
                   }) {}

        protected override ReadGrassCoverErosionInwardsCalculation ParseCalculationElement(XElement calculationElement)
        {
            var constructionProperties = new ReadGrassCoverErosionInwardsCalculation.ConstructionProperties
            {
                Name = calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value,
                HydraulicBoundaryLocation = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                DikeProfileId = calculationElement.GetStringValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeProfileElement),
                Orientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation),
                DikeHeight = calculationElement.GetDoubleValueFromDescendantElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightElement),
                DikeHeightCalculationType = (ReadHydraulicLoadsCalculationType?) calculationElement.GetConvertedValueFromDescendantStringElement<ReadHydraulicLoadsCalculationTypeConverter>(
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.DikeHeightCalculationTypeElement),
                OvertoppingRateCalculationType = (ReadHydraulicLoadsCalculationType?) calculationElement.GetConvertedValueFromDescendantStringElement<ReadHydraulicLoadsCalculationTypeConverter>(
                    GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.OvertoppingRateCalculationTypeElement),
                UseBreakWater = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseBreakWater),
                BreakWaterType = (ConfigurationBreakWaterType?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationBreakWaterTypeConverter>(ConfigurationSchemaIdentifiers.BreakWaterType),
                BreakWaterHeight = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.BreakWaterHeight),
                UseForeshore = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseForeshore)
            };

            XElement criticalFlowRateElement = calculationElement.GetStochastElement(GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers.CriticalFlowRateStochastName);
            if (criticalFlowRateElement != null)
            {
                constructionProperties.CriticalFlowRateMean = criticalFlowRateElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement);
                constructionProperties.CriticalFlowRateStandardDeviation = criticalFlowRateElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.StandardDeviationElement);
            }

            return new ReadGrassCoverErosionInwardsCalculation(constructionProperties);
        }
    }
}