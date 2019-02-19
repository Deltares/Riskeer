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
using Riskeer.Revetment.Data;

namespace Riskeer.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Factory for creating instances of <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.
    /// </summary>
    public static class GrassCoverErosionOutwardsWaveConditionsOutputFactory
    {
        /// <summary>
        /// Creates <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/> with wave run up output set.
        /// </summary>
        /// <param name="waveRunUpOutput">The wave run up output to set.</param>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveRunUpOutput"/>
        /// is <c>null</c>.</exception>
        public static GrassCoverErosionOutwardsWaveConditionsOutput CreateOutputWithWaveRunUp(IEnumerable<WaveConditionsOutput> waveRunUpOutput)
        {
            if (waveRunUpOutput == null)
            {
                throw new ArgumentNullException(nameof(waveRunUpOutput));
            }

            return new GrassCoverErosionOutwardsWaveConditionsOutput(waveRunUpOutput, null);
        }

        /// <summary>
        /// Creates <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/> with wave impact output set.
        /// </summary>
        /// <param name="waveImpactOutput">The wave impact output to set.</param>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveImpactOutput"/>
        /// is <c>null</c>.</exception>
        public static GrassCoverErosionOutwardsWaveConditionsOutput CreateOutputWithWaveImpact(IEnumerable<WaveConditionsOutput> waveImpactOutput)
        {
            if (waveImpactOutput == null)
            {
                throw new ArgumentNullException(nameof(waveImpactOutput));
            }

            return new GrassCoverErosionOutwardsWaveConditionsOutput(null, waveImpactOutput);
        }

        /// <summary>
        /// Creates <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/> with wave run up and wave impact output set.
        /// </summary>
        /// <param name="waveRunUpOutput">The wave run up output to set.</param>
        /// <param name="waveImpactOutput">The wave impact output to set.</param>
        /// <returns>The created <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static GrassCoverErosionOutwardsWaveConditionsOutput CreateOutputWithWaveRunUpAndWaveImpact(IEnumerable<WaveConditionsOutput> waveRunUpOutput,
                                                                                                           IEnumerable<WaveConditionsOutput> waveImpactOutput)
        {
            if (waveRunUpOutput == null)
            {
                throw new ArgumentNullException(nameof(waveRunUpOutput));
            }

            if (waveImpactOutput == null)
            {
                throw new ArgumentNullException(nameof(waveImpactOutput));
            }

            return new GrassCoverErosionOutwardsWaveConditionsOutput(waveRunUpOutput, waveImpactOutput);
        }
    }
}