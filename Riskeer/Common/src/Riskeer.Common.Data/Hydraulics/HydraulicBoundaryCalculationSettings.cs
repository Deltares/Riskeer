// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// Class which holds all hydraulic boundary calculation settings.
    /// </summary>
    public class HydraulicBoundaryCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryCalculationSettings"/>.
        /// </summary>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <param name="hrdFilePath">The file path of the hydraulic boundary database.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        /// <exception cref="ArgumentException">Thrown when either <paramref name="hlcdFilePath"/> or
        /// <paramref name="hrdFilePath"/> is <c>null</c>, is empty or consists of whitespaces.</exception>
        public HydraulicBoundaryCalculationSettings(string hlcdFilePath,
                                                    string hrdFilePath,
                                                    bool usePreprocessorClosure)
        {
            if (string.IsNullOrWhiteSpace(hlcdFilePath))
            {
                throw new ArgumentException($"{nameof(hlcdFilePath)} is null, empty or consists of whitespaces.");
            }

            if (string.IsNullOrWhiteSpace(hrdFilePath))
            {
                throw new ArgumentException($"{nameof(hrdFilePath)} is null, empty or consists of whitespaces.");
            }

            HlcdFilePath = hlcdFilePath;
            HrdFilePath = hrdFilePath;
            UsePreprocessorClosure = usePreprocessorClosure;
        }

        /// <summary>
        /// Gets the file path of the hydraulic location configuration database.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        /// Gets the file path of the hydraulic boundary database.
        /// </summary>
        public string HrdFilePath { get; }

        /// <summary>
        /// Gets the indicator whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; }
    }
}