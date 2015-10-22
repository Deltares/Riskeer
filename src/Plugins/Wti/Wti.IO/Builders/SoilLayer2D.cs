using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Wti.Data;
using Wti.IO.Calculation;
using Wti.IO.Properties;

namespace Wti.IO.Builders
{
    public class SoilLayer2D
    {
        public SoilLayer2D()
        {
            InnerLoops = new Collection<HashSet<Point3D>>();
        }

        public HashSet<Point3D> OuterLoop { get; set; }
        public Collection<HashSet<Point3D>> InnerLoops { get; private set; }

        internal IEnumerable<PipingSoilLayer> AsPipingSoilLayers(double atX, out double bottom)
        {
            bottom = Double.MaxValue;
            var result = new Collection<PipingSoilLayer>();
            if (OuterLoop != null)
            {
                Collection<double> outerLoopIntersectionHeights = GetLoopIntersectionHeights(OuterLoop, atX);
                
                if (outerLoopIntersectionHeights.Count > 0)
                {
                    Collection<Tuple<double, double>> outerLoopIntersectionHeightPairs = new Collection<Tuple<double, double>>();
                    AddOrderedStartAndEndPairsIn1D(outerLoopIntersectionHeights, outerLoopIntersectionHeightPairs);
                    foreach (var heightPair in outerLoopIntersectionHeightPairs)
                    {
                        result.Add(new PipingSoilLayer(heightPair.Item2));
                    }
                    bottom = outerLoopIntersectionHeightPairs[0].Item1;

                    IEnumerable<Collection<double>> innerLoopsIntersectionHeights = InnerLoops.Select(loop => GetLoopIntersectionHeights(loop, atX));
                    IEnumerable<Tuple<double, double>> innerLoopIntersectionHeightPairs = GetOrderedStartAndEndPairsIn1D(innerLoopsIntersectionHeights).ToList();

                    bottom = EnsureBottomOutsideInnerLoop(innerLoopIntersectionHeightPairs.ToList(), bottom);

                    foreach (var heightPair in innerLoopIntersectionHeightPairs)
                    {
                        if (heightPair.Item1 > bottom)
                        {
                            result.Add(new PipingSoilLayer(heightPair.Item1));
                        }
                    }
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

        private bool BottomInInnerLoop(Tuple<double, double> tuple, double bottom)
        {
            return bottom < tuple.Item2 && bottom >= tuple.Item1;
        }

        private IEnumerable<Tuple<double, double>> GetOrderedStartAndEndPairsIn1D(IEnumerable<Collection<double>> innerLoopsIntersectionPoints)
        {
            Collection<Tuple<double,double>> result = new Collection<Tuple<double, double>>();
            foreach (var innerLoopIntersectionPoints in innerLoopsIntersectionPoints)
            {
                AddOrderedStartAndEndPairsIn1D(innerLoopIntersectionPoints, result);
            }
            return result;
        }

        private static void AddOrderedStartAndEndPairsIn1D(Collection<double> innerLoopIntersectionPoints, Collection<Tuple<double, double>> result)
        {
            var orderedHeights = innerLoopIntersectionPoints.OrderBy(v => v).ToList();
            while (orderedHeights.Count >= 2)
            {
                var first = orderedHeights[0];
                var second = orderedHeights[1];
                orderedHeights.RemoveRange(0, 2);
                result.Add(new Tuple<double, double>(first, second));
            }
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