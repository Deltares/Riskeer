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
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.Configurations;
using Ringtoets.Common.IO.Exporters;
using Ringtoets.HeightStructures.Data;

namespace Ringtoets.HeightStructures.IO
{
    /// <summary>
    /// Exports a height structures calculation configuration and stores it as an XML file.
    /// </summary>
    public class HeightStructuresCalculationConfigurationExporter : SchemaCalculationConfigurationExporter<
        HeightStructuresCalculationConfigurationWriter,
        StructuresCalculation<HeightStructuresInput>,
        HeightStructureCalculationConfiguration
    >
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The calculation configuration to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HeightStructuresCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath) : base(calculations, filePath) {}

        protected override HeightStructureCalculationConfiguration ToConfiguration(StructuresCalculation<HeightStructuresInput> calculation)
        {
            HeightStructuresInput input = calculation.InputParameters;
            var calculationConfiguration = new HeightStructureCalculationConfiguration(calculation.Name);

            calculationConfiguration.HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name;

            if (input.Structure != null)
            {
                calculationConfiguration.StructureName = input.Structure.Name;
                calculationConfiguration.StructureNormalOrientation = input.StructureNormalOrientation;
                calculationConfiguration.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

                calculationConfiguration.FlowWidthAtBottomProtection = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = input.FlowWidthAtBottomProtection.Mean,
                    StandardDeviation = input.FlowWidthAtBottomProtection.StandardDeviation
                };
                calculationConfiguration.WidthFlowApertures = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = input.WidthFlowApertures.Mean,
                    StandardDeviation = input.WidthFlowApertures.StandardDeviation
                };
                calculationConfiguration.StorageStructureArea = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = input.StorageStructureArea.Mean,
                    VariationCoefficient = input.StorageStructureArea.CoefficientOfVariation
                };
                calculationConfiguration.AllowedLevelIncreaseStorage = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = input.AllowedLevelIncreaseStorage.Mean,
                    StandardDeviation = input.AllowedLevelIncreaseStorage.StandardDeviation
                };
                calculationConfiguration.LevelCrestStructure = new MeanStandardDeviationStochastConfiguration
                {
                    Mean = input.LevelCrestStructure.Mean,
                    StandardDeviation = input.LevelCrestStructure.StandardDeviation
                };
                calculationConfiguration.CriticalOvertoppingDischarge = new MeanVariationCoefficientStochastConfiguration
                {
                    Mean = input.CriticalOvertoppingDischarge.Mean,
                    VariationCoefficient = input.CriticalOvertoppingDischarge.CoefficientOfVariation
                };
            }

            if (input.ForeshoreProfile != null)
            {
                calculationConfiguration.ForeshoreProfileName = input.ForeshoreProfile?.Name;
                calculationConfiguration.WaveReduction = new WaveReductionConfiguration
                {
                    UseForeshoreProfile = input.UseForeshore,
                    UseBreakWater = input.UseBreakWater
                };

                if (input.UseBreakWater)
                {
                    calculationConfiguration.WaveReduction.BreakWaterType = (ReadBreakWaterType) input.BreakWater.Type;
                    calculationConfiguration.WaveReduction.BreakWaterHeight = input.BreakWater.Height;
                }
            }

            calculationConfiguration.StormDuration = new MeanVariationCoefficientStochastConfiguration
            {
                Mean = input.StormDuration.Mean
            };
            calculationConfiguration.ModelFactorSuperCriticalFlow = new MeanStandardDeviationStochastConfiguration
            {
                Mean = input.ModelFactorSuperCriticalFlow.Mean
            };

            return calculationConfiguration;
        }
    }
}