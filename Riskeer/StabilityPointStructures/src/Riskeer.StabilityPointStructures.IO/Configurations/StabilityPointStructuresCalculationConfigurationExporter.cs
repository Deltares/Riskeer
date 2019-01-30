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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.IO.Configurations.Export;
using Riskeer.Common.IO.Configurations.Helpers;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityPointStructures.IO.Configurations.Helpers;

namespace Riskeer.StabilityPointStructures.IO.Configurations
{
    /// <summary>
    /// Exports a stability point structures calculation configuration and stores it as an XML file.
    /// </summary>
    public class StabilityPointStructuresCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            StabilityPointStructuresCalculationConfigurationWriter,
            StructuresCalculation<StabilityPointStructuresInput>,
            StabilityPointStructuresCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityPointStructuresCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public StabilityPointStructuresCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override StabilityPointStructuresCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new StabilityPointStructuresCalculationConfigurationWriter(filePath);
        }

        protected override StabilityPointStructuresCalculationConfiguration ToConfiguration(StructuresCalculation<StabilityPointStructuresInput> calculation)
        {
            StabilityPointStructuresInput input = calculation.InputParameters;

            var calculationConfiguration = new StabilityPointStructuresCalculationConfiguration(calculation.Name)
            {
                DrainCoefficient = input.DrainCoefficient.ToStochastConfigurationWithMean(),
                FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure,
                FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion,
                HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name,
                VolumicWeightWater = input.VolumicWeightWater,
                StormDuration = input.StormDuration.ToStochastConfigurationWithMean(),
                ShouldIllustrationPointsBeCalculated = input.ShouldIllustrationPointsBeCalculated
            };

            SetConfigurationStructureDependentParameters(calculationConfiguration, input);
            calculationConfiguration.SetConfigurationForeshoreProfileDependentProperties(input);
            return calculationConfiguration;
        }

        private static void SetConfigurationStructureDependentParameters(StabilityPointStructuresCalculationConfiguration calculationConfiguration,
                                                                         StabilityPointStructuresInput input)
        {
            if (input.Structure == null)
            {
                return;
            }

            calculationConfiguration.AllowedLevelIncreaseStorage = input.AllowedLevelIncreaseStorage.ToStochastConfiguration();
            calculationConfiguration.AreaFlowApertures = input.AreaFlowApertures.ToStochastConfiguration();
            calculationConfiguration.BankWidth = input.BankWidth.ToStochastConfiguration();
            calculationConfiguration.ConstructiveStrengthLinearLoadModel = input.ConstructiveStrengthLinearLoadModel.ToStochastConfiguration();

            calculationConfiguration.ConstructiveStrengthQuadraticLoadModel = input.ConstructiveStrengthQuadraticLoadModel.ToStochastConfiguration();
            calculationConfiguration.CriticalOvertoppingDischarge = input.CriticalOvertoppingDischarge.ToStochastConfiguration();
            calculationConfiguration.EvaluationLevel = input.EvaluationLevel;
            calculationConfiguration.FailureCollisionEnergy = input.FailureCollisionEnergy.ToStochastConfiguration();

            calculationConfiguration.FailureProbabilityRepairClosure = input.FailureProbabilityRepairClosure;
            calculationConfiguration.FlowVelocityStructureClosable = input.FlowVelocityStructureClosable.ToStochastConfigurationWithMean();
            calculationConfiguration.FlowWidthAtBottomProtection = input.FlowWidthAtBottomProtection.ToStochastConfiguration();
            if (Enum.IsDefined(typeof(StabilityPointStructureInflowModelType), input.InflowModelType))
            {
                calculationConfiguration.InflowModelType = (ConfigurationStabilityPointStructuresInflowModelType?)
                    new ConfigurationStabilityPointStructuresInflowModelTypeConverter().ConvertFrom(input.InflowModelType);
            }

            calculationConfiguration.InsideWaterLevel = input.InsideWaterLevel.ToStochastConfiguration();
            calculationConfiguration.InsideWaterLevelFailureConstruction = input.InsideWaterLevelFailureConstruction.ToStochastConfiguration();
            calculationConfiguration.LevelCrestStructure = input.LevelCrestStructure.ToStochastConfiguration();
            calculationConfiguration.LevellingCount = input.LevellingCount;

            if (Enum.IsDefined(typeof(LoadSchematizationType), input.LoadSchematizationType))
            {
                calculationConfiguration.LoadSchematizationType = (ConfigurationStabilityPointStructuresLoadSchematizationType?)
                    new ConfigurationStabilityPointStructuresLoadSchematizationTypeConverter().ConvertFrom(input.LoadSchematizationType);
            }

            calculationConfiguration.ProbabilityCollisionSecondaryStructure = input.ProbabilityCollisionSecondaryStructure;
            calculationConfiguration.ShipMass = input.ShipMass.ToStochastConfiguration();
            calculationConfiguration.ShipVelocity = input.ShipVelocity.ToStochastConfiguration();

            calculationConfiguration.StabilityLinearLoadModel = input.StabilityLinearLoadModel.ToStochastConfiguration();
            calculationConfiguration.StabilityQuadraticLoadModel = input.StabilityQuadraticLoadModel.ToStochastConfiguration();
            calculationConfiguration.StorageStructureArea = input.StorageStructureArea.ToStochastConfiguration();
            calculationConfiguration.StructureId = input.Structure.Id;

            calculationConfiguration.StructureNormalOrientation = input.StructureNormalOrientation;
            calculationConfiguration.ThresholdHeightOpenWeir = input.ThresholdHeightOpenWeir.ToStochastConfiguration();
            calculationConfiguration.VerticalDistance = input.VerticalDistance;
            calculationConfiguration.WidthFlowApertures = input.WidthFlowApertures.ToStochastConfiguration();
        }
    }
}