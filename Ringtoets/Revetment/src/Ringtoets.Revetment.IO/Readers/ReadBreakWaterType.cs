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

namespace Ringtoets.Revetment.IO.Readers
{
    /// <summary>
    /// Specifies the breakwater type of a <see cref="ReadWaveConditionsCalculation"/>.
    /// </summary>
    public enum ReadBreakWaterType
    {
        /// <summary>
        /// Indicates there is no breakwater.
        /// </summary>
        None = 0,

        /// <summary>
        /// Indicates there is a caisson-shaped breakwater.
        /// </summary>
        Caisson = 1,

        /// <summary>
        /// Indicates there is a vertical structure-shaped breakwater.
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// Indicates there is a harbor breakwater.
        /// </summary>
        HarborDam = 3
    }
}