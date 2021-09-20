// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Storage.Core.DbContext
{
    /// <summary>
    /// The hydraulic boundary location calculation type that are available within the application.
    /// </summary>
    public enum HydraulicBoundaryLocationCalculationType
    {
        /// <summary>
        /// Corresponds with a water level calculation.
        /// </summary>
        WaterLevel = 1,
        
        /// <summary>
        /// Corresponds with a wave height calculation.
        /// </summary>
        WaveHeight = 2
    }
}