﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Revetment.Data
{
    /// <summary>
    /// Class containing helper methods related to <see cref="WaveConditionsInput"/>.
    /// </summary>
    public static class WaveConditionsInputHelper
    {
        private const double assessmentLevelSubtraction = 0.01;

        /// <summary>
        /// Gets an upper boundary based on the provided assessment level (that can be used while
        /// determining water levels for wave condition calculations).
        /// </summary>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <returns>The corresponding assessment level upper boundary.</returns>
        public static RoundedDouble GetUpperBoundaryAssessmentLevel(RoundedDouble assessmentLevel)
        {
            return new RoundedDouble(2, assessmentLevel - assessmentLevelSubtraction);
        }

        /// <summary>
        /// Sets the <see cref="WaveConditionsInputWaterLevelType"/> of the <paramref name="waveConditionsInput"/>
        /// based on the <see cref="NormativeProbabilityType"/>.
        /// </summary>
        /// <param name="waveConditionsInput">The <see cref="WaveConditionsInput"/> to set the water level type for.</param>
        /// <param name="normativeProbabilityType">The <see cref="NormativeProbabilityType"/> to set the <paramref name="waveConditionsInput"/> for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normativeProbabilityType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normativeProbabilityType"/> is a valid value,
        /// but unsupported.</exception>
        public static void SetWaterLevelType(WaveConditionsInput waveConditionsInput,
                                             NormativeProbabilityType normativeProbabilityType)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (!Enum.IsDefined(typeof(NormativeProbabilityType), normativeProbabilityType))
            {
                throw new InvalidEnumArgumentException(nameof(normativeProbabilityType),
                                                       (int) normativeProbabilityType,
                                                       typeof(NormativeProbabilityType));
            }

            switch (normativeProbabilityType)
            {
                case NormativeProbabilityType.MaximumAllowableFloodingProbability:
                    waveConditionsInput.WaterLevelType = WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability;
                    break;
                case NormativeProbabilityType.SignalFloodingProbability:
                    waveConditionsInput.WaterLevelType = WaveConditionsInputWaterLevelType.SignalFloodingProbability;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the hydraulic boundary location calculation that relates to the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The wave conditions input to get the hydraulic boundary location calculation for.</param>
        /// <param name="assessmentSection">The assessment section the wave conditions input belongs to.</param>
        /// <returns>A hydraulic boundary location calculation, or <c>null</c> when:
        /// <list type="bullet">
        /// <item><see cref="WaveConditionsInput.WaterLevelType"/> equals <see cref="WaveConditionsInputWaterLevelType.None"/>;</item>
        /// <item><see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> equals <c>null</c>.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is a valid value, but unsupported.</exception>
        public static HydraulicBoundaryLocationCalculation GetHydraulicBoundaryLocationCalculation(WaveConditionsInput input, IAssessmentSection assessmentSection)
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
                return null;
            }

            switch (input.WaterLevelType)
            {
                case WaveConditionsInputWaterLevelType.None:
                    return null;
                case WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability:
                    return GetRelatedHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, input);
                case WaveConditionsInputWaterLevelType.SignalFloodingProbability:
                    return GetRelatedHydraulicBoundaryLocationCalculation(assessmentSection.WaterLevelCalculationsForSignalingNorm, input);
                case WaveConditionsInputWaterLevelType.UserDefinedTargetProbability:
                    return GetRelatedHydraulicBoundaryLocationCalculation(input.CalculationsTargetProbability.HydraulicBoundaryLocationCalculations, input);
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Gets the target probability to use during wave conditions calculations based on the provided <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The wave conditions input to get the target probability for.</param>
        /// <param name="assessmentSection">The assessment section the wave conditions input belongs to.</param>
        /// <returns>A target probability, or <see cref="double.NaN"/> when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// equals <see cref="WaveConditionsInputWaterLevelType.None"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
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
                case WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability:
                    return assessmentSection.FailureMechanismContribution.LowerLimitNorm;
                case WaveConditionsInputWaterLevelType.SignalFloodingProbability:
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
        /// <returns>An assessment level, or <see cref="RoundedDouble.NaN"/> when:
        /// <list type="bullet">
        /// <item><see cref="WaveConditionsInput.WaterLevelType"/> equals <see cref="WaveConditionsInputWaterLevelType.None"/>;</item>
        /// <item><see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> equals <c>null</c>;</item>
        /// <item>no assessment level is calculated door the selected <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/>.</item>
        /// </list>
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="WaveConditionsInput.WaterLevelType"/>
        /// is a valid value, but unsupported.</exception>
        public static RoundedDouble GetAssessmentLevel(WaveConditionsInput input, IAssessmentSection assessmentSection)
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
                return RoundedDouble.NaN;
            }

            switch (input.WaterLevelType)
            {
                case WaveConditionsInputWaterLevelType.None:
                    return RoundedDouble.NaN;
                case WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, input);
                case WaveConditionsInputWaterLevelType.SignalFloodingProbability:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(assessmentSection.WaterLevelCalculationsForSignalingNorm, input);
                case WaveConditionsInputWaterLevelType.UserDefinedTargetProbability:
                    return GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(input.CalculationsTargetProbability.HydraulicBoundaryLocationCalculations, input);
                default:
                    throw new NotSupportedException();
            }
        }

        private static HydraulicBoundaryLocationCalculation GetRelatedHydraulicBoundaryLocationCalculation(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, WaveConditionsInput input)
        {
            return calculations.First(c => ReferenceEquals(c.HydraulicBoundaryLocation, input.HydraulicBoundaryLocation));
        }

        private static RoundedDouble GetAssessmentLevelFromHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations, WaveConditionsInput input)
        {
            return GetRelatedHydraulicBoundaryLocationCalculation(calculations, input).Output?.Result ?? RoundedDouble.NaN;
        }
    }
}