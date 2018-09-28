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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Util
{
    /// <summary>
    /// This class contains a calculation and the location for that calculation as a 2D point.
    /// </summary>
    public class CalculationWithLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="CalculationWithLocation"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="ICalculation"/> which has an
        /// input element which represents a location.</param>
        /// <param name="location">The <see cref="Point2D"/> which represents
        /// the location of the <paramref name="calculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either
        /// calculation or location is <c>null</c>.</exception>
        public CalculationWithLocation(ICalculation calculation, Point2D location)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            Calculation = calculation;
            Location = location;
        }

        /// <summary>
        /// Gets the calculation which has a location assigned.
        /// </summary>
        public ICalculation Calculation { get; }

        /// <summary>
        /// Gets the location of the calculation.
        /// </summary>
        public Point2D Location { get; }
    }
}