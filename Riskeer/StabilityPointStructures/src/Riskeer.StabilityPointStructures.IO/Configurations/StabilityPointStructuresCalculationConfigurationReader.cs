﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.StabilityPointStructures.IO.Configurations.Helpers;
using Riskeer.StabilityPointStructures.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Reader for reading a stability point structures calculation configuration from XML and creating a collection 
    /// of corresponding <see cref="StabilityPointStructuresCalculationConfiguration"/>.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationReader : CalculationConfigurationReader<StabilityPointStructuresCalculationConfiguration>
    {
        private const string hbLocatieSchemaVersion0Name = "HbLocatieSchema_0.xsd";
        private const string hbLocatieSchemaVersion1Name = "HbLocatieSchema.xsd";
        private const string orientatieSchemaName = "OrientatieSchema.xsd";
        private const string golfReductieSchemaName = "GolfReductieSchema.xsd";
        private const string voorlandProfielSchemaName = "VoorlandProfielSchema.xsd";
        private const string stochastSchemaName = "StochastSchema.xsd";
        private const string stochastStandaardafwijkingSchemaName = "StochastStandaardafwijkingSchema.xsd";
        private const string stochastVariatiecoefficientSchemaName = "StochastVariatiecoefficientSchema.xsd";
        private const string structureBaseSchemaName = "KunstwerkenBasisSchema.xsd";
        private const string scenarioSchemaName = "ScenariosSchema.xsd";

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
            : base(filePath, new[]
            {
                new CalculationConfigurationSchemaDefinition(
                    0, Resources.KunstwerkenConstructiefFalenSchema_0,
                    new Dictionary<string, string>
                    {
                        {
                            hbLocatieSchemaVersion0Name, RiskeerCommonIOResources.HbLocatieSchema_0
                        },
                        {
                            orientatieSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            voorlandProfielSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
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
                            stochastVariatiecoefficientSchemaName, RiskeerCommonIOResources.StochastVariatiecoefficientSchema
                        },
                        {
                            structureBaseSchemaName, RiskeerCommonIOResources.KunstwerkenBasisSchema_0
                        },
                        {
                            scenarioSchemaName, RiskeerCommonIOResources.ScenarioSchema
                        }
                    }, string.Empty),
                new CalculationConfigurationSchemaDefinition(
                    1, Resources.KunstwerkenConstructiefFalenSchema,
                    new Dictionary<string, string>
                    {
                        {
                            hbLocatieSchemaVersion1Name, RiskeerCommonIOResources.HbLocatieSchema
                        },
                        {
                            orientatieSchemaName, RiskeerCommonIOResources.OrientatieSchema
                        },
                        {
                            voorlandProfielSchemaName, RiskeerCommonIOResources.VoorlandProfielSchema
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
                            stochastVariatiecoefficientSchemaName, RiskeerCommonIOResources.StochastVariatiecoefficientSchema
                        },
                        {
                            structureBaseSchemaName, RiskeerCommonIOResources.KunstwerkenBasisSchema
                        },
                        {
                            scenarioSchemaName, RiskeerCommonIOResources.ScenarioSchema
                        }
                    }, Resources.KunstwerkenConstructiefFalenConfiguratieSchema0To1)
            }) {}

        protected override StabilityPointStructuresCalculationConfiguration ParseCalculationElement(XElement calculationElement)
        {
            return new StabilityPointStructuresCalculationConfiguration(calculationElement.Attribute(ConfigurationSchemaIdentifiers.NameAttribute).Value)
            {
                AllowedLevelIncreaseStorage = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.AllowedLevelIncreaseStorageStochastName),
                AreaFlowApertures = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.AreaFlowAperturesStochastName),
                BankWidth = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.BankWidthStochastName),
                CriticalOvertoppingDischarge = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.CriticalOvertoppingDischargeStochastName),
                ConstructiveStrengthLinearLoadModel = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthLinearLoadModelStochastName),
                ConstructiveStrengthQuadraticLoadModel = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.ConstructiveStrengthQuadraticLoadModelStochastName),
                DrainCoefficient = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.DrainCoefficientStochastName),
                EvaluationLevel = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.EvaluationLevelElement),
                FactorStormDurationOpenStructure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.FactorStormDurationOpenStructureElement),
                FailureCollisionEnergy = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.FailureCollisionEnergyStochastName),
                FailureProbabilityRepairClosure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.FailureProbabilityRepairClosureElement),
                FailureProbabilityStructureWithErosion = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.FailureProbabilityStructureWithErosionElement),
                FlowVelocityStructureClosable = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.FlowVelocityStructureClosableStochastName),
                FlowWidthAtBottomProtection = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.FlowWidthAtBottomProtectionStochastName),
                ForeshoreProfileId = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.ForeshoreProfileNameElement),
                HydraulicBoundaryLocationName = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.HydraulicBoundaryLocationElement),
                InflowModelType = (ConfigurationStabilityPointStructuresInflowModelType?)
                    calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationStabilityPointStructuresInflowModelTypeConverter>(
                        StabilityPointStructuresConfigurationSchemaIdentifiers.InflowModelTypeElement),
                InsideWaterLevel = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelStochastName),
                InsideWaterLevelFailureConstruction = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.InsideWaterLevelFailureConstructionStochastName),
                LevelCrestStructure = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.LevelCrestStructureStochastName),
                LevellingCount = calculationElement.GetIntegerValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.LevellingCountElement),
                LoadSchematizationType = (ConfigurationStabilityPointStructuresLoadSchematizationType?)
                    calculationElement.GetConvertedValueFromDescendantStringElement<ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter>(
                        StabilityPointStructuresConfigurationSchemaIdentifiers.LoadSchematizationTypeElement),
                ProbabilityCollisionSecondaryStructure = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.ProbabilityCollisionSecondaryStructureElement),
                ShipMass = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.ShipMassStochastName),
                ShipVelocity = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.ShipVelocityStochastName),
                StabilityLinearLoadModel = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityLinearLoadModelStochastName),
                StabilityQuadraticLoadModel = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.StabilityQuadraticLoadModelStochastName),
                StructureId = calculationElement.GetStringValueFromDescendantElement(ConfigurationSchemaIdentifiers.StructureElement),
                StorageStructureArea = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.StorageStructureAreaStochastName),
                StormDuration = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.StormDurationStochastName),
                StructureNormalOrientation = calculationElement.GetDoubleValueFromDescendantElement(ConfigurationSchemaIdentifiers.Orientation),
                ThresholdHeightOpenWeir = calculationElement.GetStochastConfiguration(StabilityPointStructuresConfigurationSchemaIdentifiers.ThresholdHeightOpenWeirStochastName),
                VerticalDistance = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.VerticalDistanceElement),
                VolumicWeightWater = calculationElement.GetDoubleValueFromDescendantElement(StabilityPointStructuresConfigurationSchemaIdentifiers.VolumicWeightWaterElement),
                WaveReduction = calculationElement.GetWaveReductionParameters(),
                WidthFlowApertures = calculationElement.GetStochastConfiguration(ConfigurationSchemaIdentifiers.WidthFlowAperturesStochastName),
                ShouldIllustrationPointsBeCalculated = calculationElement.GetBoolValueFromDescendantElement(ConfigurationSchemaIdentifiers.ShouldIllustrationPointsBeCalculatedElement),
                Scenario = calculationElement.GetScenarioConfiguration()
            };
        }
    }
}