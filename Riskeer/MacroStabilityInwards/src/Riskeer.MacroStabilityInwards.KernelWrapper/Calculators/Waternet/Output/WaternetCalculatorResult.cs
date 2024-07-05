﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Output
{
    /// <summary>
    /// This class contains the results of a Waternet calculation.
    /// </summary>
    public class WaternetCalculatorResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="WaternetCalculatorResult"/>.
        /// </summary>
        /// <param name="phreaticLines">The phreatic lines.</param>
        /// <param name="waternetLines">The Waternet lines.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="phreaticLines"/>
        /// or <paramref name="waternetLines"/> is <c>null</c>.</exception>
        internal WaternetCalculatorResult(IEnumerable<WaternetPhreaticLineResult> phreaticLines,
                                          IEnumerable<WaternetLineResult> waternetLines)
        {
            if (phreaticLines == null)
            {
                throw new ArgumentNullException(nameof(phreaticLines));
            }

            if (waternetLines == null)
            {
                throw new ArgumentNullException(nameof(waternetLines));
            }

            PhreaticLines = phreaticLines;
            WaternetLines = waternetLines;
        }

        /// <summary>
        /// Gets the phreatic lines.
        /// </summary>
        public IEnumerable<WaternetPhreaticLineResult> PhreaticLines { get; }

        /// <summary>
        /// Gets the Waternet lines.
        /// </summary>
        public IEnumerable<WaternetLineResult> WaternetLines { get; }
    }
}