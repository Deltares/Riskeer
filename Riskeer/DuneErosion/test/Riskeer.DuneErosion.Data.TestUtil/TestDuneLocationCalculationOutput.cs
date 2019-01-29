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

using Ringtoets.Common.Data.Hydraulics;

namespace Riskeer.DuneErosion.Data.TestUtil
{
    /// <summary>
    /// Class that creates simple instances of <see cref="DuneLocationCalculationOutput"/> that
    /// can be used during testing.
    /// </summary>
    public class TestDuneLocationCalculationOutput : DuneLocationCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestDuneLocationCalculationOutput"/>.
        /// </summary>
        public TestDuneLocationCalculationOutput()
            : this(0, 0, 0) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestDuneLocationCalculationOutput"/>.
        /// </summary>
        /// <param name="waterLevel">The water level of the calculation.</param>
        /// <param name="waveHeight">The wave height of the calculation.</param>
        /// <param name="wavePeriod">The wave period of the calculation.</param>
        public TestDuneLocationCalculationOutput(double waterLevel, double waveHeight, double wavePeriod)
            : base(CalculationConvergence.CalculatedConverged, new ConstructionProperties
            {
                WaterLevel = waterLevel,
                WavePeriod = wavePeriod,
                WaveHeight = waveHeight,
                TargetProbability = 0,
                CalculatedReliability = 0,
                TargetReliability = 0,
                CalculatedProbability = 0
            }) {}
    }
}