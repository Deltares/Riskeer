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

using System.Collections.Generic;
using System.Xml.Linq;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.HeightStructures.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.IO
{
    public class HeightStructuresCalculationConfigurationReader : CalculationConfigurationReader<HeightStructuresCalculationConfiguration>
    {
        private const string hrLocatieSchemaName = "HrLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string voorlandProfielSchemaName = "VoorlandProfielSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";
        private const string stochastVariatiecoefficientSchemaName = "StochastVariatiecoefficientSchema.xsd";
        private const string structureBaseSchemaName = "KunstwerkenBasisSchema.xsd";

        public HeightStructuresCalculationConfigurationReader(string filePath)
            : base(filePath,
                   Resources.KunstwerkenHoogteSchema,
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

        protected override HeightStructuresCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            var configuration = new HeightStructuresCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value);
            configuration.FailureProbabilityStructureWithErosion = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement);
            configuration.StructureNormalOrientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation);

            configuration.ForeshoreProfileName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement);
            configuration.HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement);
            configuration.StructureName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.StructureElement);

            configuration.WaveReduction = GetWaveReductionParameters(calculationElement);

            configuration.LevelCrestStructure = GetStandardDeviationStochastParameters(calculationElement, HeightStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName);
            configuration.AllowedLevelIncreaseStorage = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName);
            configuration.FlowWidthAtBottomProtection = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName);
            configuration.ModelFactorSuperCriticalFlow = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName);
            configuration.WidthFlowApertures = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName);

            configuration.CriticalOvertoppingDischarge = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName);
            configuration.StorageStructureArea = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName);
            configuration.StormDuration = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.StormDurationStochastName);

            return configuration;
        }

        private WaveReductionConfiguration GetWaveReductionParameters(XElement calculationElement)
        {
            XElement waveReduction = calculationElement.GetDescendantElement(ConfigurationSchemaIdentifiers.WaveReduction);
            if (waveReduction != null)
            {
                return new WaveReductionConfiguration
                {
                    BreakWaterType = (ReadBreakWaterType?) calculationElement.GetConvertedValueFromDescendantStringElement<ReadBreakWaterTypeConverter>(ConfigurationSchemaIdentifiers.BreakWaterType),
                    BreakWaterHeight = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.BreakWaterHeight),
                    UseBreakWater = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseBreakWater),
                    UseForeshoreProfile = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.UseForeshore)
                };
            }
            return null;
        }

        private static MeanVariationCoefficientStochastConfiguration GetVariationCoefficientStochastParameters(XElement calculationElement, string stochastName)
        {
            XElement element = calculationElement.GetStochastElement(stochastName);
            if (element != null)
            {
                return new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement),
                    VariationCoefficient = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.VariationCoefficientElement)
                };
            }
            return null;
        }

        private static MeanStandardDeviationStochastConfiguration GetStandardDeviationStochastParameters(XElement calculationElement, string stochastName)
        {
            XElement element = calculationElement.GetStochastElement(stochastName);
            if (element != null)
            {
                return new MeanStandardDeviationStochastConfiguration
                {
                    Mean = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.MeanElement),
                    StandardDeviation = element.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.StandardDeviationElement)
                };
            }
            return null;
        }
    }
}