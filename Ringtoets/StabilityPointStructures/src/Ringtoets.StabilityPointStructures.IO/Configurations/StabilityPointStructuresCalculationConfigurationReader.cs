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
using Ringtoets.Common.IO.Configurations.Helpers;
using Ringtoets.Common.IO.Readers;
using Ringtoets.Common.IO.Schema;
using Ringtoets.StabilityPointStructures.IO.Configurations.Helpers;
using Ringtoets.StabilityPointStructures.IO.Properties;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Reader for reading a stability point structures calculation configuration from XML and creating a collection 
    /// of corresponding <see cref="StabilityPointStructuresCalculationConfiguration"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationReader : CalculationConfigurationReader<StabilityPointStructuresCalculationConfiguration>
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
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfigurationReader"/>.
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
        public StabilityPointStructuresCalculationConfigurationReader(string filePath)
            : base(filePath,
                   Resources.StabilityPointStructuresConfigurationSchema,
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
                   }
            ) { }

        protected override StabilityPointStructuresCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            return new StabilityPointStructuresCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                AllowedLevelIncreaseStorage = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName),
                AreaFlowApertures = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName),
                BankWidth = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName),
                CriticalOvertoppingDischarge = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName),
                ConstructiveStrengthLinearLoadModel = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName),
                ConstructiveStrengthQuadraticLoadModel = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName),
                DrainCoefficient = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName),
                EvaluationLevel = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.EvaluationLevelElement),
                FactorStormDurationOpenStructure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.FactorStormDurationOpenStructureElement),
                FailureCollisionEnergy = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName),
                FailureProbabilityRepairClosure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.FailureProbabilityRepairClosureElement),
                FailureProbabilityStructureWithErosion = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement),
                FlowVelocityStructureClosable = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName),
                FlowWidthAtBottomProtection = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName),
                ForeshoreProfileName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                InflowModelType = (ConfigurationStabilityPointStructuresInflowModelType?)
                    calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationStabilityPointStructuresInflowModelTypeConverter>(
                        StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelTypeElement),
                InsideWaterLevel = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName),
                InsideWaterLevelFailureConstruction = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName),
                LevelCrestStructure = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName),
                LevellingCount = calculationElement.GetIntegerValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.LevellingCountElement),
                LoadSchematizationType = (ConfigurationStabilityPointStructuresLoadSchematizationType?)
                    calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter>(
                        StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationTypeElement),
                ProbabilityCollisionSecondaryStructure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.ProbabilityCollisionSecondaryStructureElement),
                ModelFactorSuperCriticalFlow = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.ModelFactorSuperCriticalFlowStochastName),
                ShipMass = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName),
                ShipVelocity = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName),
                StabilityLinearLoadModel = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName),
                StabilityQuadraticLoadModel = GetVariationCoefficientStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName),
                StructureName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.StructureElement),
                StorageStructureArea = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName),
                StormDuration = GetVariationCoefficientStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.StormDurationStochastName),
                StructureNormalOrientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation),
                ThresholdHeightOpenWeir = GetStandardDeviationStochastParameters(calculationElement, StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName),
                VerticalDistance = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.VerticalDistanceElement),
                VolumicWeightWater = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.VolumicWeightWaterElement),
                WaveReduction = GetWaveReductionParameters(calculationElement),
                WidthFlowApertures = GetStandardDeviationStochastParameters(calculationElement, ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName)
            };
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