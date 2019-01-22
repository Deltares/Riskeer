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

using System;

namespace Ringtoets.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds all hydraulic boundary calculation settings.
    /// </summary>
    public class HydraulicBoundaryCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabaseFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="hlcdFilePath">The file path of the HLCD.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="hydraulicBoundaryDatabaseFilePath"/>
        /// or <paramref name="hlcdFilePath"/> is <c>null</c>, is empty or consists of whitespace.</exception>
        public HydraulicBoundaryCalculationSettings(string hydraulicBoundaryDatabaseFilePath,
                                                    string hlcdFilePath,
                                                    string preprocessorDirectory)
        {
            if (string.IsNullOrWhiteSpace(hydraulicBoundaryDatabaseFilePath))
            {
                throw new ArgumentException($"{nameof(hydraulicBoundaryDatabaseFilePath)} is null, empty or consist of whitespace.");
            }

            if (string.IsNullOrWhiteSpace(hlcdFilePath))
            {
                throw new ArgumentException($"{nameof(hlcdFilePath)} is null, empty or consist of whitespace.");
            }

            HydraulicBoundaryDatabaseFilePath = hydraulicBoundaryDatabaseFilePath;
            HlcdFilePath = hlcdFilePath;
            PreprocessorDirectory = preprocessorDirectory;
        }

        /// <summary>
        /// Gets the hydraulic boundary database filepath.
        /// </summary>
        public string HydraulicBoundaryDatabaseFilePath { get; }

        /// <summary>
        /// Gets the file path of the Hydraulic Boundary Locations Configuration Database (HLCD).
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        ///  Gets the preprocessor directory.
        /// </summary>
        public string PreprocessorDirectory { get; }
    }
}