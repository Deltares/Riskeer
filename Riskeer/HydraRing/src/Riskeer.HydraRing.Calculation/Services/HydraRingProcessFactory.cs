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

using System.Diagnostics;

namespace Riskeer.HydraRing.Calculation.Services
{
    /// <summary>
    /// Factory for creating <see cref="Process"/> instances that can be used for performing Hydra-Ring calculations.
    /// </summary>
    internal static class HydraRingProcessFactory
    {
        /// <summary>
        /// Creates a <see cref="Process"/> that can be used for performing a Hydra-Ring calculation.
        /// </summary>
        /// <param name="mechanismComputationExeFilePath">The path to the MechanismComputation.exe file that should be used for the calculation.</param>
        /// <param name="iniFilePath">The path to the ini file that should be used during the calculation.</param>
        /// <param name="workingDirectory">The working directory that should be used during the calculation.</param>
        /// <returns>The created process.</returns>
        public static Process Create(string mechanismComputationExeFilePath, string iniFilePath, string workingDirectory)
        {
            return new Process
            {
                StartInfo = new ProcessStartInfo(mechanismComputationExeFilePath, iniFilePath)
                {
                    WorkingDirectory = workingDirectory,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };
        }
    }
}