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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.DuneErosion.Data
{
    /// <summary>
    /// Location of a dune.
    /// </summary>
    public class DuneLocation
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The corresponding <see cref="HydraulicBoundaryLocation"/>.</param>
        /// <param name="name">The name of the <see cref="DuneLocation"/>.</param>
        /// <param name="location">The coordinate of the <see cref="DuneLocation"/>.</param>
        /// <param name="properties">The container of the properties for the <see cref="DuneLocation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/>,
        /// <paramref name="name"/> or <paramref name="properties"/> is <c>null</c>.</exception>
        public DuneLocation(HydraulicBoundaryLocation hydraulicBoundaryLocation, string name,
                            Point2D location, ConstructionProperties properties)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            Id = hydraulicBoundaryLocation.Id;
            Name = name;
            Location = location;
            CoastalAreaId = properties.CoastalAreaId;
            Offset = new RoundedDouble(1, properties.Offset);
            Orientation = new RoundedDouble(1, properties.Orientation);
            D50 = new RoundedDouble(6, properties.D50);
        }

        /// <summary>
        /// Gets the database id of the dune location.
        /// </summary>
        public long Id { get; }

        /// <summary>
        /// Gets the name of the dune location.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the coordinate of the dune location.
        /// </summary>
        public Point2D Location { get; }

        /// <summary>
        /// Gets the coastal area id of the dune location.
        /// </summary>
        public int CoastalAreaId { get; }

        /// <summary>
        /// Gets the offset of the dune location.
        /// </summary>
        public RoundedDouble Offset { get; }

        /// <summary>
        /// Gets the orientation of the dune location.
        /// </summary>
        public RoundedDouble Orientation { get; }

        /// <summary>
        /// Gets the d50 of the dune location.
        /// </summary>
        public RoundedDouble D50 { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="DuneLocation"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Sets the coastal area id of the dune location.
            /// </summary>
            public int CoastalAreaId { internal get; set; }

            /// <summary>
            /// Sets the offset of the dune location.
            /// </summary>
            public double Offset { internal get; set; }

            /// <summary>
            /// Sets the orientation of the dune location.
            /// </summary>
            public double Orientation { internal get; set; }

            /// <summary>
            /// Sets the d50 of the dune location.
            /// </summary>
            public double D50 { internal get; set; }
        }
    }
}