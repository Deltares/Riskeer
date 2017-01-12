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

using System;
using Core.Common.Base.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO
{
    /// <summary>
    /// Class for storing the wave conditions data to be exported.
    /// </summary>
    public class ExportableWaveConditions
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableWaveConditions"/>.
        /// </summary>
        /// <param name="name">The name of the parent calculation.</param>
        /// <param name="waveConditionsInput">The input parameters of the parent calculation.</param>
        /// <param name="waveConditionsOutput">The output parameters of the parent calculation.</param>
        /// <param name="coverType">The type of dike cover.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>, <paramref name="waveConditionsInput"/>, or 
        /// <paramref name="waveConditionsOutput"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> for <paramref name="waveConditionsInput"/>.</exception>
        public ExportableWaveConditions(string name, WaveConditionsInput waveConditionsInput, WaveConditionsOutput waveConditionsOutput, CoverType coverType)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }
            if (waveConditionsOutput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsOutput));
            }

            if (waveConditionsInput.HydraulicBoundaryLocation == null)
            {
                throw new ArgumentException(@"HydraulicBoundaryLocation is null.", "waveConditionsInput");
            }

            CalculationName = name;
            LocationName = waveConditionsInput.HydraulicBoundaryLocation.Name;
            LocationXCoordinate = waveConditionsInput.HydraulicBoundaryLocation.Location.X;
            LocationYCoordinate = waveConditionsInput.HydraulicBoundaryLocation.Location.Y;
            UseForeshore = waveConditionsInput.UseForeshore;
            if (UseForeshore)
            {
                ForeshoreName = waveConditionsInput.ForeshoreProfile.Name;
            }
            UseBreakWater = waveConditionsInput.UseBreakWater;
            CoverType = coverType;
            WaterLevel = waveConditionsOutput.WaterLevel;
            WaveHeight = waveConditionsOutput.WaveHeight;
            WavePeriod = waveConditionsOutput.WavePeakPeriod;
            WaveAngle = waveConditionsOutput.WaveAngle;
            WaveDirection = waveConditionsOutput.WaveDirection;
        }

        /// <summary>
        /// Gets the wave angle with respect to the dike normal.
        /// </summary>
        public RoundedDouble WaveAngle { get; private set; }

        /// <summary>
        /// Gets the wave direction with respect to North.
        /// </summary>
        public RoundedDouble WaveDirection { get; private set; }

        /// <summary>
        /// Gets the wave period.
        /// </summary>
        public RoundedDouble WavePeriod { get; private set; }

        /// <summary>
        /// Gets the wave height.
        /// </summary>
        public RoundedDouble WaveHeight { get; private set; }

        /// <summary>
        /// Gets the water level.
        /// </summary>
        public RoundedDouble WaterLevel { get; private set; }

        /// <summary>
        /// Gets the type of dike cover.
        /// </summary>
        public CoverType CoverType { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a foreshore profile was used in the calculation.
        /// </summary>
        public bool UseForeshore { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a break water was used in the calculation. 
        /// </summary>
        public bool UseBreakWater { get; private set; }

        /// <summary>
        /// Gets the name of the foreshore.
        /// </summary>
        public string ForeshoreName { get; private set; }

        /// <summary>
        /// Gets the y coordinate of the location.
        /// </summary>
        public double LocationYCoordinate { get; private set; }

        /// <summary>
        /// Gets the x coordinate of the location.
        /// </summary>
        public double LocationXCoordinate { get; private set; }

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public string LocationName { get; private set; }

        /// <summary>
        /// Gets the name of the calculation.
        /// </summary>
        public string CalculationName { get; private set; }
    }
}