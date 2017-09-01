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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// A 2D soil layer that has been adapted by using a surface line.
    /// </summary>
    public class MacroStabilityInwardsSoilLayerUnderSurfaceLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="outerRing">The outer ring of the geometry of the soil layer.</param>
        /// <param name="properties">The properties of the soil layer.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayerUnderSurfaceLine(Point2D[] outerRing, MacroStabilityInwardsSoilLayerProperties properties)
            : this(outerRing, Enumerable.Empty<Point2D[]>(), properties) {}

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSoilLayerUnderSurfaceLine"/>.
        /// </summary>
        /// <param name="outerRing">The outer ring of the geometry of the soil layer.</param>
        /// <param name="holes">The holes of the geometry of the soil layer.</param>
        /// <param name="properties">The properties of the soil layer.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSoilLayerUnderSurfaceLine(Point2D[] outerRing, IEnumerable<Point2D[]> holes, MacroStabilityInwardsSoilLayerProperties properties)
        {
            if (outerRing == null)
            {
                throw new ArgumentNullException(nameof(outerRing));
            }
            if (holes == null)
            {
                throw new ArgumentNullException(nameof(holes));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            OuterRing = outerRing;
            Holes = holes;
            Properties = properties;
        }

        /// <summary>
        /// Gets the outer ring of the geometry.
        /// </summary>
        public Point2D[] OuterRing { get; }

        /// <summary>
        /// Gets the holes of the geometry.
        /// </summary>
        public IEnumerable<Point2D[]> Holes { get; }

        /// <summary>
        /// Gets the properties of the soil layer.
        /// </summary>
        public MacroStabilityInwardsSoilLayerProperties Properties { get; }
    }
}