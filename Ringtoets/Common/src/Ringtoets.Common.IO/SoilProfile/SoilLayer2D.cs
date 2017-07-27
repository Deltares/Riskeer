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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.IO.Properties;

namespace Ringtoets.Common.IO.SoilProfile
{
    /// <summary>
    /// This class represents objects which were imported from a D-Soil Model database. 
    /// Instances of this class are transient and are not to be used once the D-Soil Model
    /// database has been imported.
    /// </summary>
    public class SoilLayer2D : SoilLayerBase
    {
        private readonly Collection<Segment2D[]> innerLoops;
        private Segment2D[] outerLoop;

        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/>.
        /// </summary>
        public SoilLayer2D()
        {
            innerLoops = new Collection<Segment2D[]>();
        }

        /// <summary>
        /// Gets the outer loop of the <see cref="SoilLayer2D"/> as a <see cref="List{T}"/> of <see cref="Segment2D"/>,
        /// for which each of the segments are connected to the next.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the <see cref="Segment2D"/> in <paramref name="value"/>
        /// do not form a loop.</exception>
        public IEnumerable<Segment2D> OuterLoop
        {
            get
            {
                return outerLoop;
            }
            internal set
            {
                Segment2D[] loop = value.ToArray();
                CheckValidLoop(loop);
                outerLoop = loop;
            }
        }

        /// <summary>
        /// Gets the <see cref="Collection{T}"/> of inner loops (as <see cref="List{T}"/> of <see cref="Segment2D"/>,
        /// for which each of the segments are connected to the next) of the <see cref="SoilLayer2D"/>.
        /// </summary>
        public IEnumerable<Segment2D[]> InnerLoops
        {
            get
            {
                return innerLoops;
            }
        }

        /// <summary>
        /// Adds an inner loop to the <see cref="SoilLayer2D"/> geometry.
        /// </summary>
        /// <param name="innerLoop">The inner loop to add.</param>
        /// <exception cref="ArgumentException">Thrown when the <see cref="Segment2D"/> in <paramref name="innerLoop"/> 
        /// do not form a loop.</exception>
        internal void AddInnerLoop(IEnumerable<Segment2D> innerLoop)
        {
            Segment2D[] loop = innerLoop.ToArray();
            CheckValidLoop(loop);
            innerLoops.Add(loop);
        }

        private static void CheckValidLoop(Segment2D[] innerLoop)
        {
            if (innerLoop.Length == 1 || !IsLoopConnected(innerLoop))
            {
                throw new ArgumentException(Resources.SoilLayer2D_CheckValidLoop_Loop_contains_disconnected_segments);
            }
        }

        private static bool IsLoopConnected(Segment2D[] segments)
        {
            int segmentCount = segments.Length;
            if (segmentCount == 2)
            {
                return segments[0].Equals(segments[1]);
            }
            for (var i = 0; i < segmentCount; i++)
            {
                Segment2D segmentA = segments[i];
                Segment2D segmentB = segments[(i + 1) % segmentCount];
                if (!segmentA.IsConnected(segmentB))
                {
                    return false;
                }
            }
            return true;
        }
    }
}