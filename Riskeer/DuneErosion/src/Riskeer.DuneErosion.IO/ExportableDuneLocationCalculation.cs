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
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.IO
{
    /// <summary>
    /// Class that holds all required data to export a <see cref="DuneLocationCalculation"/>.
    /// </summary>
    public class ExportableDuneLocationCalculation
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableDuneLocationCalculation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="DuneLocationCalculation"/> that will be exported.</param>
        /// <param name="norm">The norm belonging to the calculation.</param>
        /// <param name="categoryBoundaryName">The display name of the category boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculation"/>
        /// or <paramref name="categoryBoundaryName"/> is <c>null</c>.</exception>
        public ExportableDuneLocationCalculation(DuneLocationCalculation calculation,
                                                 double norm,
                                                 string categoryBoundaryName)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (categoryBoundaryName == null)
            {
                throw new ArgumentNullException(nameof(categoryBoundaryName));
            }

            Calculation = calculation;
            Norm = norm;
            CategoryBoundaryName = categoryBoundaryName;
        }

        /// <summary>
        /// Gets the <see cref="DuneLocationCalculation"/> that will be exported.
        /// </summary>
        public DuneLocationCalculation Calculation { get; }

        /// <summary>
        /// Gets the norm belonging to the calculation.
        /// </summary>
        public double Norm { get; }

        /// <summary>
        /// Gets the display name of the category boundary.
        /// </summary>
        public string CategoryBoundaryName { get; }
    }
}