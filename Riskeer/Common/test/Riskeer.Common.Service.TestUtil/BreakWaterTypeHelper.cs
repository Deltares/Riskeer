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

using Riskeer.Common.Data.DikeProfiles;

namespace Riskeer.Common.Service.TestUtil
{
    /// <summary>
    /// A helper to be used while asserting Hydra-Ring break water types.
    /// </summary>
    public static class BreakWaterTypeHelper
    {
        /// <summary>
        /// Gets the Hydra-Ring integer value corresponding to <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="BreakWaterType"/> to get the Hydra-Ring value for.</param>
        /// <returns>A Hydra-Ring specific integer value.</returns>
        public static int GetHydraRingBreakWaterType(BreakWaterType type)
        {
            switch (type)
            {
                case BreakWaterType.Caisson:
                    return 1;
                case BreakWaterType.Wall:
                    return 2;
                case BreakWaterType.Dam:
                    return 3;
                default:
                    return 0;
            }
        }
    }
}