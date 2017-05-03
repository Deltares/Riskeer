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
using Ringtoets.ClosingStructures.IO.Configurations.Helpers;
using Ringtoets.ClosingStructures.IO.Properties;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Configurations.Import;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Reader for reading a closing structure calculation configuration from XML and creating a collection 
    /// of corresponding <see cref="ClosingStructuresCalculationConfiguration"/>.
    /// </summary>
    public class ClosingStructuresCalculationConfigurationReader : CalculationConfigurationReader<ClosingStructuresCalculationConfiguration>
    {
        private const string hrLocatieSchemaName = "HrLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string voorlandProfielSchemaName = "VoorlandProfielSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";
        private const string stochastVariatiecoefficientSchemaName = "StochastVariatiecoefficientSchema.xsd";
        private const string structureBaseSchemaName = "KunstwerkenBasisSchema.xsd";

        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationConfigurationReader"/>.
        /// </summary>
        /// <param name="filePath">The file path to the XML file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="filePath"/> points to a file that does not exist.</item>
        /// <item><paramref name="filePath"/> points to a file that does not contain valid XML.</item>
        /// <item><paramref name="filePath"/> points to a file that does not pass the schema validation.</item>
        /// <item><paramref name="filePath"/> points to a file that does not contain configuration elements.</item>
        /// </list>
        /// </exception>
        public ClosingStructuresCalculationConfigurationReader(string filePath)
            : base(filePath,
                   Resources.KunstwerkenBetrouwbaarheidSluitenSchema,
                   new Dictionary<string, string>
                   {
                       {
                           hrLocatieSchemaName, RingtoetsCommonIOResources.HrLocatieSchema
                       },
                       {
                           orientatieSchemaName, RingtoetsCommonIOResources.OrientatieSchema
                       },
                       {
                           voorlandProfielSchemaName, RingtoetsCommonIOResources.VoorlandProfielSchema
                       },
                       {
                           golfReductieSchemaName, RingtoetsCommonIOResources.GolfReductieSchema
                       },
                       {
                           stochastSchemaName, RingtoetsCommonIOResources.StochastSchema
                       },
                       {
                           stochastStandaardafwijkingSchemaName, RingtoetsCommonIOResources.StochastStandaardafwijkingSchema
                       },
                       {
                           stochastVariatiecoefficientSchemaName, RingtoetsCommonIOResources.StochastVariatiecoefficientSchema
                       },
                       {
                           structureBaseSchemaName, RingtoetsCommonIOResources.KunstwerkenBasisSchema
                       }
                   }) {}

        protected override ClosingStructuresCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new ClosingStructuresCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                FailureProbabilityStructureWithErosion = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement),
                StructureNormalOrientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation),
                InflowModelType = (ConfigurationClosingStructureInflowModelType?)
                    calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationClosingStructureInflowModelTypeConverter>(
                        ClosingStructuresConfigurationSchemaIdentifiers.InflowModelType),
                FactorStormDurationOpenStructure = calculationElement.GetDoubleValueFromDescendantElement(ClosingStructuresConfigurationSchemaIdentifiers.FactorStormDurationOpenStructure),
                FailureProbabilityOpenStructure = calculationElement.GetDoubleValueFromDescendantElement(ClosingStructuresConfigurationSchemaIdentifiers.FailureProbabilityOpenStructure),
                FailureProbabilityReparation = calculationElement.GetDoubleValueFromDescendantElement(ClosingStructuresConfigurationSchemaIdentifiers.FailureProbabilityReparation),
                ProbabilityOrFrequencyOpenStructureBeforeFlooding = calculationElement.GetDoubleValueFromDescendantElement(ClosingStructuresConfigurationSchemaIdentifiers.ProbabilityOrFrequencyOpenStructureBeforeFlooding),
                IdenticalApertures = calculationElement.GetIntegerValueFromDescendantElement(ClosingStructuresConfigurationSchemaIdentifiers.IdenticalApertures),
                ForeshoreProfileId = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                StructureId = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.StructureElement),
                WaveReduction = GetWaveReductionParameters(calculationElement),
                AreaFlowApertures = calculationElement.GetStochastConfiguration(ClosingStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName),
                DrainCoefficient = calculationElement.GetStochastConfiguration(ClosingStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName),
                InsideWaterLevel = calculationElement.GetStochastConfiguration(ClosingStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName),
                LevelCrestStructureNotClosing = calculationElement.GetStochastConfiguration(ClosingStructuresConfigurationSchemaIdentifiers.LevelCrestStructureNotClosingStochastName),
                ThresholdHeightOpenWeir = calculationElement.GetStochastConfiguration(ClosingStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName),
                AllowedLevelIncreaseStorage = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName),
                FlowWidthAtBottomProtection = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName),
                ModelFactorSuperCriticalFlow = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName),
                WidthFlowApertures = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName),
                CriticalOvertoppingDischarge = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName),
                StorageStructureArea = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName),
                StormDuration = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.StormDurationStochastName)
            };

            return configuration;
        }

        private static WaveReductionConfiguration GetWaveReductionParameters(XElement calculationElement)
        {
            XElement waveReduction = calculationElement.GetDescendantElement(ConfigurationSchemaIdentifiers.WaveReduction);
            if (waveReduction != null)
            {
                return new WaveReductionConfiguration
                {
                    BreakWaterType = (ConfigurationBreakWaterType?) calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationBreakWaterTypeConverter>(ConfigurationSchemaIdentifiers.BreakWaterType),
                    BreakWaterHeight = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.BreakWaterHeight),
                    UseBreakWater = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseBreakWater),
                    UseForeshoreProfile = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseForeshore)
                };
            }
            return null;
        }
    }
}