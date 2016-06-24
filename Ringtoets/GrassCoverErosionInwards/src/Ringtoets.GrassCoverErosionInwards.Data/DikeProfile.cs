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
using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Definition for a dike profile for the Grass Cover Erosion Inwards failure mechanism.
    /// </summary>
    public class DikeProfile
    {
        private RoundedDouble orientation;
        private RoundedDouble dikeHeight;
        private readonly List<Point2D> foreshoreGeometry;
        private readonly List<RoughnessPoint> dikeGeometry;

        /// <summary>
        /// Creates a new instance of the <see cref="DikeProfile"/> class.
        /// </summary>
        /// <param name="worldCoordinate">The value for <see cref="WorldReferencePoint"/>.</param>
        public DikeProfile(Point2D worldCoordinate)
        {
            if (worldCoordinate == null)
            {
                throw new ArgumentNullException("worldCoordinate");
            }

            orientation = new RoundedDouble(2);
            dikeHeight = new RoundedDouble(2);

            Name = Resources.DikeProfile_DefaultName;
            Memo = "";
            dikeGeometry = new List<RoughnessPoint>();
            foreshoreGeometry = new List<Point2D>();
            WorldReferencePoint = worldCoordinate;
        }

        /// <summary>
        /// Gets or sets the name of the dike profile.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the optional notes about this instance.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Gets the reference point in world coordinates corresponding to the local coordinate <see cref="X0"/>.
        /// </summary>
        public Point2D WorldReferencePoint { get; private set; }

        /// <summary>
        /// Gets or sets the local x-coordinate corresponding to the world reference point <see cref="WorldReferencePoint"/>.
        /// </summary>
        public double X0 { get; set; }

        /// <summary>
        /// Gets or sets the orientation of the dike profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return orientation;
            }
            set
            {
                orientation = value.ToPrecision(orientation.NumberOfDecimalPlaces);
            }
        }

        /// <summary>
        /// Indicates if there is a break water object available for this instance or not.
        /// </summary>
        public bool HasBreakWater
        {
            get
            {
                return BreakWater != null;
            }
        }

        /// <summary>
        /// Gets or sets the break water object of the dike profile, if any.
        /// </summary>
        public BreakWater BreakWater { get; set; }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public List<Point2D> ForeshoreGeometry {
            get
            {
                return foreshoreGeometry;
            }
        }

        /// <summary>
        /// Gets the geometry of the dike with roughness data.
        /// </summary>
        /// <remarks>
        /// The roughness of a <see cref="RoughnessPoint"/> in the list represents
        /// the roughness of the section between this <see cref="RoughnessPoint"/>
        /// and the succeeding <see cref="RoughnessPoint"/>. The roughness of the last
        /// point is irrelevant.
        /// </remarks>
        public List<RoughnessPoint> DikeGeometry {
            get
            {
                return dikeGeometry;
            }
        }

        /// <summary>
        /// Gets or sets the height of the dike [m+NAP].
        /// </summary>
        public RoundedDouble DikeHeight
        {
            get
            {
                return dikeHeight;
            }
            set
            {
                dikeHeight = value.ToPrecision(dikeHeight.NumberOfDecimalPlaces);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}