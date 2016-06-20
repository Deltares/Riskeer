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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// This class represents a geometry point with a roughness.
    /// </summary>
    public class RoughnessPoint
    {
        /// <summary>
        /// Creates a new instance of the <see cref="RoughnessPoint"/> class.
        /// </summary>
        /// <param name="point">The geometry point.</param>
        /// <param name="roughness">The roughness.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="roughness"/> is not in range [0.5, 1.0].</exception>
        public RoughnessPoint(Point2D point, double roughness)
        {
            if (point == null)
            {
                throw new ArgumentNullException("point");
            }

            var roundedRoughness = new RoundedDouble(2, roughness);
            if (roundedRoughness < 0.5 || roundedRoughness > 1.0)
            {
                string message = string.Format(Resources.RoughnessPoint_Roughness_Value_0_should_be_in_interval, roundedRoughness);
                throw new ArgumentOutOfRangeException("roughness", message);
            }

            Point = point;
            Roughness = roundedRoughness;
        }

        /// <summary>
        /// Gets the geometry point of the <see cref="RoughnessPoint"/>.
        /// </summary>
        public Point2D Point { get; private set; }

        /// <summary>
        /// Gets the roughness of the <see cref="RoughnessPoint"/>.
        /// </summary>
        public RoundedDouble Roughness { get; private set; }
    }
}
