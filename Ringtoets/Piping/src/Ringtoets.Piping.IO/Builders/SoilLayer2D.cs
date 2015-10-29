using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.IO.Calculation;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a DSoilModel database. Instances of this class are transient and are not to be used
    /// once the DSoilModel database has been imported.
    /// </summary>
    internal class SoilLayer2D
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2D"/>.
        /// </summary>
        public SoilLayer2D()
        {
            InnerLoops = new Collection<HashSet<Point3D>>();
        }

        /// <summary>
        /// Gets the outer loop of the <see cref="SoilLayer2D"/> as a <see cref="HashSet{T}"/> of <see cref="Point3D"/>.
        /// </summary>
        internal HashSet<Point3D> OuterLoop { get; set; }

        /// <summary>
        /// Gets the <see cref="Collection{T}"/> of inner loops (as <see cref="HashSet{T}"/> of <see cref="Point3D"/>) of the <see cref="SoilLayer2D"/>.
        /// </summary>
        internal Collection<HashSet<Point3D>> InnerLoops { get; private set; }

        public bool IsAquifer { get; set; }

        /// <summary>
        /// Constructs a (1D) <see cref="PipingSoilLayer"/> based on the <see cref="InnerLoops"/> and <see cref="OuterLoop"/> set for the <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <param name="atX">The point from which to take a 1D profile.</param>
        /// <param name="bottom">The bottom level of the <see cref="PipingSoilLayer"/>.</param>
        /// <returns></returns>
        internal IEnumerable<PipingSoilLayer> AsPipingSoilLayers(double atX, out double bottom)
        {
            bottom = Double.MaxValue;
            var result = new Collection<PipingSoilLayer>();
            if (OuterLoop != null)
            {
                Collection<double> outerLoopIntersectionHeights = GetLoopIntersectionHeights(OuterLoop, atX);
                
                if (outerLoopIntersectionHeights.Count > 0)
                {
                    IEnumerable<Collection<double>> innerLoopsIntersectionHeights = InnerLoops.Select(loop => GetLoopIntersectionHeights(loop, atX));
                    IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(innerLoopsIntersectionHeights).ToList();
                    IEnumerable<Tuple<double, double>> outerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(outerLoopIntersectionHeights).ToList();

                    var currentBottom = outerLoopIntersectionHeightPairs.First().Item1;
                    var heights = new List<double>();
                    heights.AddRange(innerLoopIntersectionHeightPairs.Where(p => p.Item1 >= currentBottom).Select(p => p.Item1));
                    heights.AddRange(outerLoopIntersectionHeightPairs.Select(p => p.Item2));

                    foreach (var height in heights.Where(height => !innerLoopIntersectionHeightPairs.Any(tuple => HeightInInnerLoop(tuple, height))))
                    {
                        result.Add(new PipingSoilLayer(height)
                        {
                            IsAquifer = IsAquifer
                        });
                    }
                    bottom = EnsureBottomOutsideInnerLoop(innerLoopIntersectionHeightPairs, currentBottom);
                }
            }
            return result;
        }

        private double EnsureBottomOutsideInnerLoop(IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs, double bottom)
        {
            var newBottom = bottom;
            var heigthPairArray = innerLoopIntersectionHeightPairs.ToList();
            var overlappingInnerLoop = heigthPairArray.FirstOrDefault(t => BottomInInnerLoop(t, newBottom));

            while (overlappingInnerLoop != null)
            {
                newBottom = overlappingInnerLoop.Item2;
                overlappingInnerLoop = heigthPairArray.FirstOrDefault(t => BottomInInnerLoop(t, newBottom));
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

        private IEnumerable<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<Collection<double>> innerLoopsIntersectionPoints)
        {
            Collection<Tuple<double,double>> result = new Collection<Tuple<double, double>>();
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
            for (int i = 0; i < orderedHeights.Count; i = i+2)
            {
                var first = orderedHeights[i];
                var second = orderedHeights[i+1];
                result.Add(new Tuple<double, double>(first, second));
            }
            return result;
        }

        private Collection<double> GetLoopIntersectionHeights(HashSet<Point3D> loop, double atX)
        {
            Collection<double> intersectionPointY = new Collection<double>(); ;
            for (int segmentIndex = 0; segmentIndex < loop.Count; segmentIndex++)
            {
                var intersectionPoint = GetSegmentIntersectionAtX(loop, segmentIndex, atX);

                if (intersectionPoint.Length > 0)
                {
                    intersectionPointY.Add(intersectionPoint[1]);
                }
                else if (IsVerticalAtX(GetSegmentWithStartAtIndex(loop, segmentIndex), atX))
                {
                    throw new SoilLayer2DConversionException(String.Format(Resources.Error_CanNotDetermine1DProfileWithVerticalSegmentsAtX, atX));
                }
            }
            return intersectionPointY;
        }

        private bool IsVerticalAtX(Point3D[] segment, double atX)
        {
            return Math.Abs(segment[0].X - atX) + Math.Abs(segment[1].X - atX) < Math2D.EpsilonForComparisons;
        }

        private static Point3D[] GetSegmentWithStartAtIndex(HashSet<Point3D> loop, int i)
        {
            var current = loop.ElementAt(i);
            var next = loop.ElementAt((i + 1) % loop.Count);

            return new[]
            {
                current,
                next
            };
        }

        private static double[] GetSegmentIntersectionAtX(HashSet<Point3D> loop, int segmentIndex, double atX)
        {
            Point3D[] segment = GetSegmentWithStartAtIndex(loop, segmentIndex);

            return Math2D.LineSegmentIntersectionWithLine(new[]
            {
                segment[0].X,
                segment[1].X,
                atX,
                atX
            }, new[]
            {
                segment[0].Z,
                segment[1].Z,
                0,
                1
            });
        }
    }
}