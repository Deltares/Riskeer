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
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Hydraulics
{
    /// <summary>
    /// The data that represents a linked hydraulic boundary database file.
    /// </summary>
    public class HrdFile
    {
        private bool canUsePreprocessor;
        private bool usePreprocessor;
        private string preprocessorDirectory;

        /// <summary>
        /// Creates a new instance of <see cref="HrdFile"/>.
        /// </summary>
        /// <remarks><see cref="CanUsePreprocessor"/> is set to <c>false</c>.</remarks>
        public HrdFile()
        {
            CanUsePreprocessor = false;
        }

        /// <summary>
        /// Gets or sets the path to the hydraulic boundary database file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets the version of the hydraulic boundary database.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the indicator whether to use the preprocessor closure.
        /// </summary>
        public bool UsePreprocessorClosure { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the Hydra-Ring preprocessor can be used.
        /// </summary>
        /// <remarks>When setting this property to <c>false</c>, both <see cref="UsePreprocessor"/>
        /// and <see cref="PreprocessorDirectory"/> are reset.</remarks>
        public bool CanUsePreprocessor
        {
            get
            {
                return canUsePreprocessor;
            }
            set
            {
                canUsePreprocessor = value;

                if (!canUsePreprocessor)
                {
                    usePreprocessor = false;
                    preprocessorDirectory = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the Hydra-Ring preprocessor must be used.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when set while <see cref="CanUsePreprocessor"/> is <c>false</c>.</exception>
        public bool UsePreprocessor
        {
            get
            {
                return usePreprocessor;
            }
            set
            {
                if (!CanUsePreprocessor)
                {
                    throw new InvalidOperationException($"{nameof(CanUsePreprocessor)} is false.");
                }

                usePreprocessor = value;
            }
        }

        /// <summary>
        /// Gets or sets the Hydra-Ring preprocessor directory.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when set while <see cref="CanUsePreprocessor"/> is <c>false</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when setting a value that matches <see cref="string.IsNullOrWhiteSpace"/>.</exception>
        public string PreprocessorDirectory
        {
            get
            {
                return preprocessorDirectory;
            }
            set
            {
                if (!CanUsePreprocessor)
                {
                    throw new InvalidOperationException($"{nameof(CanUsePreprocessor)} is false.");
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException(Resources.HydraulicBoundaryDatabase_PreprocessorDirectory_Path_must_have_a_value);
                }

                preprocessorDirectory = value;
            }
        }
    }
}