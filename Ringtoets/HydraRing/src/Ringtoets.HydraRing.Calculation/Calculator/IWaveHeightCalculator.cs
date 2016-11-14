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

using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates a wave height associated with a
    /// reliability index.
    /// </summary>
    public interface IWaveHeightCalculator
    {
        /// <summary>
        /// Gets the wave height resulting from iterating towards the reliability index.
        /// </summary>
        double WaveHeight { get; }

        /// <summary>
        /// The reliability index towards which was iterated.
        /// </summary>
        double ReliabilityIndex { get; }

        /// <summary>
        /// Gets the the output directory used during the Hydra-Ring calculation.
        /// </summary>
        string OutputDirectory { get; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        string LastErrorFileContent { get; }

        /// <summary>
        /// Performs the actual calculation by running the Hydra-Ring executable.
        /// </summary>
        /// <param name="input">The <see cref="WaveHeightCalculationInput"/> which contains all the necessary input
        /// for the calculation.</param>
        void Calculate(WaveHeightCalculationInput input);

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        void Cancel();
    }
}