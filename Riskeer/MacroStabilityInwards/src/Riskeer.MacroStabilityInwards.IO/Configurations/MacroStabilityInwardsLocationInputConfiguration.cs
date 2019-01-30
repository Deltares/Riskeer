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

namespace Riskeer.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Configuration of locations input.
    /// </summary>
    public class MacroStabilityInwardsLocationInputConfiguration
    {
        /// <summary>
        /// Gets or sets the polder water level.
        /// </summary>
        public double? WaterLevelPolder { get; set; }

        /// <summary>
        /// Gets or sets whether the default offsets should be used.
        /// </summary>
        public bool? UseDefaultOffsets { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at river.
        /// </summary>
        public double? PhreaticLineOffsetBelowDikeTopAtRiver { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike top at polder.
        /// </summary>
        public double? PhreaticLineOffsetBelowDikeTopAtPolder { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below shoulder base inside.
        /// </summary>
        public double? PhreaticLineOffsetBelowShoulderBaseInside { get; set; }

        /// <summary>
        /// Gets or sets the offset of the phreatic line below dike toe at polder.
        /// </summary>
        public double? PhreaticLineOffsetBelowDikeToeAtPolder { get; set; }
    }
}