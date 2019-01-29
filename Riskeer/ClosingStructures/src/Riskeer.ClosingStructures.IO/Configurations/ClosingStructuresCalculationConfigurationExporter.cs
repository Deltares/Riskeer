// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.IO.Configurations.Helpers;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.IO.Configurations.Export;
using Ringtoets.Common.IO.Configurations.Helpers;

namespace Riskeer.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Exports a closing structures calculation configuration and stores it as an XML file.
    /// </summary>
    public class ClosingStructuresCalculationConfigurationExporter
        : CalculationConfigurationExporter<
            ClosingStructuresCalculationConfigurationWriter,
            StructuresCalculation<ClosingStructuresInput>,
            ClosingStructuresCalculationConfiguration>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ClosingStructuresCalculationConfigurationExporter"/>.
        /// </summary>
        /// <param name="calculations">The hierarchy of calculations to export.</param>
        /// <param name="filePath">The path of the XML file to export to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculations"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        public ClosingStructuresCalculationConfigurationExporter(IEnumerable<ICalculationBase> calculations, string filePath)
            : base(calculations, filePath) {}

        protected override ClosingStructuresCalculationConfiguration ToConfiguration(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            ClosingStructuresInput input = calculation.InputParameters;
            var calculationConfiguration = new ClosingStructuresCalculationConfiguration(calculation.Name)
            {
                HydraulicBoundaryLocationName = input.HydraulicBoundaryLocation?.Name,
                ShouldIllustrationPointsBeCalculated = input.ShouldIllustrationPointsBeCalculated,
                FailureProbabilityStructureWithErosion = input.FailureProbabilityStructureWithErosion,
                FactorStormDurationOpenStructure = input.FactorStormDurationOpenStructure,
                StormDuration = input.StormDuration.ToStochastConfigurationWithMean(),
                DrainCoefficient = input.DrainCoefficient.ToStochastConfigurationWithMean(),
                ModelFactorSuperCriticalFlow = input.ModelFactorSuperCriticalFlow.ToStochastConfigurationWithMean()
            };

            calculationConfiguration.SetConfigurationForeshoreProfileDependentProperties(input);

            if (input.Structure != null)
            {
                calculationConfiguration.StructureId = input.Structure.Id;
                calculationConfiguration.StructureNormalOrientation = input.StructureNormalOrientation;

                calculationConfiguration.InflowModelType = (ConfigurationClosingStructureInflowModelType?) new ConfigurationClosingStructureInflowModelTypeConverter().ConvertFrom(input.InflowModelType);
                calculationConfiguration.IdenticalApertures = input.IdenticalApertures;
                calculationConfiguration.FailureProbabilityOpenStructure = input.FailureProbabilityOpenStructure;
                calculationConfiguration.FailureProbabilityReparation = input.FailureProbabilityReparation;
                calculationConfiguration.ProbabilityOpenStructureBeforeFlooding = input.ProbabilityOpenStructureBeforeFlooding;

                calculationConfiguration.FlowWidthAtBottomProtection = input.FlowWidthAtBottomProtection.ToStochastConfiguration();
                calculationConfiguration.WidthFlowApertures = input.WidthFlowApertures.ToStochastConfiguration();
                calculationConfiguration.StorageStructureArea = input.StorageStructureArea.ToStochastConfiguration();
                calculationConfiguration.AllowedLevelIncreaseStorage = input.AllowedLevelIncreaseStorage.ToStochastConfiguration();
                calculationConfiguration.CriticalOvertoppingDischarge = input.CriticalOvertoppingDischarge.ToStochastConfiguration();

                calculationConfiguration.AreaFlowApertures = input.AreaFlowApertures.ToStochastConfiguration();
                calculationConfiguration.InsideWaterLevel = input.InsideWaterLevel.ToStochastConfiguration();
                calculationConfiguration.LevelCrestStructureNotClosing = input.LevelCrestStructureNotClosing.ToStochastConfiguration();
                calculationConfiguration.ThresholdHeightOpenWeir = input.ThresholdHeightOpenWeir.ToStochastConfiguration();
            }

            return calculationConfiguration;
        }

        protected override ClosingStructuresCalculationConfigurationWriter CreateWriter(string filePath)
        {
            return new ClosingStructuresCalculationConfigurationWriter(filePath);
        }
    }
}