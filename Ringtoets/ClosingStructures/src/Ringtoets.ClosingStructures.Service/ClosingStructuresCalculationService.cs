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
using System.Linq;
using log4net;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Calculation.Calculator;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.ClosingStructures.Service
{
    /// <summary>
    /// Service that provides methods for perfoming Hydra-ring calculations for closing structures calculations.
    /// </summary>
    public class ClosingStructuresCalculationService
    {
        private static ILog log = LogManager.GetLogger(typeof(ClosingStructuresCalculationService));

        private IStructuresClosureCalculator calculator;
        private bool canceled;

        /// <summary>
        /// Performs a height structures calculation based on the supplied <see cref="ClosingStructuresCalculation"/> and sets 
        /// <see cref="ClosingStructuresCalculation.Output"/> if the calculation was successful. Error and status information is 
        /// logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="ClosingStructuresCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="generalInput">The <see cref="GeneralClosingStructuresInput"/> to create the input with for the calculation.</param>
        /// <param name="failureMechanismContribution">The amount of contribution for this failure mechanism in the assessment section.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <exception cref="NotSupportedException"></exception>
        public void Calculate(ClosingStructuresCalculation calculation,
                              IAssessmentSection assessmentSection,
                              FailureMechanismSection failureMechanismSection,
                              GeneralClosingStructuresInput generalInput,
                              double failureMechanismContribution, string hlcdDirectory)
        {
            StructuresClosureCalculationInput input;

            switch (calculation.InputParameters.InflowModelType)
            {
                case ClosingStructureInflowModelType.VerticalWall:
                    input = CreateClosureVerticalWallCalculationInput(calculation, failureMechanismSection, generalInput);
                    break;

                default:
                    throw new NotSupportedException("ClosingStructureInflowModelType");
            }

            calculator = HydraRingCalculatorFactory.Instance.CreateStructuresClosureCalculator(hlcdDirectory, assessmentSection.Id);

            calculator.Calculate(input);
        }

        private static StructuresClosureVerticalWallCalculationInput CreateClosureVerticalWallCalculationInput(ClosingStructuresCalculation calculation,
                                                                                                               FailureMechanismSection failureMechanismSection,
                                                                                                               GeneralClosingStructuresInput generalInput)
        {
            return new StructuresClosureVerticalWallCalculationInput(
                calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.StructureNormalOrientation),
                ParseForeshore(calculation.InputParameters),
                ParseBreakWater(calculation.InputParameters),
                generalInput.GravitationalAcceleration,
                calculation.InputParameters.FactorStormDurationOpenStructure,
                calculation.InputParameters.FailureProbabilityOpenStructure,
                calculation.InputParameters.FailureProbabilityReparation,
                calculation.InputParameters.IdenticalApertures,
                calculation.InputParameters.AllowedLevelIncreaseStorage.Mean, calculation.InputParameters.AllowedLevelIncreaseStorage.StandardDeviation,
                generalInput.ModelFactorStorageVolume.Mean, generalInput.ModelFactorStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.CoefficientOfVariation,
                generalInput.ModelFactorInflowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.CoefficientOfVariation,
                calculation.InputParameters.FailureProbabilityStructureWithErosion,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.CoefficientOfVariation,
                calculation.InputParameters.ProbabilityOpenStructureBeforeFlooding,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                calculation.InputParameters.StructureNormalOrientation,
                calculation.InputParameters.ModelFactorSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.LevelCrestStructureNotClosing.Mean, calculation.InputParameters.LevelCrestStructureNotClosing.StandardDeviation,
                calculation.InputParameters.WidthFlowApertures.Mean, calculation.InputParameters.WidthFlowApertures.CoefficientOfVariation,
                calculation.InputParameters.DeviationWaveDirection);
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(ClosingStructuresInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        private static HydraRingBreakWater ParseBreakWater(ClosingStructuresInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height) : null;
        }
    }
}