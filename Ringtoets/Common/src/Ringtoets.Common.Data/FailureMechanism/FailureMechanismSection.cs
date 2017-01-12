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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.FailureMechanism
{
    /// <summary>
    /// This class represents a sub-section of a <see cref="ReferenceLine"/> in which similar
    /// characteristics can be found to allow for 1 calculation to determine a result that
    /// applies to the whole section.
    /// </summary>
    public class FailureMechanismSection
    {
        private readonly Point2D geometryStart;
        private readonly Point2D geometryEnd;

        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismSection"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="geometryPoints">The geometry points.</param>
        /// <exception cref="ArgumentNullException">Thrown when:<list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c>.</item>
        /// <item><paramref name="geometryPoints"/> is <c>null</c>.</item> </list></exception>
        /// <exception cref="ArgumentException">Thrown when:<list type="bullet">
        /// <item>One ore more <paramref name="geometryPoints"/> elements are <c>null</c>.</item>
        /// <item><paramref name="geometryPoints"/> does not have at lease one geometry point.</item>
        /// </list></exception>
        public FailureMechanismSection(string name, IEnumerable<Point2D> geometryPoints)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }
            if (geometryPoints == null)
            {
                throw new ArgumentNullException(nameof(geometryPoints));
            }
            var point2Ds = geometryPoints.ToArray();
            if (point2Ds.Any(p => p == null))
            {
                throw new ArgumentException(@"One or multiple elements are null.", nameof(geometryPoints));
            }
            if (point2Ds.Length == 0)
            {
                throw new ArgumentException(Resources.FailureMechanismSection_Section_must_have_at_least_1_geometry_point, nameof(geometryPoints));
            }

            Name = name;
            Points = point2Ds;
            geometryStart = point2Ds[0];
            geometryEnd = point2Ds[point2Ds.Length - 1];
        }

        /// <summary>
        /// Gets the name of the section.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the 2D points describing the geometry of the section.
        /// </summary>
        public IEnumerable<Point2D> Points { get; private set; }

        /// <summary>
        /// Gets the geometric start of the section.
        /// </summary>
        public Point2D GetStart()
        {
            return geometryStart;
        }

        /// <summary>
        /// Gets the geometric end of the section.
        /// </summary>
        public Point2D GetLast()
        {
            return geometryEnd;
        }

        /// <summary>
        /// Gets the length of the section.
        /// </summary>
        public double GetSectionLength()
        {
            return Math2D.ConvertLinePointsToLineSegments(Points).Sum(segment => segment.Length);
        }
    }
}