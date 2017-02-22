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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Piping.IO.Exceptions;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a D-Soil Model database. 
    /// Instances of this class are transient and are not to be used once the D-Soil Model
    /// database has been imported.
    /// </summary>
    internal class SoilLayer2D : GenericSoilLayerParameters
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
                var loop = value.ToArray();
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
        /// <param name="innerLoop">The innerloop to add.</param>
        /// <exception cref="ArgumentException">Thrown when the <see cref="Segment2D"/> in <paramref name="innerLoop"/> 
        /// do not form a loop.</exception>
        internal void AddInnerLoop(IEnumerable<Segment2D> innerLoop)
        {
            var loop = innerLoop.ToArray();
            CheckValidLoop(loop);
            innerLoops.Add(loop);
        }

        /// <summary>
        /// Constructs a (1D) <see cref="PipingSoilLayer"/> based on the <see cref="InnerLoops"/> and 
        /// <see cref="OuterLoop"/> set for the <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <param name="atX">The point from which to take a 1D profile.</param>
        /// <param name="bottom">The bottom level of the <see cref="PipingSoilLayer"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="PipingSoilLayer"/>.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when either:
        /// <list type="bullet">
        /// <item>any of the <see cref="InnerLoops"/> or <see cref="OuterLoop"/> contain a vertical 
        /// line at <paramref name="atX"/></item>
        /// <item>any of the distributions of the stochastic parameters is not defined as lognormal 
        /// or is shifted when it should not be</item>
        /// </list>
        /// </exception>
        internal IEnumerable<PipingSoilLayer> AsPipingSoilLayers(double atX, out double bottom)
        {
            ValidateStochasticParametersForPiping();

            bottom = double.MaxValue;
            var result = new Collection<PipingSoilLayer>();
            if (OuterLoop != null)
            {
                double[] outerLoopIntersectionHeights = GetLoopIntersectionHeights(outerLoop, atX).ToArray();
                if (outerLoopIntersectionHeights.Any())
                {
                    IEnumerable<IEnumerable<double>> innerLoopsIntersectionHeights = InnerLoops.Select(loop => GetLoopIntersectionHeights(loop, atX));
                    IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(innerLoopsIntersectionHeights).ToList();
                    IEnumerable<Tuple<double, double>> outerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(outerLoopIntersectionHeights).ToList();

                    var currentBottom = outerLoopIntersectionHeightPairs.First().Item1;
                    var heights = new List<double>();
                    heights.AddRange(innerLoopIntersectionHeightPairs.Where(p => p.Item1 >= currentBottom).Select(p => p.Item1));
                    heights.AddRange(outerLoopIntersectionHeightPairs.Select(p => p.Item2));

                    foreach (var height in heights.Where(height => !innerLoopIntersectionHeightPairs.Any(tuple => HeightInInnerLoop(tuple, height))))
                    {
                        var pipingSoilLayer = new PipingSoilLayer(height)
                        {
                            IsAquifer = IsAquifer.HasValue && IsAquifer.Value.Equals(1.0),
                            MaterialName = MaterialName ?? string.Empty,
                            Color = SoilLayerColorConversionHelper.ColorFromNullableDouble(Color)
                        };

                        SetOptionalStochasticParameters(pipingSoilLayer);

                        result.Add(pipingSoilLayer);
                    }
                    bottom = EnsureBottomOutsideInnerLoop(innerLoopIntersectionHeightPairs, currentBottom);
                }
            }
            return result;
        }

        private static void CheckValidLoop(Segment2D[] innerLoop)
        {
            if (innerLoop.Length == 1 || !IsLoopConnected(innerLoop))
            {
                throw new ArgumentException(Resources.SoilLayer2D_Error_Loop_contains_disconnected_segments);
            }
        }

        private static bool IsLoopConnected(Segment2D[] segments)
        {
            int segmentCount = segments.Length;
            if (segmentCount == 2)
            {
                return segments[0].Equals(segments[1]);
            }
            for (int i = 0; i < segmentCount; i++)
            {
                var segmentA = segments[i];
                var segmentB = segments[(i + 1)%segmentCount];
                if (!segmentA.IsConnected(segmentB))
                {
                    return false;
                }
            }
            return true;
        }

        private double EnsureBottomOutsideInnerLoop(IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs, double bottom)
        {
            var newBottom = bottom;
            var heightPairArray = innerLoopIntersectionHeightPairs.ToList();
            var overlappingInnerLoop = heightPairArray.FirstOrDefault(t => BottomInInnerLoop(t, newBottom));

            while (overlappingInnerLoop != null)
            {
                newBottom = overlappingInnerLoop.Item2;
                overlappingInnerLoop = heightPairArray.FirstOrDefault(t => BottomInInnerLoop(t, newBottom));
            }
            return newBottom;
        }

        private bool HeightInInnerLoop(Tuple<double, double> tuple, double height)
        {
            return height <= tuple.Item2 && height > tuple.Item1;
        }

        private bool BottomInInnerLoop(Tuple<double, double> tuple, double height)
        {
            return height < tuple.Item2 && height >= tuple.Item1;
        }

        private IEnumerable<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<IEnumerable<double>> innerLoopsIntersectionPoints)
        {
            Collection<Tuple<double, double>> result = new Collection<Tuple<double, double>>();
            foreach (var innerLoopIntersectionPoints in innerLoopsIntersectionPoints)
            {
                foreach (var tuple in GetOrderedStartAndEndPairsIn1D(innerLoopIntersectionPoints))
                {
                    result.Add(tuple);
                }
            }
            return result;
        }

        private static Collection<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<double> innerLoopIntersectionPoints)
        {
            var result = new Collection<Tuple<double, double>>();
            var orderedHeights = innerLoopIntersectionPoints.OrderBy(v => v).ToList();
            for (int i = 0; i < orderedHeights.Count; i = i + 2)
            {
                var first = orderedHeights[i];
                var second = orderedHeights[i + 1];
                result.Add(Tuple.Create(first, second));
            }
            return result;
        }

        /// <summary>
        /// Gets a <see cref="Collection{T}"/> of heights where the <paramref name="loop"/> intersects the 
        /// vertical line at <paramref name="atX"/>.
        /// </summary>
        /// <param name="loop">The sequence of <see cref="Segment2D"/> which together create a loop.</param>
        /// <param name="atX">The point on the x-axis where the vertical line is constructed do determine intersections with.</param>
        /// <returns>A <see cref="Collection{T}"/> of <see cref="double"/>, representing the height at which the 
        /// <paramref name="loop"/> intersects the vertical line at <paramref name="atX"/>.</returns>
        /// <exception cref="SoilLayerConversionException">Thrown when a segment is vertical at <see cref="atX"/> and thus
        /// no deterministic intersection points can be determined.</exception>
        private IEnumerable<double> GetLoopIntersectionHeights(IEnumerable<Segment2D> loop, double atX)
        {
            var segment2Ds = loop.ToArray();
            if (segment2Ds.Any(segment => IsVerticalAtX(segment, atX)))
            {
                var message = string.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_X_0_, atX);
                throw new SoilLayerConversionException(message);
            }
            return Math2D.SegmentsIntersectionWithVerticalLine(segment2Ds, atX).Select(p => p.Y);
        }

        private static bool IsVerticalAtX(Segment2D segment, double atX)
        {
            return segment.FirstPoint.X.Equals(atX) && segment.IsVertical();
        }
    }
}