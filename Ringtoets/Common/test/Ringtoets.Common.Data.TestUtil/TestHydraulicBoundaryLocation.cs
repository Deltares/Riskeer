// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Class that creates simple instances of <see cref="HydraulicBoundaryLocation"/>, that
    /// can be used during testing.
    /// </summary>
    public class TestHydraulicBoundaryLocation : HydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>
        /// with the given name.
        /// </summary>
        /// <param name="name">The name for the <see cref="TestHydraulicBoundaryLocation"/>.</param>
        public TestHydraulicBoundaryLocation(string name) : base(0, name, 0, 0) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/>.
        /// </summary>
        public TestHydraulicBoundaryLocation() : this(null, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with 
        /// <see cref="HydraulicBoundaryLocationCalculation.Output"/> set.
        /// </summary>
        /// <param name="designWaterLevel">The design water level result to set in the output.</param>
        /// <param name="waveHeight">The wave height result to set in the output.</param>
        private TestHydraulicBoundaryLocation(double? designWaterLevel, double? waveHeight)
            : this(string.Empty)
        {
            if (designWaterLevel.HasValue)
            {
                DesignWaterLevelCalculation.Output = new HydraulicBoundaryLocationOutput(
                    designWaterLevel.Value, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged, null);
            }
            if (waveHeight.HasValue)
            {
                WaveHeightCalculation.Output = new HydraulicBoundaryLocationOutput(
                    waveHeight.Value, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged, null);
            }
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with
        /// <see cref="HydraulicBoundaryLocationCalculation.Output"/> set for
        /// <see cref="HydraulicBoundaryLocation.DesignWaterLevelCalculation"/> 
        /// and <see cref="HydraulicBoundaryLocation.WaveHeightCalculation"/>.
        /// </summary>
        /// <returns>A new <see cref="TestHydraulicBoundaryLocation"/>.</returns>
        public static TestHydraulicBoundaryLocation CreateFullyCalculated()
        {
            return new TestHydraulicBoundaryLocation(4.5, 5.5);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with 
        /// <see cref="HydraulicBoundaryLocationCalculation.Output"/> set for
        /// <see cref="HydraulicBoundaryLocation.DesignWaterLevelCalculation"/>.
        /// </summary>
        /// <param name="designWaterLevel">The design water level result to set in the output.</param>
        /// <returns>A new <see cref="TestHydraulicBoundaryLocation"/>.</returns>
        public static TestHydraulicBoundaryLocation CreateDesignWaterLevelCalculated(double designWaterLevel)
        {
            return new TestHydraulicBoundaryLocation(designWaterLevel, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TestHydraulicBoundaryLocation"/> with 
        /// <see cref="HydraulicBoundaryLocationCalculation.Output"/> set for
        /// <see cref="HydraulicBoundaryLocation.WaveHeightCalculation"/>.
        /// </summary>
        /// <param name="waveheight">The wave height result to set in the output.</param>
        /// <returns>A new <see cref="TestHydraulicBoundaryLocation"/>.</returns>
        public static TestHydraulicBoundaryLocation CreateWaveHeightCalculated(double waveheight)
        {
            return new TestHydraulicBoundaryLocation(null, waveheight);
        }
    }
}