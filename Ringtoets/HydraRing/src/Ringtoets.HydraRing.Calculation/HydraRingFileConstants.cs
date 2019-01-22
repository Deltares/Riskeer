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

namespace Ringtoets.HydraRing.Calculation
{
    /// <summary>
    /// Class containing (parts of) file names that are generated during a Hydra-Ring calculation.
    /// </summary>
    internal static class HydraRingFileConstants
    {
        /// <summary>
        /// The file name of the file containing the output of a calculation.
        /// </summary>
        internal const string DesignTablesFileName = "designTable.txt";

        /// <summary>
        /// The tail and extension of the output database which contains output for a calculation.
        /// </summary>
        internal const string OutputDatabaseFileNameSuffix = "-output.sqlite";

        /// <summary>
        /// The file name of the executable of Hydra-Ring.
        /// </summary>
        internal const string HydraRingExecutableFileName = "MechanismComputation.exe";

        /// <summary>
        /// The database which contains configuration parameters.
        /// </summary>
        internal const string ConfigurationDatabaseFileName = "config.sqlite";

        /// <summary>
        /// The file name which contains the error of a calculation.
        /// </summary>
        internal const string LastErrorFileName = "last_error.txt";
    }
}