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

using System.Collections.Generic;
using System.Linq;
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data.TestUtil
{
    /// <summary>
    /// Factory that creates simple instances of <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>
    /// that can be used in tests.
    /// </summary>
    public static class GrassCoverErosionOutwardsWaveConditionsOutputTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>
        /// with default output.
        /// </summary>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.</returns>
        public static GrassCoverErosionOutwardsWaveConditionsOutput Create()
        {
            return new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>());
        }

        /// <summary>
        /// Creates a new instance of <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>
        /// with given output.
        /// </summary>
        /// <param name="waveRunUpOutput">The wave run up output.</param>
        /// <param name="waveImpactOutput">The wave impact output.</param>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.</returns>
        public static GrassCoverErosionOutwardsWaveConditionsOutput Create(IEnumerable<WaveConditionsOutput> waveRunUpOutput, 
                                                                           IEnumerable<WaveConditionsOutput> waveImpactOutput)
        {
            return new GrassCoverErosionOutwardsWaveConditionsOutput(waveRunUpOutput, waveRunUpOutput);
        }
    }
}