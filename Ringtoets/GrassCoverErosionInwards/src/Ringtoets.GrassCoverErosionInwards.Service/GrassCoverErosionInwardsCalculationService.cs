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
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Service.Properties;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Input.Overtopping;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Calculation.Parsers;
using Ringtoets.HydraRing.Calculation.Services;
using Ringtoets.HydraRing.IO;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Service
{
    /// <summary>
    /// Service that provides methods for performing Hydra-Ring calculations for grass cover erosion inwards calculations.
    /// </summary>
    public static class GrassCoverErosionInwardsCalculationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionInwardsCalculationService));

        /// <summary>
        /// Performs validation over the values on the given <paramref name="calculation"/>. Error and status information is logged during
        /// the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> for which to validate the values.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> for which to validate the values.</param>
        /// <returns><c>True</c>c> if <paramref name="calculation"/> has no validation errors; <c>False</c>c> otherwise.</returns>
        public static bool Validate(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection)
        {
            return CalculationServiceHelper.PerformValidation(calculation.Name, () => ValidateInput(calculation.InputParameters, assessmentSection));
        }

        /// <summary>
        /// Performs a grass cover erosion inwards calculation based on the supplied <see cref="GrassCoverErosionInwardsCalculation"/> 
        /// and sets <see cref="GrassCoverErosionInwardsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="generalInput">Calculation input parameters that apply to all <see cref="GrassCoverErosionInwardsCalculation"/> instances.</param>
        /// <returns>A <see cref="ExceedanceProbabilityCalculationOutput"/> on a successful calculation, <c>null</c> otherwise.</returns>
        internal static GrassCoverErosionInwardsCalculationServiceOutput CalculateProbability(GrassCoverErosionInwardsCalculation calculation,
                                                                                              string hlcdDirectory, FailureMechanismSection failureMechanismSection,
                                                                                              string ringId, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            OvertoppingCalculationInput overtoppingCalculationInput = CreateOvertoppingInput(calculation, failureMechanismSection, generalInput);
            var exceedanceProbabilityCalculationParser = new ExceedanceProbabilityCalculationParser();
            var waveHeightParser = new OvertoppingCalculationWaveHeightParser();

            HydraRingCalculationService.PerformCalculation(
                hlcdDirectory,
                ringId,
                HydraRingUncertaintiesType.All,
                overtoppingCalculationInput,
                new IHydraRingFileParser[]
                {
                    exceedanceProbabilityCalculationParser,
                    waveHeightParser
                });

            VerifyOvertoppingCalculationOutput(exceedanceProbabilityCalculationParser.Output, waveHeightParser.Output, calculation.Name);

            return exceedanceProbabilityCalculationParser.Output != null && waveHeightParser.Output != null ?
                       new GrassCoverErosionInwardsCalculationServiceOutput(
                           exceedanceProbabilityCalculationParser.Output.Beta,
                           waveHeightParser.Output.WaveHeight,
                           waveHeightParser.Output.IsOvertoppingDominant,
                           null) :
                       null;
        }

        /// <summary>
        /// Performs a grass cover erosion inwards dike height calculation based on the supplied <see cref="GrassCoverErosionInwardsCalculation"/> 
        /// and sets <see cref="GrassCoverErosionInwardsCalculation.Output"/> if the calculation was successful. 
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="calculation">The <see cref="GrassCoverErosionInwardsCalculation"/> that holds all the information required to perform the calculation.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> that holds information about the norm used in the calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="failureMechanismSection">The <see cref="FailureMechanismSection"/> to create input with.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="generalInput">Calculation input parameters that apply to all <see cref="GrassCoverErosionInwardsCalculation"/> instances.</param>
        /// <returns>A double with a  value on a successful calculation, <c>double.NaN</c> otherwise.</returns>
        internal static double CalculateDikeHeight(GrassCoverErosionInwardsCalculation calculation,
                                                   IAssessmentSection assessmentSection,
                                                   string hlcdDirectory, FailureMechanismSection failureMechanismSection,
                                                   string ringId, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            var targetProbabiltyCalculationParser = new ReliabilityIndexCalculationParser();

            DikeHeightCalculationInput dikeHeightCalculationInput = CreateDikeHeightInput(calculation, assessmentSection, failureMechanismSection, generalInput);

            HydraRingCalculationService.PerformCalculation(
                hlcdDirectory,
                ringId,
                HydraRingUncertaintiesType.All,
                dikeHeightCalculationInput,
                new IHydraRingFileParser[]
                {
                    targetProbabiltyCalculationParser
                });

            VerifyDikeHeightCalculationOutput(targetProbabiltyCalculationParser.Output, calculation.Name);

            return targetProbabiltyCalculationParser.Output == null ? double.NaN : targetProbabiltyCalculationParser.Output.Result;
        }

        private static void VerifyOvertoppingCalculationOutput(ExceedanceProbabilityCalculationOutput exceedanceOutput, OvertoppingCalculationWaveHeightOutput waveHeightOutput, string name)
        {
            if (exceedanceOutput == null || waveHeightOutput == null)
            {
                log.ErrorFormat(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_grass_cover_erosion_inwards_0_calculation, name);
            }
        }

        private static void VerifyDikeHeightCalculationOutput(ReliabilityIndexCalculationOutput output, string name)
        {
            if (output == null)
            {
                log.ErrorFormat(Resources.GrassCoverErosionInwardsCalculationService_Calculate_Error_in_hbn_grass_cover_erosion_inwards_0_calculation, name);
            }
        }

        private static OvertoppingCalculationInput CreateOvertoppingInput(GrassCoverErosionInwardsCalculation calculation,
                                                                          FailureMechanismSection failureMechanismSection,
                                                                          GeneralGrassCoverErosionInwardsInput generalInput)
        {
            return new OvertoppingCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                   new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.Orientation),
                                                   calculation.InputParameters.DikeHeight,
                                                   generalInput.CriticalOvertoppingModelFactor,
                                                   generalInput.FbFactor.Mean,
                                                   generalInput.FbFactor.StandardDeviation,
                                                   generalInput.FnFactor.Mean,
                                                   generalInput.FnFactor.StandardDeviation,
                                                   generalInput.OvertoppingModelFactor,
                                                   calculation.InputParameters.CriticalFlowRate.Mean,
                                                   calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                   generalInput.FrunupModelFactor.Mean,
                                                   generalInput.FrunupModelFactor.StandardDeviation,
                                                   generalInput.FshallowModelFactor.Mean,
                                                   generalInput.FshallowModelFactor.StandardDeviation,
                                                   ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                   ParseForeshore(calculation.InputParameters),
                                                   ParseBreakWater(calculation.InputParameters));
        }

        private static DikeHeightCalculationInput CreateDikeHeightInput(GrassCoverErosionInwardsCalculation calculation, IAssessmentSection assessmentSection,
                                                                        FailureMechanismSection failureMechanismSection, GeneralGrassCoverErosionInwardsInput generalInput)
        {
            return new DikeHeightCalculationInput(calculation.InputParameters.HydraulicBoundaryLocation.Id,
                                                  assessmentSection.FailureMechanismContribution.Norm,
                                                  new HydraRingSection(1, failureMechanismSection.GetSectionLength(), calculation.InputParameters.Orientation),
                                                  generalInput.CriticalOvertoppingModelFactor,
                                                  generalInput.FbFactor.Mean,
                                                  generalInput.FbFactor.StandardDeviation,
                                                  generalInput.FnFactor.Mean,
                                                  generalInput.FnFactor.StandardDeviation,
                                                  generalInput.OvertoppingModelFactor,
                                                  calculation.InputParameters.CriticalFlowRate.Mean,
                                                  calculation.InputParameters.CriticalFlowRate.StandardDeviation,
                                                  generalInput.FrunupModelFactor.Mean,
                                                  generalInput.FrunupModelFactor.StandardDeviation,
                                                  generalInput.FshallowModelFactor.Mean,
                                                  generalInput.FshallowModelFactor.StandardDeviation,
                                                  ParseProfilePoints(calculation.InputParameters.DikeGeometry),
                                                  ParseForeshore(calculation.InputParameters),
                                                  ParseBreakWater(calculation.InputParameters));
        }

        private static HydraRingBreakWater ParseBreakWater(GrassCoverErosionInwardsInput input)
        {
            return input.UseBreakWater ? new HydraRingBreakWater((int) input.BreakWater.Type, input.BreakWater.Height) : null;
        }

        private static IEnumerable<HydraRingForelandPoint> ParseForeshore(GrassCoverErosionInwardsInput input)
        {
            return input.UseForeshore ? input.ForeshoreGeometry.Select(c => new HydraRingForelandPoint(c.X, c.Y)) : new HydraRingForelandPoint[0];
        }

        private static IEnumerable<HydraRingRoughnessProfilePoint> ParseProfilePoints(RoughnessPoint[] roughnessProfilePoints)
        {
            for (var i = 0; i < roughnessProfilePoints.Length; i++)
            {
                var roughnessProfilePoint = roughnessProfilePoints[i];

                if (i == 0)
                {
                    yield return new HydraRingRoughnessProfilePoint(roughnessProfilePoint.Point.X, roughnessProfilePoint.Point.Y, 1.0);
                }
                else
                {
                    var precedingRoughnessProfilePoint = roughnessProfilePoints[i - 1];

                    yield return new HydraRingRoughnessProfilePoint(roughnessProfilePoint.Point.X, roughnessProfilePoint.Point.Y, precedingRoughnessProfilePoint.Roughness);
                }
            }
        }

        private static string[] ValidateInput(GrassCoverErosionInwardsInput inputParameters, IAssessmentSection assessmentSection)
        {
            List<string> validationResult = new List<string>();

            if (inputParameters.HydraulicBoundaryLocation == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_hydraulic_boundary_location_selected);
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);

            if (inputParameters.DikeProfile == null)
            {
                validationResult.Add(RingtoetsCommonServiceResources.CalculationService_ValidateInput_No_dike_profile_selected);
            }

            if (!string.IsNullOrEmpty(validationProblem))
            {
                validationResult.Add(validationProblem);
            }

            return validationResult.ToArray();
        }
    }
}