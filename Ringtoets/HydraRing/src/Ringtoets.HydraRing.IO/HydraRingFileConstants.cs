﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.IO
{
    /// <summary>
    /// Class containing (parts of) file names that are generated during a Hydra-Ring calculation.
    /// </summary>
    internal static class HydraRingFileConstants
    {
        /// <summary>
        /// The tail and extension of the file containing output generated during a calculation.
        /// </summary>
        internal const string OutputFileSuffix = "-output.txt";

        /// <summary>
        /// The extension of the log file for a certain section.
        /// </summary>
        internal const string LogFileExtension = ".log";

        /// <summary>
        /// The file name of the file containing the output of a calculation.
        /// </summary>
        internal const string DesignTablesFileName = "designTable.txt";

        /// <summary>
        /// The file name of the working database which contains input and output.
        /// </summary>
        internal const string OutputDatabaseFileName = "temp.sqlite";

        /// <summary>
        /// The file name of the HLCD database.
        /// </summary>
        internal const string HlcdDatabaseFileName = "HLCD.sqlite";

        /// <summary>
        /// The file name of the executable of Hydra-Ring.
        /// </summary>
        internal const string HydraRingExecutableFileName = "MechanismComputation.exe";

        /// <summary>
        /// The database which contains configuration paramters.
        /// </summary>
        internal const string ConfigurationDatabaseFileName = "config.sqlite";
    }
}