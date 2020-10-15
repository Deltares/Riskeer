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
using Riskeer.Piping.Data.SemiProbabilistic;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Factory for creating <see cref="SemiProbabilisticPipingOutput"/> that can be used
    /// for test purposes.
    /// </summary>
    public static class SemiProbabilisticPipingOutputTestFactory
    {
        /// <summary>
        /// Creates <see cref="SemiProbabilisticPipingOutput"/> with random values.
        /// </summary>
        /// <returns>The created <see cref="SemiProbabilisticPipingOutput"/>.</returns>
        public static SemiProbabilisticPipingOutput Create()
        {
            var random = new Random(39);

            return new SemiProbabilisticPipingOutput(new SemiProbabilisticPipingOutput.ConstructionProperties
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
    }
}