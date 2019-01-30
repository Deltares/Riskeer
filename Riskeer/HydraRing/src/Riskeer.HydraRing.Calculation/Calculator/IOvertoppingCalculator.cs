// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.HydraRing.Calculation.Data.Input.Overtopping;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates probability of failure by overtopping or overflow
    /// and the associated wave height.
    /// </summary>
    public interface IOvertoppingCalculator
    {
        /// <summary>
        /// Gets the probability of failure.
        /// </summary>
        double ExceedanceProbabilityBeta { get; }

        /// <summary>
        /// Gets an indication of the height of the wave.
        /// </summary>
        double WaveHeight { get; }

        /// <summary>
        /// Gets a value representing whether overtopping is dominant over overflow.
        /// </summary>
        bool IsOvertoppingDominant { get; }

        /// <summary>
        /// Gets the temporary output directory that is generated during the Hydra-Ring calculation.
        /// </summary>
        string OutputDirectory { get; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        string LastErrorFileContent { get; }

        /// <summary>
        /// Gets the result of parsing the illustration points in the Hydra-Ring database.
        /// </summary>
        GeneralResult IllustrationPointsResult { get; }

        /// <summary>
        /// Gets the error message when parsing the illustration points.
        /// </summary>
        string IllustrationPointsParserErrorMessage { get; }

        /// <summary>
        /// Performs the actual calculation by running the Hydra-Ring executable.
        /// </summary>
        /// <param name="input">The <see cref="OvertoppingCalculationInput"/> which contains all the necessary input
        /// for the calculation.</param>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        /// <exception cref="InvalidOperationException">Thrown when preprocessor directory is required but not specified.</exception>
        void Calculate(OvertoppingCalculationInput input);

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        void Cancel();
    }
}