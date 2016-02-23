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
using System.Linq;

using Core.Common.Base.Geometry;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// This class represents a sub-section of a <see cref="ReferenceLine"/> in which similar
    /// characteristics can be found to allow for 1 calculation to determine a result that
    /// applies to the whole section.
    /// </summary>
    public class FailureMechanismSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSection"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="geometryPoints">The geometry points.</param>
        public FailureMechanismSection(string name, IEnumerable<Point2D> geometryPoints)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (geometryPoints == null)
            {
                throw new ArgumentNullException("geometryPoints");
            }
            var point2Ds = geometryPoints.ToArray();
            if (point2Ds.Any(p => p == null))
            {
                throw new ArgumentException("One or multiple elements are null.", "geometryPoints");
            }

            Name = name;
            Points = point2Ds;
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the 2D points describing the geometry of the section.
        /// </summary>
        public IEnumerable<Point2D> Points { get; private set; }
    }
}