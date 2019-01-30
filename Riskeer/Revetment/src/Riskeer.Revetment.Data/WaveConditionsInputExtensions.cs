// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Base.Data;

namespace Riskeer.Revetment.Data
{
    /// <summary>
    /// Extension methods for <see cref="WaveConditionsInput"/>.
    /// </summary>
    public static class WaveConditionsInputExtensions
    {
        /// <summary>
        /// Gets the water levels to perform a wave conditions calculation for.
        /// </summary>
        /// <param name="waveConditionsInput">The wave conditions input.</param>
        /// <param name="assessmentLevel">The assessment level to use.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> containing water levels.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> is <c>null</c>.</exception>
        public static IEnumerable<RoundedDouble> GetWaterLevels(this WaveConditionsInput waveConditionsInput,
                                                                RoundedDouble assessmentLevel)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            var waterLevels = new List<RoundedDouble>();

            var upperBoundary = new RoundedDouble(2, Math.Min(WaveConditionsInputHelper.GetUpperBoundaryAssessmentLevel(assessmentLevel),
                                                              Math.Min(waveConditionsInput.UpperBoundaryRevetment,
                                                                       !double.IsNaN(waveConditionsInput.UpperBoundaryWaterLevels)
                                                                           ? waveConditionsInput.UpperBoundaryWaterLevels
                                                                           : double.MaxValue)));

            var lowerBoundary = new RoundedDouble(2, Math.Max(waveConditionsInput.LowerBoundaryRevetment,
                                                              !double.IsNaN(waveConditionsInput.LowerBoundaryWaterLevels)
                                                                  ? waveConditionsInput.LowerBoundaryWaterLevels
                                                                  : double.MinValue));

            if (double.IsNaN(upperBoundary) || double.IsNaN(lowerBoundary) || lowerBoundary >= upperBoundary)
            {
                return waterLevels;
            }

            waterLevels.Add(upperBoundary);

            double stepSizeValue = waveConditionsInput.StepSize.AsValue();
            var currentWaterLevel = new RoundedDouble(2, Math.Ceiling(upperBoundary / stepSizeValue) * stepSizeValue - stepSizeValue);

            while (currentWaterLevel > lowerBoundary)
            {
                waterLevels.Add(currentWaterLevel);
                currentWaterLevel = new RoundedDouble(currentWaterLevel.NumberOfDecimalPlaces, currentWaterLevel - stepSizeValue);
            }

            waterLevels.Add(lowerBoundary);

            return waterLevels;
        }
    }
}