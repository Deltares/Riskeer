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
using System.Collections.Generic;
using Core.Common.Base.Geometry;

namespace Riskeer.Integration.IO.Assembly
{
    /// <summary>
    /// Class which holds all the information to export a failure mechanism section.
    /// </summary>
    public class ExportableFailureMechanismSection
    {
        /// <summary>
        /// Creates a new instance of <see cref="ExportableFailureMechanismSection"/>.
        /// </summary>
        /// <param name="geometry">The geometry of the failure mechanism section.</param>
        /// <param name="startDistance">The start distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <param name="endDistance">The end distance of the failure mechanism section between the section
        /// and the start of the reference line in meters.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/> is <c>null</c>.</exception>
        public ExportableFailureMechanismSection(IEnumerable<Point2D> geometry, double startDistance, double endDistance)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Geometry = geometry;
            StartDistance = startDistance;
            EndDistance = endDistance;
        }

        /// <summary>
        /// Gets the geometry of this failure mechanism section.
        /// </summary>
        public IEnumerable<Point2D> Geometry { get; }

        /// <summary>
        /// Gets the start distance between this failure mechanism section and the start of the reference line.
        /// [m]
        /// </summary>
        public double StartDistance { get; }

        /// <summary>
        /// Gets the end distance between this failure mechanism section and the reference line.
        /// [m]
        /// </summary>
        public double EndDistance { get; }
    }
}