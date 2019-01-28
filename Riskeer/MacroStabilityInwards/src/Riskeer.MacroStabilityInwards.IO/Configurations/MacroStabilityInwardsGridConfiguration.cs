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

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Configuration of a grid.
    /// </summary>
    public class MacroStabilityInwardsGridConfiguration
    {
        /// <summary>
        /// Gets or sets the left boundary of the grid.
        /// </summary>
        public double? XLeft { get; set; }

        /// <summary>
        /// Gets or sets the right boundary of the grid.
        /// </summary>
        public double? XRight { get; set; }

        /// <summary>
        /// Gets or sets the top boundary of the grid.
        /// </summary>
        public double? ZTop { get; set; }

        /// <summary>
        /// Gets or sets the bottom boundary of the grid.
        /// </summary>
        public double? ZBottom { get; set; }

        /// <summary>
        /// Gets or set the number of horizontal points.
        /// </summary>
        public int? NumberOfHorizontalPoints { get; set; }

        /// <summary>
        /// Gets or sets the number of vertical points.
        /// </summary>
        public int? NumberOfVerticalPoints { get; set; }
    }
}