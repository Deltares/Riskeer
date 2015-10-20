using System.Collections.Generic;

namespace Wti.Data
{
    public class PipingSoilLayer
    {
        public HashSet<Point3D> OuterLoop { get; set; }
        public HashSet<Point3D> InnerLoop { get; set; }
    }
}