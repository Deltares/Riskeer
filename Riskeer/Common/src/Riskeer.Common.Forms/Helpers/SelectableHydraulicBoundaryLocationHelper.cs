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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.PresentationObjects;

namespace Riskeer.Common.Forms.Helpers
{
    /// <summary>
    /// Class holds methods to help when dealing with <see cref="SelectableHydraulicBoundaryLocation"/>.
    /// </summary>
    public static class SelectableHydraulicBoundaryLocationHelper
    {
        /// <summary>
        /// Gets the sorted selectable hydraulic boundary locations from <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The available hydraulic boundary locations.</param>
        /// <param name="referencePoint">The reference point to which the distance needs to be 
        /// calculated in <see cref="SelectableHydraulicBoundaryLocation"/>.</param>
        /// <returns>An ordered collection of selectable hydraulic boundary locations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public static IEnumerable<SelectableHydraulicBoundaryLocation> GetSortedSelectableHydraulicBoundaryLocations(
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations, Point2D referencePoint)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            return hydraulicBoundaryLocations.Select(hbl => new SelectableHydraulicBoundaryLocation(hbl, referencePoint))
                                             .OrderBy(hbl => hbl.Distance)
                                             .ThenBy(hbl => hbl.HydraulicBoundaryLocation.Id)
                                             .ToArray();
        }
    }
}