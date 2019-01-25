// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Class containing the results of a Hydra-Ring calculation for the sub failure mechanism overtopping.
    /// </summary>
    public class OvertoppingCalculationWaveHeightOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="OvertoppingCalculationWaveHeightOutput"/>.
        /// </summary>
        /// <param name="waveHeight">The resulting wave height.</param>
        /// <param name="isOvertoppingDominant">The value indicating whether the overtopping 
        /// sub failure mechanism was dominant over the overflow sub failure mechanism.</param>
        public OvertoppingCalculationWaveHeightOutput(double waveHeight, bool isOvertoppingDominant)
        {
            WaveHeight = waveHeight;
            IsOvertoppingDominant = isOvertoppingDominant;
        }

        /// <summary>
        /// Gets the wave height that was a result of the overtopping calculation.
        /// </summary>
        public double WaveHeight { get; }

        /// <summary>
        /// Gets whether the overtopping sub failure mechanism was dominant over
        /// the overflow sub failure mechanism.
        /// </summary>
        public bool IsOvertoppingDominant { get; }
    }
}