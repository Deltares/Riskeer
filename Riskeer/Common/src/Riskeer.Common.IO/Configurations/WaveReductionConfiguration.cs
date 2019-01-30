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

namespace Riskeer.Common.IO.Configurations
{
    /// <summary>
    /// Configuration of the wave reduction.
    /// </summary>
    public class WaveReductionConfiguration
    {
        /// <summary>
        /// Gets or sets the value indicating whether to use a break water.
        /// </summary>
        public bool? UseBreakWater { get; set; }

        /// <summary>
        /// Gets or sets the type of the break water.
        /// </summary>
        public ConfigurationBreakWaterType? BreakWaterType { get; set; }

        /// <summary>
        /// Gets or sets the height of the break water.
        /// </summary>
        public double? BreakWaterHeight { get; set; }

        /// <summary>
        /// Gets or sets the value indicating whether to use the foreshore profile.
        /// </summary>
        public bool? UseForeshoreProfile { get; set; }
    }
}