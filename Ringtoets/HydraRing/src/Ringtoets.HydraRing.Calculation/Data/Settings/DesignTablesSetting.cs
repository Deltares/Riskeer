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

namespace Riskeer.HydraRing.Calculation.Data.Settings
{
    /// <summary>
    /// Container for design tables settings.
    /// </summary>
    public class DesignTablesSetting
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DesignTablesSetting"/> class.
        /// </summary>
        /// <param name="valueMin">The minimum value to use while iterating towards a reliability index.</param>
        /// <param name="valueMax">The maximum value to use while iterating towards a reliability index.</param>
        public DesignTablesSetting(double valueMin, double valueMax)
        {
            ValueMin = valueMin;
            ValueMax = valueMax;
        }

        /// <summary>
        /// Gets the minimum value to use while iterating towards a reliability index.
        /// </summary>
        /// <remarks>Only relevant for calculations that iterate towards a reliability index.</remarks>
        public double ValueMin { get; }

        /// <summary>
        /// Gets the maximum value to use while iterating towards a reliability index.
        /// </summary>
        /// <remarks>Only relevant for calculations that iterate towards a reliability index.</remarks>
        public double ValueMax { get; }
    }
}