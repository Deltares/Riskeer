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

namespace Ringtoets.Common.IO.SurfaceLines
{
    /// <summary>
    /// This class represents a collection of characterizing locations on a surface line.
    /// </summary>
    public class CharacteristicPoints
    {
        /// <summary>
        /// Creates an instance of <see cref="CharacteristicPoints"/>.
        /// </summary>
        /// <param name="name">The name of the location for which the <see cref="CharacteristicPoints"/>
        /// defines characteristic points.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public CharacteristicPoints(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name), @"Cannot make a definition of characteristic points for an unknown location.");
            }

            Name = name;
        }

        /// <summary>
        /// Gets the name of the location for which the <see cref="CharacteristicPoints"/> defines
        /// characteristic points.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets or sets the location which generalizes the surface level on the
        /// inside of the polder
        /// </summary>
        public Point3D SurfaceLevelInside { get; set; }

        /// <summary>
        /// Gets or sets the location of the start of the ditch when approaching from
        /// inside the polder.
        /// </summary>
        public Point3D DitchPolderSide { get; set; }

        /// <summary>
        /// Gets or sets the location of the bottom of the ditch when approaching 
        /// from inside the polder.
        /// </summary>
        public Point3D BottomDitchPolderSide { get; set; }

        /// <summary>
        /// Gets or sets the location of the bottom of the ditch when approaching
        /// from the dike.
        /// </summary>
        public Point3D BottomDitchDikeSide { get; set; }

        /// <summary>
        /// Gets or sets the location of the start of the ditch when approaching
        /// from the dike.
        /// </summary>
        public Point3D DitchDikeSide { get; set; }

        /// <summary>
        /// Gets or sets the location of dike toe when approaching from inside
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtPolder { get; set; }

        /// <summary>
        /// Gets or sets the location where the shoulder on the side of the polder
        /// declines towards the location of the dike toe when approaching from inside 
        /// the polder.
        /// </summary>
        public Point3D ShoulderTopInside { get; set; }

        /// <summary>
        /// Gets or sets the location where the shoulder on the side of the polder
        /// connects with the dike.
        /// </summary>
        public Point3D ShoulderBaseInside { get; set; }

        /// <summary>
        /// Gets or sets the location of the top of the dike when approaching from 
        /// inside the polder.
        /// </summary>
        public Point3D DikeTopAtPolder { get; set; }

        /// <summary>
        /// Gets or sets the location of the start of traffic load when approaching 
        /// from inside the polder.
        /// </summary>
        public Point3D TrafficLoadInside { get; set; }

        /// <summary>
        /// Gets or sets the location of the start of traffic load when approaching 
        /// from outside the polder.
        /// </summary>
        public Point3D TrafficLoadOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of the top of the dike when approaching 
        /// from outside the polder.
        /// </summary>
        public Point3D DikeTopAtRiver { get; set; }

        /// <summary>
        /// Gets or sets the location where the shoulder on the outside of the polder
        /// connects with the dike.
        /// </summary>
        public Point3D ShoulderBaseOutside { get; set; }

        /// <summary>
        /// Gets or sets the location where the shoulder on the outside of the polder
        /// declines towards the location of the dike toe when approaching from outside 
        /// the polder.
        /// </summary>
        public Point3D ShoulderTopOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike toe when approaching from outside 
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtRiver { get; set; }

        /// <summary>
        /// Gets or sets the location which generalizes the height of the surface
        /// on the outside of the polder.
        /// </summary>
        public Point3D SurfaceLevelOutside { get; set; }

        /// <summary>
        /// Gets or sets the location of dike table height.
        /// </summary>
        public Point3D DikeTableHeight { get; set; }

        /// <summary>
        /// Gets or sets the location of insert river channel.
        /// </summary>
        public Point3D InsertRiverChannel { get; set; }

        /// <summary>
        /// Gets or sets the location of bottom river channel.
        /// </summary>
        public Point3D BottomRiverChannel { get; set; }
    }
}