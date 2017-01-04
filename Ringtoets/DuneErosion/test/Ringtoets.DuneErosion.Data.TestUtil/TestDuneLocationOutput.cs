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

using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.DuneErosion.Data.TestUtil
{
    /// <summary>
    /// Class that creates simple instances of <see cref="DuneLocationOutput"/> that
    /// can be used during testing.
    /// </summary>
    public class TestDuneLocationOutput : DuneLocationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestDuneLocationOutput"/>.
        /// </summary>
        public TestDuneLocationOutput() : this(0, 0, 0) {}

        private TestDuneLocationOutput(double waterLevel, double wavePeriod, double waveHeight)
            : base(CalculationConvergence.CalculatedConverged, new ConstructionProperties
            {
                WaterLevel = waterLevel,
                WavePeriod = wavePeriod,
                WaveHeight = waveHeight,
                TargetProbability = 0,
                TargetReliability = 0,
                CalculatedProbability = 0,
                CalculatedReliability = 0
            }) {}

        /// <summary>
        /// Creates a <see cref="TestDuneLocationOutput"/> with desired values 
        /// that are relevant when exporting a <see cref="DuneLocationOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level.</param>
        /// <param name="wavePeriod">The wave period.</param>
        /// <param name="waveHeight">The wave height.</param>
        /// <returns>A <see cref="TestDuneLocationOutput"/>
        /// with values that are relevant for the export.</returns>
        public static TestDuneLocationOutput CreateDuneLocationOutputForExport(double waterLevel, double wavePeriod, double waveHeight)
        {
            return new TestDuneLocationOutput(waterLevel, wavePeriod, waveHeight);
        }
    }
}