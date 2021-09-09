using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Service
{
    /// <summary>
    /// Helper class containing methods related to wave conditions calculations.
    /// </summary>
    public static class WaveConditionsCalculationHelper
    {
        /// <summary>
        /// Gets the target probability to use during wave conditions calculations based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The wave conditions input to get the target probability for.</param>
        /// <param name="assessmentSection">The assessment section the wave conditions input belongs to.</param>
        /// <returns>A target probability, or <see cref="double.NaN"/> when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// equals <see cref="WaveConditionsInputWaterLevelType.None"/>.</returns>
        /// <exception cref="NullReferenceException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is a valid value, but unsupported.</exception>
        public static double GetTargetProbability(WaveConditionsInput input, IAssessmentSection assessmentSection)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(WaveConditionsInputWaterLevelType), input.WaterLevelType))
            {
                throw new InvalidEnumArgumentException(nameof(input.WaterLevelType),
                                                       (int) input.WaterLevelType,
                                                       typeof(WaveConditionsInputWaterLevelType));
            }

            switch (input.WaterLevelType)
            {
                case WaveConditionsInputWaterLevelType.None:
                    return double.NaN;
                case WaveConditionsInputWaterLevelType.LowerLimit:
                    return assessmentSection.FailureMechanismContribution.LowerLimitNorm;
                case WaveConditionsInputWaterLevelType.Signaling:
                    return assessmentSection.FailureMechanismContribution.SignalingNorm;
                case WaveConditionsInputWaterLevelType.UserDefinedTargetProbability:
                    return input.CalculationsTargetProbability.TargetProbability;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the assessment level to use during wave conditions calculations based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The wave conditions input to get the assessment level for.</param>
        /// <param name="assessmentSection">The assessment section the wave conditions input belongs to.</param>
        /// <returns>An assessment level, or <see cref="double.NaN"/> when:
        /// <list type="bullet">
        /// <item><see cref="WaveConditionsInput.WaterLevelType"/> equals <see cref="WaveConditionsInputWaterLevelType.None"/>;</item>
        /// <item><see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> equals <c>null</c>;</item>
        /// <item>no assessment level is calculated door the selected <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/>.</item>
        /// </list>
        /// </returns>
        /// <exception cref="NullReferenceException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is a valid value, but unsupported.</exception>
        public static double GetAssessmentLevel(WaveConditionsInput input, IAssessmentSection assessmentSection)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (!Enum.IsDefined(typeof(WaveConditionsInputWaterLevelType), input.WaterLevelType))
            {
                throw new InvalidEnumArgumentException(nameof(input.WaterLevelType),
                                                       (int) input.WaterLevelType,
                                                       typeof(WaveConditionsInputWaterLevelType));
            }

            if (input.HydraulicBoundaryLocation == null)
            {
                return double.NaN;
            }

            switch (input.WaterLevelType)
            {
                case WaveConditionsInputWaterLevelType.None:
                    return double.NaN;
                case WaveConditionsInputWaterLevelType.LowerLimit:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, input);
                case WaveConditionsInputWaterLevelType.Signaling:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(assessmentSection.WaterLevelCalculationsForSignalingNorm, input);
                case WaveConditionsInputWaterLevelType.UserDefinedTargetProbability:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(input.CalculationsTargetProbability.HydraulicBoundaryLocationCalculations, input);
                default:
                    throw new NotSupportedException();
            }
        }

        private static double GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, WaveConditionsInput input)
        {
            return calculations.First(c => ReferenceEquals(c.HydraulicBoundaryLocation, input.HydraulicBoundaryLocation)).Output?.Result ?? double.NaN;
        }
    }
}