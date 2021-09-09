using System;
using System.ComponentModel;
using Riskeer.Common.Data.AssessmentSection;
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
        /// <returns>A target probability.</returns>
        /// <exception cref="NullReferenceException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is a valid value, but unsupported.</exception>
        public static double GetTargetProbability(WaveConditionsInput input, IAssessmentSection assessmentSection)
        {
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
                case WaveConditionsInputWaterLevelType.Signaling:
                    return assessmentSection.FailureMechanismContribution.SignalingNorm;
                case WaveConditionsInputWaterLevelType.LowerLimit:
                    return assessmentSection.FailureMechanismContribution.LowerLimitNorm;
                case WaveConditionsInputWaterLevelType.UserDefinedTargetProbability:
                    return input.CalculationsTargetProbability.TargetProbability;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}