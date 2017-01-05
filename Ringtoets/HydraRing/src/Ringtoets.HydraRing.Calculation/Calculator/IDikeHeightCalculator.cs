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
using System.ComponentModel;
using System.IO;
using System.Security;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Exceptions;

namespace Ringtoets.HydraRing.Calculation.Calculator
{
    /// <summary>
    /// Interface for a calculator which calculates a dike height associated to the result of iterating towards a
    /// probability of failure given a norm.
    /// </summary>
    public interface IDikeHeightCalculator
    {
        /// <summary>
        /// Gets the calculated dike height.
        /// </summary>
        double DikeHeight { get; }

        /// <summary>
        /// Gets the reliability index.
        /// </summary>
        double ReliabilityIndex { get; }

        /// <summary>
        /// Gets the temporary output directory that is generated during the Hydra-Ring calculation.
        /// </summary>
        string OutputDirectory { get; }

        /// <summary>
        /// Gets the content of the last error file generated during the Hydra-Ring calculation.
        /// </summary>
        string LastErrorFileContent { get; }

        /// <summary>
        /// Performs the actual calculation by running the Hydra-Ring executable.
        /// </summary>
        /// <param name="input">The <see cref="DikeHeightCalculationInput"/> which contains all the necessary input
        /// for the calculation.</param>
        /// <exception cref="SecurityException">Thrown when the temporary path can't be accessed due to missing permissions.</exception>
        /// <exception cref="IOException">Thrown when the specified path is not valid or the network name is not known 
        /// or an I/O error occurred while opening the file</exception>
        /// <exception cref="UnauthorizedAccessException">Thrown when the directory can't be created due to missing
        /// the required persmissions.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="input"/> is not unique.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="HydraRingCalculationInput.FailureMechanismType"/>
        /// is not the same with already added input.</exception>
        /// <exception cref="Win32Exception">Thrown when there was an error in opening the associated file
        /// or the wait setting could not be accessed.</exception>
        /// <exception cref="ObjectDisposedException">Thrown when the process object has already been disposed.</exception>
        /// <exception cref="HydraRingFileParserException">Thrown when the HydraRing file parser 
        /// encounters an error while parsing HydraRing output.</exception>
        void Calculate(DikeHeightCalculationInput input);

        /// <summary>
        /// Cancels any currently running Hydra-Ring calculation.
        /// </summary>
        void Cancel();
    }
}