﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Properties;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for grass cover erosion inwards calculations.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationService
    {
        private static readonly ILog grassCoverErosionInwardsCalculationLogger = LogManager.GetLogger(typeof(GrassCoverErosionInwardsCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>False</c> if <paramref name="calculation"/> contains validation errors; <c>True</c> otherwise.</returns>
        internal static bool Validate(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection)
        {
            grassCoverErosionInwardsCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Validation_Subject_0_started_Time_1_,
                                                                 calculation.Name, DateTimeService.CurrentTimeAsString));

            var inputValidationResults = ValidateInput(calculation.InputParameters, assessmentSection);

            if (inputValidationResults.Count > 0)
            {
                LogMessagesAsError(RingtoetsCommonServiceResources.Error_in_validation_0, inputValidationResults.ToArray());
            }

            LogValidationEndTime(calculation);

            return inputValidationResults.Count == 0;
        }

        /// <summary>
        /// Performs a height structures calculation based on the supplied <see cref="GrassCoverErosionInwardsCalculation"/> and sets <see cref="GrassCoverErosionInwardsCalculation.Output"/>
        /// if the calculation was successful. Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> to base the input for the calculation upon.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="generalInput">The <see cref="GeneralGrassCoverErosionInwardsInput"/> to create the input with for the calculation.</param>
        /// <returns>A <see cref="ExceedanceProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        internal static ExceedanceProbabilityCalculationOutput Calculate(GrassCoverErosionInwardsCalculation calculation,
                                                                         string hlcdDirectory, FailureMechanismSection failureMechanismSection,
                                                                         string ringId, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            grassCoverErosionInwardsCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_started_Time_1_,
                                                                 calculation.Name, DateTimeService.CurrentTimeAsString));

            try
            {
                var input = CreateInput(calculation, failureMechanismSection, generalInput);
                var output = HydraRingCalculationService.PerformCalculation(hlcdDirectory, ringId, HydraRingTimeIntegrationSchemeType.FBC, HydraRingUncertaintiesType.All, input);

                if (output == null)
                {
                    grassCoverErosionInwardsCalculationLogger.ErrorFormat(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_grass_cover_erosion_inwards_0_calculation, calculation.Name);
                }

                return output;
            }
            finally
            {
                grassCoverErosionInwardsCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Calculation_Subject_0_ended_Time_1_,
                                                                     calculation.Name, DateTimeService.CurrentTimeAsString));
            }
        }

        private static OvertoppingCalculationInput CreateInput(GrassCoverErosionInwardsCalculation calculation, FailureMechanismSection failureMechanismSection, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            return new OvertoppingCalculationInput((int) calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                   new HydraRingSection(1, failureMechanismSection.Name, failureMechanismSection.GetSectionLength(), calculation.InputParameters.Orientation),
                                                   calculation.InputParameters.DikeHeight,
                                                   generalInput.CriticalOvertoppingModelFactor,
                                                   generalInput.FbFactor.Mean, generalInput.FbFactor.StandardDeviation,
                                                   generalInput.FnFactor.Mean, generalInput.FnFactor.StandardDeviation,
                                                   generalInput.OvertoppingModelFactor,
                                                   calculation.InputParameters.CriticalFlowRate.Mean, calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                   generalInput.FrunupModelFactor.Mean, generalInput.FrunupModelFactor.StandardDeviation,
                                                   generalInput.FshallowModelFactor.Mean, generalInput.FshallowModelFactor.StandardDeviation,
                                                   ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                   ParseForeshore(calculation.InputParameters),
                                                   ParseBreakWater(calculation.InputParameters));
        }

        private static HydraRingBreakWater ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int)input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            var firstProfileSection = input.ForeshoreGeometry.FirstOrDefault();
            if (!input.UseForeshore || firstProfileSection == null)
            {
                yield break;
            }

            yield return new HydraRingForelandPoint(firstProfileSection.StartingPoint.X, firstProfileSection.StartingPoint.Y);

            foreach (var foreshore in input.ForeshoreGeometry)
            {
                yield return new HydraRingForelandPoint(foreshore.EndingPoint.X, foreshore.EndingPoint.Y);
            }
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(IEnumerable<RoughnessProfileSection> profileSections)
        {
            var firstProfileSection = profileSections.FirstOrDefault();
            if (firstProfileSection == null)
            {
                yield break;
            }

            // By default, the roughness is 1.0 (no reduction due to bed friction).
            yield return new HydraRingRoughnessProfilePoint(firstProfileSection.StartingPoint.X, firstProfileSection.StartingPoint.Y, 1);

            foreach (var profileSection in profileSections)
            {
                yield return new HydraRingRoughnessProfilePoint(profileSection.EndingPoint.X, profileSection.EndingPoint.Y, profileSection.Roughness);
            }
        }

        private static List<string> ValidateInput(GrassCoverErosionInwardsInput inputParameters, IAssessmentSection assessmentSection)
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

        private static void LogMessagesAsError(string format, params string[] errorMessages)
        {
            foreach (var errorMessage in errorMessages)
            {
                grassCoverErosionInwardsCalculationLogger.ErrorFormat(format, errorMessage);
            }
        }

        private static void LogValidationEndTime(GrassCoverErosionInwardsCalculation calculation)
        {
            grassCoverErosionInwardsCalculationLogger.Info(string.Format(RingtoetsCommonServiceResources.Validation_Subject_0_ended_Time_1_,
                                                                 calculation.Name, DateTimeService.CurrentTimeAsString));
        }
    }
}