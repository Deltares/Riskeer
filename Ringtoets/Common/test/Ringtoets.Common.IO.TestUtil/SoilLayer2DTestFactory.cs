﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.TestUtil
{
    /// <summary>
    /// Factory to create simple <see cref="SoilLayer2D"/> instances that can be used for testing.
    /// </summary>
    public static class SoilLayer2DTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <returns>The created <see cref="SoilLayer2D"/>.</returns>
        public static SoilLayer2D CreateSoilLayer2D()
        {
            var pointA = new Point2D(0.0, 0.0);
            var pointB = new Point2D(1.0, 0.0);
            var pointC = new Point2D(1.0, 1.0);
            var pointD = new Point2D(2.0, 1.0);

            var innerLoops = new[]
            {
                new[]
                {
                    new Segment2D(pointC, pointD),
                    new Segment2D(pointD, pointC)
                }
            };
            var outerLoop = new List<Segment2D>
            {
                new Segment2D(pointA, pointB),
                new Segment2D(pointB, pointA)
            };

            return CreateSoilLayer2D(innerLoops, outerLoop);
        }

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <param name="innerLoops">The inner loops of the <see cref="SoilLayer2D"/>,
        /// for which each of the segments are connected to the next.</param>
        /// <param name="outerLoop">The outer loop of the <see cref="SoilLayer2D"/>,
        /// for which each of the segments are connected to the next.</param>
        /// <returns>The created <see cref="SoilLayer2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="Segment2D"/> in <paramref name="innerLoops"/> 
        /// or <paramref name="outerLoop"/> do not form a loop.</exception>
        public static SoilLayer2D CreateSoilLayer2D(IEnumerable<IEnumerable<Segment2D>> innerLoops, IEnumerable<Segment2D> outerLoop)
        {
            if (innerLoops == null)
            {
                throw new ArgumentNullException(nameof(innerLoops));
            }
            if (outerLoop == null)
            {
                throw new ArgumentNullException(nameof(outerLoop));
            }

            var soilLayer2D = new SoilLayer2D
            {
                OuterLoop = outerLoop
            };

            foreach (IEnumerable<Segment2D> innerLoop in innerLoops)
            {
                soilLayer2D.AddInnerLoop(innerLoop);
            }

            return soilLayer2D;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/> 
        /// which can be used for testing transformer implementations.
        /// </summary>
        /// <returns>The created <see cref="SoilLayer2D"/>.</returns>
        public static SoilLayer2D CreateSoilLayer2DForTransforming()
        {
            SoilLayer2D layer = CreateSoilLayer2D();
            layer.IsAquifer = 0.0;

            return layer;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/> which can be 
        /// used for testing transformer implementations.
        /// </summary>
        /// <param name="innerLoops">The inner loops of the <see cref="SoilLayer2D"/>,
        /// for which each of the segments are connected to the next.</param>
        /// <param name="outerLoop">The outer loop of the <see cref="SoilLayer2D"/>,
        /// for which each of the segments are connected to the next.</param>
        /// <returns>The created <see cref="SoilLayer2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when the <see cref="Segment2D"/> in <paramref name="innerLoops"/> 
        /// or <paramref name="outerLoop"/> do not form a loop.</exception>
        public static SoilLayer2D CreateSoilLayer2DForTransforming(IEnumerable<IEnumerable<Segment2D>> innerLoops,
                                                                 IEnumerable<Segment2D> outerLoop)
        {
            SoilLayer2D layer = CreateSoilLayer2D(innerLoops, outerLoop);
            layer.IsAquifer = 0.0;

            return layer;
        }
    }
}