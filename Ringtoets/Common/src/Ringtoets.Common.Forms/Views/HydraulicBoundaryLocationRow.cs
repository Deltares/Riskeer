﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocation"/>.
    /// </summary>
    public abstract class HydraulicBoundaryLocationRow
    {
        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The <see cref="HydraulicBoundaryLocation"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocation");
            }

            HydraulicBoundaryLocation = hydraulicBoundaryLocation;
        }

        /// <summary>
        /// Gets the hydraulic boundaries location.
        /// </summary>
        public HydraulicBoundaryLocation HydraulicBoundaryLocation { get; private set; }

        /// <summary>
        /// Gets or sets whether the <see cref="HydraulicBoundaryLocationRow"/> is set to be calculated.
        /// </summary>
        public bool ToCalculate { get; set; }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return HydraulicBoundaryLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return HydraulicBoundaryLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.HydraRing.Data.HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return HydraulicBoundaryLocation.Location;
            }
        }
    }
}