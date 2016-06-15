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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Service;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.HeightStructures.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for height structures calculations.
    /// </summary>
    internal static class HeightStructuresCalculationService
    {
        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        internal static bool Validate(HeightStructuresCalculation calculation, IAssessmentSection assessmentSection)
        {
            return CalculationServiceHelper.PerformValidation(calculation.Name, () => ValidateInput(calculation.InputParameters, assessmentSection));
        }

        /// <summary>
        /// Performs a height structures calculation based on the supplied <see cref="HeightStructuresCalculation"/> and sets <see cref="HeightStructuresCalculation.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="HeightStructuresCalculation"/> to base the input for the calculation upon.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="generalInput">The <see cref="GeneralHeightStructuresInput"/> to create the input with for the calculation.</param>
        /// <returns>A <see cref="ExceedanceProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        internal static ExceedanceProbabilityCalculationOutput Calculate(HeightStructuresCalculation calculation,
                                                                         string hlcdDirectory, FailureMechanismSection failureMechanismSection,
                                                                         string ringId, GeneralHeightStructuresInput generalInput)
        {
            var input = CreateInput(calculation, failureMechanismSection, generalInput);
            return CalculationServiceHelper.PerformCalculation(calculation.Name,
                                                               () => HydraRingCalculationService.PerformCalculation(hlcdDirectory, ringId, HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, input),
                                                               Resources.HeightStructuresCalculationService_Calculate_Error_in_height_structures_0_calculation);
        }

        private static StructuresOvertoppingCalculationInput CreateInput(HeightStructuresCalculation calculation, FailureMechanismSection failureMechanismSection, GeneralHeightStructuresInput generalInput)
        {
            return new StructuresOvertoppingCalculationInput(
                (int) calculation.InputParameters.HydraulicBoundaryLocation.Id,
                new HydraRingSection(1, failureMechanismSection.Name, failureMechanismSection.GetSectionLength(), calculation.InputParameters.OrientationOfTheNormalOfTheStructure),
                generalInput.GravitationalAcceleration,
                generalInput.ModelFactorOvertoppingFlow.Mean, generalInput.ModelFactorOvertoppingFlow.StandardDeviation,
                calculation.InputParameters.LevelOfCrestOfStructure.Mean, calculation.InputParameters.LevelOfCrestOfStructure.StandardDeviation,
                calculation.InputParameters.OrientationOfTheNormalOfTheStructure,
                calculation.InputParameters.ModelFactorOvertoppingSuperCriticalFlow.Mean, calculation.InputParameters.ModelFactorOvertoppingSuperCriticalFlow.StandardDeviation,
                calculation.InputParameters.AllowableIncreaseOfLevelForStorage.Mean, calculation.InputParameters.AllowableIncreaseOfLevelForStorage.StandardDeviation,
                generalInput.ModelFactorForStorageVolume.Mean, generalInput.ModelFactorForStorageVolume.StandardDeviation,
                calculation.InputParameters.StorageStructureArea.Mean, calculation.InputParameters.StorageStructureArea.GetVariationCoefficient(),
                generalInput.ModelFactorForIncomingFlowVolume,
                calculation.InputParameters.FlowWidthAtBottomProtection.Mean, calculation.InputParameters.FlowWidthAtBottomProtection.StandardDeviation,
                calculation.InputParameters.CriticalOvertoppingDischarge.Mean, calculation.InputParameters.CriticalOvertoppingDischarge.GetVariationCoefficient(),
                calculation.InputParameters.FailureProbabilityOfStructureGivenErosion,
                calculation.InputParameters.WidthOfFlowApertures.Mean, calculation.InputParameters.WidthOfFlowApertures.GetVariationCoefficient(),
                calculation.InputParameters.DeviationOfTheWaveDirection,
                calculation.InputParameters.StormDuration.Mean, calculation.InputParameters.StormDuration.GetVariationCoefficient());
        }

        private static List<string> ValidateInput(HeightStructuresInput inputParameters, IAssessmentSection assessmentSection)
        {
            List<string> validationResult = new List<string>();

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult;
        }
    }
}