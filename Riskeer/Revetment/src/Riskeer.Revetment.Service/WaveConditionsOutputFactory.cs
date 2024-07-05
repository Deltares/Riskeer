﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Core.Common.Util;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Service;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.Service
{
    /// <summary>
    /// Class for creating the <see cref="WaveConditionsOutput"/>.
    /// </summary>
    public static class WaveConditionsOutputFactory
    {
        /// <summary>
        /// Creates the <see cref="WaveConditionsOutput"/> based on the provided parameters.
        /// </summary>
        /// <param name="waterLevel">The water level that was calculated for.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle with respect to the dike normal.</param>
        /// <param name="waveDirection">The calculated wave direction with respect to the North.</param>
        /// <param name="targetProbability">The target probability that was calculated for.</param>
        /// <param name="calculatedReliability">The calculated reliability.</param>
        /// <param name="calculatedConvergence">The calculated convergence value.</param>
        /// <returns>The created <see cref="WaveConditionsOutput"/>.</returns>
        public static WaveConditionsOutput CreateOutput(double waterLevel, double waveHeight,
                                                        double wavePeakPeriod, double waveAngle,
                                                        double waveDirection, double targetProbability,
                                                        double calculatedReliability, bool? calculatedConvergence)
        {
            double targetReliability = StatisticsConverter.ProbabilityToReliability(targetProbability);
            double calculatedProbability = StatisticsConverter.ReliabilityToProbability(calculatedReliability);
            CalculationConvergence convergence = RiskeerCommonDataCalculationService.GetCalculationConvergence(calculatedConvergence);

            return new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection, targetProbability,
                                            targetReliability, calculatedProbability, calculatedReliability, convergence);
        }

        /// <summary>
        /// Creates the <see cref="WaveConditionsOutput"/> based on the provided parameters of
        /// a failed calculation.
        /// </summary>
        /// <param name="waterLevel">The water level that was calculated for.</param>
        /// <param name="targetProbability">The target probability that was calculated for.</param>
        /// <returns>The created <see cref="WaveConditionsOutput"/>.</returns>
        public static WaveConditionsOutput CreateFailedOutput(double waterLevel,
                                                              double targetProbability)
        {
            double targetReliability = StatisticsConverter.ProbabilityToReliability(targetProbability);

            return new WaveConditionsOutput(waterLevel,
                                            double.NaN,
                                            double.NaN,
                                            double.NaN,
                                            double.NaN,
                                            targetProbability,
                                            targetReliability,
                                            double.NaN,
                                            double.NaN,
                                            CalculationConvergence.CalculatedNotConverged);
        }
    }
}