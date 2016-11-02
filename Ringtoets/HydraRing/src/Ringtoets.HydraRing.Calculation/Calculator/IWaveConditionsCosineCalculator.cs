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

using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates values for a wave at a water level.
    /// These are used in different failure mechanisms as input.
    /// </summary>
    public interface IWaveConditionsCosineCalculator
    {
        /// <summary>
        /// Gets the height of the wave.
        /// </summary>
        double WaveHeight { get; }

        /// <summary>
        /// Gets the angle of the wave.
        /// </summary>
        double WaveAngle { get; }

        /// <summary>
        /// Gets the peak period of the wave.
        /// </summary>
        double WavePeakPeriod { get; }

        /// <summary>
        /// Gets the the output directory used during the Hydra-Ring calculation.
        /// </summary>
        string OutputDirectory { get; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        string LastErrorContent { get; }

        /// <summary>
        /// Performs the actual calculation by running the Hydra-Ring executable.
        /// </summary>
        /// <param name="input">The <see cref="WaveConditionsCosineCalculationInput"/> which contains all the necessary input
        /// for the calculation.</param>
        void Calculate(WaveConditionsCosineCalculationInput input);

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        void Cancel();
    }
}