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

namespace Riskeer.ClosingStructures.IO.Configurations
{
    /// <summary>
    /// Defines the types of the various <see cref="ClosingStructuresCalculationConfiguration"/> inflow models.
    /// </summary>
    public enum ConfigurationClosingStructureInflowModelType
    {
        /// <summary>
        /// A vertical wall.
        /// </summary>
        VerticalWall = 1,

        /// <summary>
        /// A low sill structure.
        /// </summary>
        LowSill = 2,

        /// <summary>
        /// A flooded culvert structure.
        /// </summary>
        FloodedCulvert = 3
    }
}