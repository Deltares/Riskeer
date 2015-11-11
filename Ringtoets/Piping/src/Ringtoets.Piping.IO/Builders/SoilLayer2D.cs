using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Calculation;
using Ringtoets.Piping.IO.Properties;

namespace Ringtoets.Piping.IO.Builders
{
    /// <summary>
    /// This class represents objects which were imported from a DSoilModel database. Instances of this class are transient and are not to be used
    /// once the DSoilModel database has been imported.
    /// </summary>
    internal class SoilLayer2D
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
        /// Gets or sets a <see cref="double"/> value representing 
        /// whether the <see cref="SoilLayer2D"/> is an aquifer.
        /// </summary>
        public double? IsAquifer { get; set; }

        /// <summary>
        /// Gets or sets the above phreatic level for the <see cref="SoilLayer2D"/>.
        /// </summary>
        public double? AbovePhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the below phreatic level for the <see cref="SoilLayer2D"/>.
        /// </summary>
        public double? BelowPhreaticLevel { get; set; }

        /// <summary>
        /// Gets or sets the dry unit weight for the <see cref="SoilLayer2D"/>.
        /// </summary>
        public double? DryUnitWeight { get; set; }

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
                var loop = value.ToArray();;
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
        /// Constructs a (1D) <see cref="PipingSoilLayer"/> based on the <see cref="InnerLoops"/> and <see cref="OuterLoop"/> set for the <see cref="SoilLayer2D"/>.
        /// </summary>
        /// <param name="atX">The point from which to take a 1D profile.</param>
        /// <param name="bottom">The bottom level of the <see cref="PipingSoilLayer"/>.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="PipingSoilLayer"/>.</returns>
        /// <exception cref="SoilLayer2DConversionException">Thrown when any of the <see cref="InnerLoops"/> or
        /// <see cref="OuterLoop"/> contain a vertical line at <paramref name="atX"/>.</exception>
        internal IEnumerable<PipingSoilLayer> AsPipingSoilLayers(double atX, out double bottom)
        {
            bottom = Double.MaxValue;
            var result = new Collection<PipingSoilLayer>();
            if (OuterLoop != null)
            {
                IEnumerable<double> outerLoopIntersectionHeights = GetLoopIntersectionHeights(outerLoop, atX);

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
                        result.Add(new PipingSoilLayer(height)
                        {
                            IsAquifer = IsAquifer.HasValue && IsAquifer.Value.Equals(1.0),
                            BelowPhreaticLevel = BelowPhreaticLevel,
                            AbovePhreaticLevel = AbovePhreaticLevel,
                            DryUnitWeight = DryUnitWeight
                        });
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
                result.Add(new Tuple<double, double>(first, second));
            }
            return result;
        }

        /// <summary>
        /// Gets a <see cref="Collection{T}"/> of heights where the <paramref name="loop"/> intersects the 
        /// vertical line at <paramref name="atX"/>.
        /// </summary>
        /// <param name="loop">The <see cref="HashSet{T}"/> of <see cref="Point3D"/> which together create a loop.</param>
        /// <param name="atX">The point on the x-axis where the vertical line is constructed do determine intersections with.</param>
        /// <returns>A <see cref="Collection{T}"/> of <see cref="double"/>, representing the height at which the 
        /// <paramref name="loop"/> intersects the vertical line at <paramref name="atX"/>.</returns>
        /// <exception cref="SoilLayer2DConversionException">Thrown when a segment is vertical at <see cref="atX"/> and thus
        /// no deterministic intersection points can be determined.</exception>
        private IEnumerable<double> GetLoopIntersectionHeights(IEnumerable<Segment2D> loop, double atX)
        {
            var segment2Ds = loop.ToArray();
            if(segment2Ds.Any(segment => IsVerticalAtX(segment, atX)))
            {
                var message = string.Format(Resources.Error_Can_not_determine_1D_profile_with_vertical_segments_at_x, atX);
                throw new SoilLayer2DConversionException(message);
            }
            return Math2D.SegmentsIntersectionWithVerticalLine(segment2Ds, atX).Select(p => p.Y);
        }

        private static bool IsVerticalAtX(Segment2D segment, double atX)
        {
            return segment.FirstPoint.X.Equals(atX) && segment.IsVertical();
        }
    }
}