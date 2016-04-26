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

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class represents a sub-section of a line and the characteristic properties of that line.
    /// </summary>
    public class RoughnessProfileSection
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RoughnessProfileSection"/> class.
        /// </summary>
        /// <param name="startingPoint">Starting point of the section.</param>
        /// <param name="endingPoint">Ending point of the section.</param>
        /// <param name="roughness">The roughness of the section between <paramref name="startingPoint"/> and <paramref name="endingPoint"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when:<list type="bullet">
        /// <item><paramref name="startingPoint"/> is <c>null</c>.</item>
        /// <item><paramref name="endingPoint"/> is <c>null</c>.</item>
        /// </list></exception>
        public RoughnessProfileSection(Point2D startingPoint, Point2D endingPoint, double roughness)
        {
            if (startingPoint == null)
            {
                throw new ArgumentNullException("startingPoint");
            }
            if (endingPoint == null)
            {
                throw new ArgumentNullException("endingPoint");
            }
            StartingPoint = startingPoint;
            EndingPoint = endingPoint;
            Roughness = roughness;
        }

        /// <summary>
        /// Gets the starting 2D geometry point of the <see cref="RoughnessProfileSection"/>.
        /// </summary>
        public Point2D StartingPoint { get; private set; }

        /// <summary>
        /// Gets the ending 2D geometry point of the <see cref="RoughnessProfileSection"/>.
        /// </summary>
        public Point2D EndingPoint { get; private set; }

        /// <summary>
        /// Gets the roughness of the <see cref="RoughnessProfileSection"/>.
        /// </summary>
        public double Roughness { get; private set; }
    }
}