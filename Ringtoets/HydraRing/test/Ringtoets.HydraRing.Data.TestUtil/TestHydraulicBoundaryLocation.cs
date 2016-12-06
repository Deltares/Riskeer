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

using Core.Common.Base.Data;

namespace Ringtoets.HydraRing.Data.TestUtil
{
    /// <summary>
    /// Class which creates simple instances of <see cref="HydraulicBoundaryLocation"/>, which
    /// can be used during testing.
    /// </summary>
    public class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with 
        /// specified design water level.
        /// </summary>
        /// <param name="designWaterLevel">The design water level of the location.</param>
        public TestHydraulicBoundaryLocation(RoundedDouble designWaterLevel) : base(0, string.Empty, 0, 0)
        {
            DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(designWaterLevel, 0, 0, 0, 0, CalculationConvergence.NotCalculated);
            DesignWaterLevel = designWaterLevel;
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>.
        /// </summary>
        public TestHydraulicBoundaryLocation() : this(RoundedDouble.NaN) {}
    }
}