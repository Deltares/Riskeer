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
using Core.Common.Base.Data;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Service
{
    /// <summary>
    /// Service for synchronizing common data 
    /// </summary>
    public static class RingtoetsCommonDataSynchronizationService
    {
        /// <summary>
        /// Clears the output design water level
        /// of the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="HydraulicBoundaryLocation"/> 
        /// to clear the output for</param>.
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> 
        /// is <c>null</c>.</exception>
        public static void ClearDesignWaterLevel(HydraulicBoundaryLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            location.DesignWaterLevel = RoundedDouble.NaN;
        }

        /// <summary>
        /// Clears the output WaveHeight 
        /// of the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="location">The <see cref="HydraulicBoundaryLocation"/> 
        /// to clear the output for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="location"/> 
        /// is <c>null</c>.</exception>
        public static void ClearWaveHeight(HydraulicBoundaryLocation location)
        {
            if (location == null)
            {
                throw new ArgumentNullException("location");
            }

            location.WaveHeight = RoundedDouble.NaN;
        }
    }
}