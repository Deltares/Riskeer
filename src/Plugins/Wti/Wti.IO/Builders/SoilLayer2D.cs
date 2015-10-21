using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Wti.Data;

namespace Wti.IO.Builders
{
    public class SoilLayer2D
    {
        public HashSet<Point3D> OuterLoop { get; set; }
        public HashSet<Point3D> InnerLoop { get; set; }

        internal IEnumerable<PipingSoilLayer> AsPipingSoilLayers(double atX, out double bottom)
        {
            bottom = Double.MaxValue;
            var result = new Collection<PipingSoilLayer>();
            if (OuterLoop != null)
            {
                for (int i = 0; i <= OuterLoop.Count; i++)
                {
                    var current = i;
                    var next = (i+i) %OuterLoop.Count;

                }
            }
            return result;
        }
    }
}