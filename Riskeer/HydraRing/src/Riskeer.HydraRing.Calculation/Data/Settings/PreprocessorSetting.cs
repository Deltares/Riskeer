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

namespace Riskeer.HydraRing.Calculation.Data.Settings
{
    /// <summary>
    /// Container for preprocessor settings.
    /// </summary>
    public class PreprocessorSetting
    {
        /// <summary>
        /// Creates a new instance of <see cref="PreprocessorSetting"/>.
        /// </summary>
        /// <remarks><see cref="RunPreprocessor"/> is set to <c>false</c>; <see cref="ValueMin"/> and <see cref="ValueMax"/>
        /// are set to <see cref="double.NaN"/>; <see cref="NumericsSetting"/> is set to <c>null</c>.</remarks>
        public PreprocessorSetting()
        {
            ValueMin = double.NaN;
            ValueMax = double.NaN;
        }

        /// <summary>
        /// Creates a new instance of <see cref="PreprocessorSetting"/>.
        /// </summary>
        /// <param name="valueMin">The minimum value to use while running the preprocessor.</param>
        /// <param name="valueMax">The maximum value to use while running the preprocessor.</param>
        /// <param name="numericsSetting">The numerics settings to use while running the preprocessor.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="numericsSetting"/> is <c>null</c>.</exception>
        /// <remarks><see cref="RunPreprocessor"/> is set to <c>true</c>.</remarks>
        public PreprocessorSetting(double valueMin, double valueMax, NumericsSetting numericsSetting)
        {
            if (numericsSetting == null)
            {
                throw new ArgumentNullException(nameof(numericsSetting));
            }

            RunPreprocessor = true;
            ValueMin = valueMin;
            ValueMax = valueMax;
            NumericsSetting = numericsSetting;
        }

        /// <summary>
        /// Gets a value indicating whether the preprocessor should be run.
        /// </summary>
        public bool RunPreprocessor { get; }

        /// <summary>
        /// Gets the minimum value to use while running the preprocessor.
        /// </summary>
        public double ValueMin { get; }

        /// <summary>
        /// Gets the maximum value to use while running the preprocessor.
        /// </summary>
        public double ValueMax { get; }

        /// <summary>
        /// Get the numerics settings to use while running the preprocessor.
        /// </summary>
        public NumericsSetting NumericsSetting { get; }
    }
}