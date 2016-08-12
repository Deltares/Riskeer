// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    internal class HydraulicBoundaryLocationDesignWaterLevelRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationDesignWaterLevelRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        internal HydraulicBoundaryLocationDesignWaterLevelRow(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }
            Name = hydraulicBoundaryLocation.Name;
            Id = hydraulicBoundaryLocation.Id;
            Location = hydraulicBoundaryLocation.Location;
            DesignWaterLevel = new RoundedDouble(2, hydraulicBoundaryLocation.DesignWaterLevel);
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id { get; private set; }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location { get; private set; }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.DesignWaterLevel"/>.
        /// </summary>
        public RoundedDouble DesignWaterLevel { get; private set; }
    }
}