using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Wti.Data;
using Wti.IO.Calculation;

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
                Collection<double> intersectionPointY = new Collection<double>();
                for (int i = 0; i < OuterLoop.Count; i++)
                {
                    var current = OuterLoop.ElementAt(i);
                    var next = OuterLoop.ElementAt((i + 1)%OuterLoop.Count);

                    var intersectionPoint = Math2D.LineSegmentIntersectionWithLine(new[]
                    {
                        current.X,
                        next.X,
                        atX,
                        atX
                    }, new[]
                    {
                        current.Z,
                        next.Z,
                        0,
                        1
                    });
                    if (intersectionPoint.Length > 0)
                    {
                        intersectionPointY.Add(intersectionPoint[1]);
                    }
                }
                if (intersectionPointY.Count > 0)
                {
                    result.Add(new PipingSoilLayer(intersectionPointY.Max()));
                    bottom = intersectionPointY.Min();
                }
            }
            return result;
        }
    }
}