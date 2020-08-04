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

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Factory for creating <see cref="PipingOutput"/> that can be used
    /// for test purposes.
    /// </summary>
    public static class PipingOutputTestFactory
    {
        /// <summary>
        /// Creates <see cref="PipingOutput"/> with random values.
        /// </summary>
        /// <returns>The created <see cref="PipingOutput"/>.</returns>
        public static PipingOutput Create()
        {
            var random = new Random(39);

            return new PipingOutput(new PipingOutput.ConstructionProperties
            {
                HeaveFactorOfSafety = random.NextDouble(),
                UpliftEffectiveStress = random.NextDouble(),
                UpliftFactorOfSafety = random.NextDouble(),
                SellmeijerFactorOfSafety = random.NextDouble(),
                HeaveGradient = random.NextDouble(),
                SellmeijerCreepCoefficient = random.NextDouble(),
                SellmeijerCriticalFall = random.NextDouble(),
                SellmeijerReducedFall = random.NextDouble()
            });
        }

        /// <summary>
        /// Creates output with the given values.
        /// </summary>
        /// <param name="heaveFactorOfSafety">The heave factor of safety.</param>
        /// <param name="upliftFactorOfSafety">The uplift factor of safety.</param>
        /// <param name="sellmeijerFactorOfSafety">The sellmeijer factor of safety.</param>
        /// <returns>The created <see cref="PipingOutput"/>.</returns>
        public static PipingOutput Create(double heaveFactorOfSafety, double upliftFactorOfSafety, double sellmeijerFactorOfSafety)
        {
            return new PipingOutput(new PipingOutput.ConstructionProperties
            {
                HeaveFactorOfSafety = heaveFactorOfSafety,
                UpliftFactorOfSafety = upliftFactorOfSafety,
                SellmeijerFactorOfSafety = sellmeijerFactorOfSafety
            });
        }
    }
}