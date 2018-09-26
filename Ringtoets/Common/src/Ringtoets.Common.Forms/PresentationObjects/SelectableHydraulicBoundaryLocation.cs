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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// Class that represents a <see cref="HydraulicBoundaryLocation"/> with respect to a reference point.
    /// </summary>
    public class SelectableHydraulicBoundaryLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="SelectableHydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="referencePoint">A <see cref="Point2D"/> reference point to which the distance of
        /// <paramref name="hydraulicBoundaryLocation"/> needs to be calculated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/>
        /// is <c>null</c>.</exception>
        /// <remarks>The distance between <paramref name="hydraulicBoundaryLocation"/> and its reference point is defined as 
        /// <see cref="double.NaN"/> when <paramref name="referencePoint"/>is <c>null</c>.</remarks>
        public SelectableHydraulicBoundaryLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation, Point2D referencePoint)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            Distance = new RoundedDouble(0, referencePoint?.GetEuclideanDistanceTo(hydraulicBoundaryLocation.Location) ?? double.NaN);
        }

        /// <summary>
        /// Gets the hydraulic boundary location.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the distance between the <see cref="HydraulicBoundaryLocation"/> and the given reference point.
        /// [m] 
        /// <remarks>The value will be <see cref="double.NaN"/> in case there's no reference point specified.</remarks>
        /// </summary>
        public RoundedDouble Distance { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SelectableHydraulicBoundaryLocation) obj);
        }

        public override int GetHashCode()
        {
            return HydraulicBoundaryLocation.GetHashCode();
        }

        public override string ToString()
        {
            if (double.IsNaN(Distance))
            {
                return HydraulicBoundaryLocation.Name;
            }

            return Distance < 1000
                       ? $"{HydraulicBoundaryLocation.Name} ({Distance:f0} m)"
                       : $"{HydraulicBoundaryLocation.Name} ({Distance / 1000:f1} km)";
        }

        private bool Equals(SelectableHydraulicBoundaryLocation other)
        {
            return ReferenceEquals(HydraulicBoundaryLocation, other.HydraulicBoundaryLocation);
        }
    }
}