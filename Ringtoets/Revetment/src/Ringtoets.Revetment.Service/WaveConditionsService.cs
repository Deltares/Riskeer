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
using Core.Common.Utils;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Service;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service
{
    /// <summary>
    /// Class for calculating the <see cref="WaveConditionsOutput"/>.
    /// </summary>
    public static class WaveConditionsService
    {
        /// <summary>
        /// Calculates the <see cref="WaveConditionsOutput"/> based on the provided parameters.
        /// </summary>
        /// <param name="waterLevel">The calculated water level.</param>
        /// <param name="waveHeight">The calculated wave height.</param>
        /// <param name="wavePeakPeriod">The calculated wave peak period.</param>
        /// <param name="waveAngle">The calculated wave angle w.r.t the dike normal.</param>
        /// <param name="waveDirection">The calculated wave direction w.r.t. North.</param>
        /// <param name="norm">The target norm to calculate for.</param>
        /// <param name="calculatedReliability">The calculated reliability.</param>
        /// <returns>The calculated <see cref="WaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the target probability or 
        /// calculated probability falls outside the [0.0, 1.0] range and is not <see cref="double.NaN"/>.</exception>
        public static WaveConditionsOutput Calculate(double waterLevel, double waveHeight, double wavePeakPeriod,
                                                     double waveAngle, double waveDirection,
                                                     double norm, double calculatedReliability)
        {
            double targetReliability = StatisticsConverter.ProbabilityToReliability(norm);
            double targetProbability = StatisticsConverter.ReliabilityToProbability(targetReliability);

            double calculatedProbability = StatisticsConverter.ReliabilityToProbability(calculatedReliability);

            CalculationConvergence convergence = RingtoetsCommonDataCalculationService.GetCalculationConvergence(calculatedReliability, norm);

            return new WaveConditionsOutput(waterLevel, waveHeight, wavePeakPeriod, waveAngle, waveDirection, targetProbability,
                                            targetReliability, calculatedProbability, calculatedReliability, convergence);
        }
    }
}