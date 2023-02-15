﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Data.Input
{
    /// <summary>
    /// Class which holds all the general information to run a Hydra-Ring calculation.
    /// </summary>
    public class HydraRingCalculationSettings
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraRingCalculationSettings"/>.
        /// </summary>
        /// <param name="hlcdFilePath">The file path of the hydraulic location configuration database.</param>
        /// <param name="preprocessorDirectory">The preprocessor directory to be used for the calculations.</param>
        /// <param name="usePreprocessorClosure">Indicator whether to use the preprocessor closure.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hlcdFilePath"/> or
        /// <paramref name="preprocessorDirectory"/> is <c>null</c>.</exception>
        public HydraRingCalculationSettings(string hlcdFilePath,
                                            string preprocessorDirectory,
                                            bool usePreprocessorClosure)
        {
            if (hlcdFilePath == null)
            {
                throw new ArgumentNullException(nameof(hlcdFilePath));
            }

            if (preprocessorDirectory == null)
            {
                throw new ArgumentNullException(nameof(preprocessorDirectory));
            }

            HlcdFilePath = hlcdFilePath;
            PreprocessorDirectory = preprocessorDirectory;
            UsePreprocessorClosure = usePreprocessorClosure;
        }

        /// <summary>
        /// Gets the file path of the hydraulic location configuration database.
        /// </summary>
        public string HlcdFilePath { get; }

        /// <summary>
        /// Gets the preprocessor director.
        /// </summary>
        public string PreprocessorDirectory { get; }

        /// <summary>
        /// Gets whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; }
    }
}