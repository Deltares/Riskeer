﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using Riskeer.Revetment.Data;

namespace Riskeer.Revetment.IO.WaveConditions
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
        /// <param name="targetProbability">The target probability to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="WaveConditionsInput.HydraulicBoundaryLocation"/> 
        /// is <c>null</c> in <paramref name="waveConditionsInput"/>.</exception>
        public ExportableWaveConditions(string name,
                                        WaveConditionsInput waveConditionsInput,
                                        WaveConditionsOutput waveConditionsOutput,
                                        CoverType coverType,
                                        string targetProbability)
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

            if (coverType == null)
            {
                throw new ArgumentNullException(nameof(coverType));
            }

            if (targetProbability == null)
            {
                throw new ArgumentNullException(nameof(targetProbability));
            }

            if (waveConditionsInput.HydraulicBoundaryLocation == null)
            {
                throw new ArgumentException(@"HydraulicBoundaryLocation is null.", nameof(waveConditionsInput));
            }

            CalculationName = name;
            LocationName = waveConditionsInput.HydraulicBoundaryLocation.Name;
            LocationXCoordinate = waveConditionsInput.HydraulicBoundaryLocation.Location.X;
            LocationYCoordinate = waveConditionsInput.HydraulicBoundaryLocation.Location.Y;
            if (waveConditionsInput.ForeshoreProfile != null)
            {
                ForeshoreId = waveConditionsInput.ForeshoreProfile.Id;
            }

            UseForeshore = waveConditionsInput.UseForeshore;
            UseBreakWater = waveConditionsInput.UseBreakWater;
            CoverType = coverType;
            TargetProbability = targetProbability;
            WaterLevel = waveConditionsOutput.WaterLevel;
            WaveHeight = waveConditionsOutput.WaveHeight;
            WavePeriod = waveConditionsOutput.WavePeakPeriod;
            WaveAngle = waveConditionsOutput.WaveAngle;
            WaveDirection = waveConditionsOutput.WaveDirection;
        }

        /// <summary>
        /// Gets the wave angle with respect to the dike normal.
        /// </summary>
        public RoundedDouble WaveAngle { get; }

        /// <summary>
        /// Gets the wave direction with respect to the North.
        /// </summary>
        public RoundedDouble WaveDirection { get; }

        /// <summary>
        /// Gets the wave period.
        /// </summary>
        public RoundedDouble WavePeriod { get; }

        /// <summary>
        /// Gets the wave height.
        /// </summary>
        public RoundedDouble WaveHeight { get; }

        /// <summary>
        /// Gets the water level.
        /// </summary>
        public RoundedDouble WaterLevel { get; }

        /// <summary>
        /// Gets the type of dike cover.
        /// </summary>
        public CoverType CoverType { get; }

        /// <summary>
        /// Gets the target probability.
        /// </summary>
        public string TargetProbability { get; }

        /// <summary>
        /// Gets the id of the foreshore.
        /// </summary>
        public string ForeshoreId { get; }

        /// <summary>
        /// Gets a value indicating whether a foreshore profile was used in the calculation.
        /// </summary>
        public bool UseForeshore { get; }

        /// <summary>
        /// Gets a value indicating whether a break water was used in the calculation. 
        /// </summary>
        public bool UseBreakWater { get; }

        /// <summary>
        /// Gets the y coordinate of the location.
        /// </summary>
        public double LocationYCoordinate { get; }

        /// <summary>
        /// Gets the x coordinate of the location.
        /// </summary>
        public double LocationXCoordinate { get; }

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public string LocationName { get; }

        /// <summary>
        /// Gets the name of the calculation.
        /// </summary>
        public string CalculationName { get; }
    }
}