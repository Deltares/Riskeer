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

namespace Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase
{
    /// <summary>
    /// Class for holding a mapping between the hydraulic boundary location id and hydraulic location configuration id.
    /// </summary>
    public class ReadHydraulicLocationMapping
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadHydraulicLocationMapping"/>.
        /// </summary>
        /// <param name="hrdLocationId">The hydraulic boundary location id.</param>
        /// <param name="hlcdLocationId">The hydraulic location configuration id.</param>
        internal ReadHydraulicLocationMapping(long hrdLocationId, long hlcdLocationId)
        {
            HrdLocationId = hrdLocationId;
            HlcdLocationId = hlcdLocationId;
        }

        /// <summary>
        /// Gets the hydraulic boundary location id.
        /// </summary>
        public long HrdLocationId { get; }

        /// <summary>
        /// Gets the hydraulic location configuration id.
        /// </summary>
        public long HlcdLocationId { get; }
    }
}