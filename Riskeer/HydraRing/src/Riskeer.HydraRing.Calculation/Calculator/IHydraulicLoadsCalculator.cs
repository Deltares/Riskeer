// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;
using Riskeer.HydraRing.Calculation.Exceptions;

namespace Riskeer.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates a hydraulic loads value associated to the result of iterating towards a target probability.
    /// </summary>
    public interface IHydraulicLoadsCalculator
    {
        /// <summary>
        /// Gets the calculated hydraulic loads value.
        /// </summary>
        double Value { get; }

        /// <summary>
        /// Gets the reliability index.
        /// </summary>
        double ReliabilityIndex { get; }

        /// <summary>
        /// Gets the value indicating whether the calculation converged.
        /// </summary>
        bool? Converged { get; }

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
        /// <param name="input">The <see cref="HydraulicLoadsCalculationInput"/> which contains all the necessary input
        /// for the calculation.</param>
        /// <exception cref="HydraRingCalculationException">Thrown when an error occurs while performing the calculation.</exception>
        void Calculate(HydraulicLoadsCalculationInput input);

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        void Cancel();
    }
}