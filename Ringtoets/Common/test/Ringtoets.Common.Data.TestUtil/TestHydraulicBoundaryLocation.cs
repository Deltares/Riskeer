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
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class which creates simple instances of <see cref="HydraulicBoundaryLocation"/>, which
    /// can be used during testing.
    /// </summary>
    public class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>.
        /// </summary>
        public TestHydraulicBoundaryLocation() : this(null, null) { }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.DesignWaterLevelOutput"/> 
        /// and <see cref="HydraulicBoundaryLocation.WaveHeightOutput"/> set.
        /// </summary>
        /// <param name="designWaterLevel">The design water level result to set in the output.</param>
        /// <param name="waveHeight">The waveheight result to set in the output.</param>
        public TestHydraulicBoundaryLocation(double? designWaterLevel = double.NaN, double? waveHeight = double.NaN)
            : base(0, string.Empty, 0, 0)
        {
            if (designWaterLevel.HasValue)
            {
                DesignWaterLevelOutput = new HydraulicBoundaryLocationOutput(designWaterLevel.Value, 0, 0, 0, 0, CalculationConvergence.NotCalculated);
                DesignWaterLevel = (RoundedDouble)designWaterLevel.Value;
            }
            if (waveHeight.HasValue)
            {
                WaveHeightOutput = new HydraulicBoundaryLocationOutput(waveHeight.Value, 0, 0, 0, 0, CalculationConvergence.NotCalculated);
                WaveHeight = (RoundedDouble)waveHeight.Value;
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.DesignWaterLevelOutput"/> set.
        /// </summary>
        /// <param name="designWaterLevel">The design water level result to set in the output.</param>
        /// <returns>A new <see cref="TestHydraulicBoundaryLocation"/>.</returns>
        public static TestHydraulicBoundaryLocation CreateDesignWaterLevelCalculated(double designWaterLevel)
        {
            return new TestHydraulicBoundaryLocation(designWaterLevel, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with <see cref="HydraulicBoundaryLocation.WaveHeightOutput"/> set.
        /// </summary>
        /// <param name="waveheight">The waveheight result to set in the output.</param>
        /// <returns>A new <see cref="TestHydraulicBoundaryLocation"/>.</returns>
        public static TestHydraulicBoundaryLocation CreateWaveHeightCalculated(double waveheight)
        {
            return new TestHydraulicBoundaryLocation(null, waveheight);
        }
    }
}