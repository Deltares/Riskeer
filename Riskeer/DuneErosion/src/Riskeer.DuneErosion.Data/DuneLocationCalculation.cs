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
using Core.Common.Base;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.DuneErosion.Data
{
    /// <summary>
    /// Adapter class for a dune location calculation.
    /// </summary>
    public class DuneLocationCalculation : Observable, ICalculatable
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculation"/>.
        /// </summary>
        /// <param name="duneLocation">The dune location the calculation belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="duneLocation"/>
        /// is <c>null</c>.</exception>
        public DuneLocationCalculation(DuneLocation duneLocation)
        {
            if (duneLocation == null)
            {
                throw new ArgumentNullException(nameof(duneLocation));
            }

            DuneLocation = duneLocation;
        }

        /// <summary>
        /// Gets or sets the output of the calculation.
        /// </summary>
        public DuneLocationCalculationOutput Output { get; set; }

        /// <summary>
        /// Gets the dune location the calculation belongs to.
        /// </summary>
        public DuneLocation DuneLocation { get; }

        public bool ShouldCalculate
        {
            get
            {
                return Output == null;
            }
        }
    }
}