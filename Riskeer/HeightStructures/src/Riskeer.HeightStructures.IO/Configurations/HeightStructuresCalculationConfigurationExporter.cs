// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.HeightStructures.Data;

namespace Riskeer.HeightStructures.IO.Configurations
{
    /// <summary>
    /// Exports a height structures calculation configuration and stores it as an XML file.
    /// </summary>
    public class HeightStructuresCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            HeightStructuresCalculationConfigurationWriter,
            StructuresCalculationScenario<HeightStructuresInput>,
            HeightStructuresCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public HeightStructuresCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, new HeightStructuresCalculationConfigurationWriter(filePath)) {}

        protected override HeightStructuresCalculationConfiguration ToConfiguration(StructuresCalculationScenario<HeightStructuresInput> calculation)
        {
            HeightStructuresInput input = calculation.InputParameters;
            var calculationConfiguration = new HeightStructuresCalculationConfiguration(calculation.Name)
            {
                HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name,
                ShouldIllustrationPointsBeCalculated = input.ShouldIllustrationPointsBeCalculated,
                StormDuration = input.StormDuration.ToStochastConfigurationWithMean(),
                ModelFactorSuperCriticalFlow = input.ModelFactorSuperCriticalFlow.ToStochastConfigurationWithMean(),
                Scenario = calculation.ToScenarioConfiguration()
            };

            calculationConfiguration.SetConfigurationForeshoreProfileDependentProperties(input);

            if (input.Structure != null)
            {
                calculationConfiguration.StructureId = input.Structure.Id;
                calculationConfiguration.StructureNormalOrientation = input.StructureNormalOrientation;
                calculationConfiguration.FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion;

                calculationConfiguration.FlowWidthAtBottomProtection = input.FlowWidthAtBottomProtection.ToStochastConfiguration();
                calculationConfiguration.WidthFlowApertures = input.WidthFlowApertures.ToStochastConfiguration();
                calculationConfiguration.StorageStructureArea = input.StorageStructureArea.ToStochastConfiguration();
                calculationConfiguration.AllowedLevelIncreaseStorage = input.AllowedLevelIncreaseStorage.ToStochastConfiguration();
                calculationConfiguration.LevelCrestStructure = input.LevelCrestStructure.ToStochastConfiguration();
                calculationConfiguration.CriticalOvertoppingDischarge = input.CriticalOvertoppingDischarge.ToStochastConfiguration();
            }

            return calculationConfiguration;
        }
    }
}