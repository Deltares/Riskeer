// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class holds information to be able to present calculations coupled to
    /// hydraulic boundary locations on the map.
    /// </summary>
    public class MapCalculationData
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapCalculationData"/>.
        /// </summary>
        /// <param name="calculationName">The name of the calculation.</param>
        /// <param name="calculationLocation">The location of the calculation.</param>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location 
        /// assigned to the calculation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters is <c>null</c>.</exception>
        public MapCalculationData(string calculationName, Point2D calculationLocation, HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (calculationName == null)
            {
                throw new ArgumentNullException(nameof(calculationName), @"A calculation name is required.");
            }

            if (calculationLocation == null)
            {
                throw new ArgumentNullException(nameof(calculationLocation), @"A location for the calculation is required.");
            }

            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation), @"A hydraulic boundary location is required.");
            }

            Name = calculationName;
            CalculationLocation = calculationLocation;
            HydraulicBoundaryLocation = hydraulicBoundaryLocation;
        }

        /// <summary>
        /// Gets the name of the calculation.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the location of the calculation.
        /// </summary>
        public Point2D CalculationLocation { get; }

        /// <summary>
        /// Gets the hydraulic boundary location assigned to the calculation.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; }
    }
}