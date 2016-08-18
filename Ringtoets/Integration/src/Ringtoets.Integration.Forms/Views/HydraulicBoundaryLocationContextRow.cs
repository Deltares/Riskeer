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
using Core.Common.Base.Geometry;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="HydraulicBoundaryLocationContext"/>.
    /// </summary>
    internal abstract class HydraulicBoundaryLocationContextRow
    {
        private readonly HydraulicBoundaryLocationContext hydraulicBoundaryLocationContext;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationContextRow"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocationContext">The <see cref="HydraulicBoundaryLocationContext"/> for this row.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocationContext"/> is <c>null</c>.</exception>
        internal HydraulicBoundaryLocationContextRow(HydraulicBoundaryLocationContext hydraulicBoundaryLocationContext)
        {
            if (hydraulicBoundaryLocationContext == null)
            {
                throw new ArgumentNullException("hydraulicBoundaryLocationContext");
            }

            this.hydraulicBoundaryLocationContext = hydraulicBoundaryLocationContext;
        }

        /// <summary>
        /// Gets or sets whether the <see cref="HydraulicBoundaryLocationContextRow"/> is set to be calculated.
        /// </summary>
        public bool ToCalculate { get; set; }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Name"/>.
        /// </summary>
        public string Name
        {
            get
            {
                return hydraulicBoundaryLocationContext.HydraulicBoundaryLocation.Name;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Id"/>.
        /// </summary>
        public long Id
        {
            get
            {
                return hydraulicBoundaryLocationContext.HydraulicBoundaryLocation.Id;
            }
        }

        /// <summary>
        /// Gets the <see cref="HydraulicBoundaryLocation.Location"/>.
        /// </summary>
        public Point2D Location
        {
            get
            {
                return hydraulicBoundaryLocationContext.HydraulicBoundaryLocation.Location;
            }
        }

        /// <summary>
        /// Gets the <see cref="Ringtoets.Integration.Forms.PresentationObjects.HydraulicBoundaryLocationContext"/>.
        /// </summary>
        public HydraulicBoundaryLocationContext HydraulicBoundaryLocationContext
        {
            get
            {
                return hydraulicBoundaryLocationContext;
            }
        }
    }
}