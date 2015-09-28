using System.Collections;
using GeoAPI.Geometries;

namespace GisSharpBlog.NetTopologySuite.Operation.Polygonize
{
    /// <summary>
    /// Polygonizes a set of Geometrys which contain linework that
    /// represents the edges of a planar graph.
    /// Any dimension of Geometry is handled - the constituent linework is extracted
    /// to form the edges.
    /// The edges must be correctly noded; that is, they must only meet
    /// at their endpoints.  The Polygonizer will still run on incorrectly noded input
    /// but will not form polygons from incorrected noded edges.
    /// The Polygonizer reports the follow kinds of errors:
    /// Dangles - edges which have one or both ends which are not incident on another edge endpoint
    /// Cut Edges - edges which are connected at both ends but which do not form part of polygon
    /// Invalid Ring Lines - edges which form rings which are invalid
    /// (e.g. the component lines contain a self-intersection).
    /// </summary>
    public class Polygonizer
    {
        /// <summary>
        /// Add every linear element in a point into the polygonizer graph.
        /// </summary>
        private class LineStringAdder : IGeometryComponentFilter
        {
            private readonly Polygonizer container;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="container"></param>
            public LineStringAdder(Polygonizer container)
            {
                this.container = container;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="g"></param>
            public void Filter(IGeometry g) 
            {
                if (g is ILineString)
					container.Add((ILineString)g);
            }
        }

        /// <summary>
        /// Default factory.
        /// </summary>
        private readonly LineStringAdder lineStringAdder;

        /// <summary>
        /// 
        /// </summary>
        protected PolygonizeGraph graph;

        /// <summary>
        /// Initialized with empty collections, in case nothing is computed
        /// </summary>
        protected IList dangles = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        protected IList cutEdges = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        protected IList invalidRingLines = new ArrayList();

        /// <summary>
        /// 
        /// </summary>
        protected IList holeList;
        
        /// <summary>
        /// 
        /// </summary>        
        protected IList shellList;

        /// <summary>
        /// 
        /// </summary>
        protected IList polyList;

        /// <summary>
        /// Create a polygonizer with the same {GeometryFactory}
        /// as the input <c>Geometry</c>s.
        /// </summary>
        public Polygonizer() 
        {
            lineStringAdder = new LineStringAdder(this);
        }

        /// <summary>
        /// Add a collection of geometries to be polygonized.
        /// May be called multiple times.
        /// Any dimension of Geometry may be added;
        /// the constituent linework will be extracted and used.
        /// </summary>
        /// <param name="geomList">A list of <c>Geometry</c>s with linework to be polygonized.</param>
        public void Add(IList geomList)
        {
            for (var i = geomList.GetEnumerator(); i.MoveNext(); ) 
            {
				var geometry = (IGeometry)i.Current;
                Add(geometry);
            }
        }

        /// <summary>
        /// Add a point to the linework to be polygonized.
        /// May be called multiple times.
        /// Any dimension of Geometry may be added;
        /// the constituent linework will be extracted and used
        /// </summary>
        /// <param name="g">A <c>Geometry</c> with linework to be polygonized.</param>
		public void Add(IGeometry g)
        {
            g.Apply(lineStringAdder);
        }

        /// <summary>
        /// Add a linestring to the graph of polygon edges.
        /// </summary>
        /// <param name="line">The <c>LineString</c> to add.</param>
        private void Add(ILineString line)
        {
            // create a new graph using the factory from the input Geometry
            if (graph == null)
				graph = new PolygonizeGraph(line.Factory);
            graph.AddEdge(line);
        }

        /// <summary>
        /// Compute and returns the list of polygons formed by the polygonization.
        /// </summary>        
        public IList GetPolygons()
        {
            Polygonize();
            return polyList;
        }

        /// <summary> 
        /// Compute and returns the list of dangling lines found during polygonization.
        /// </summary>
        public IList GetDangles()
        {
            Polygonize();
            return dangles;
        }

        /// <summary>
        /// Compute and returns the list of cut edges found during polygonization.
        /// </summary>
        public IList GetCutEdges()
        {
            Polygonize();
            return cutEdges;
        }

        /// <summary>
        /// Compute and returns the list of lines forming invalid rings found during polygonization.
        /// </summary>
        public IList GetInvalidRingLines()
        {
            Polygonize();
            return invalidRingLines;
        }

        /// <summary>
        /// Perform the polygonization, if it has not already been carried out.
        /// </summary>
        private void Polygonize()
        {
            // check if already computed
            if (polyList != null) 
                return;

            polyList = new ArrayList();

            // if no geometries were supplied it's possible graph could be null
            if (graph == null) 
                return;

            dangles = graph.DeleteDangles();
            cutEdges = graph.DeleteCutEdges();
            var edgeRingList = graph.GetEdgeRings();

            IList validEdgeRingList = new ArrayList();
            invalidRingLines = new ArrayList();
            FindValidRings(edgeRingList, validEdgeRingList, invalidRingLines);

            FindShellsAndHoles(validEdgeRingList);
            AssignHolesToShells(holeList, shellList);

            polyList = new ArrayList();
            for (var i = shellList.GetEnumerator(); i.MoveNext(); ) 
            {
                var er = (EdgeRing) i.Current;
                polyList.Add(er.Polygon);
            }
        }

        private static void FindValidRings(IList edgeRingList, IList validEdgeRingList, IList invalidRingList)
        {
            for (var i = edgeRingList.GetEnumerator(); i.MoveNext(); ) 
            {
                var er = (EdgeRing) i.Current;
                if (er.IsValid)
                     validEdgeRingList.Add(er);
                else invalidRingList.Add(er.LineString);
            }
        }

        private void FindShellsAndHoles(IList edgeRingList)
        {
            holeList = new ArrayList();
            shellList = new ArrayList();
            for (var i = edgeRingList.GetEnumerator(); i.MoveNext(); ) 
            {
                var er = (EdgeRing) i.Current;
                if (er.IsHole)
                     holeList.Add(er);
                else shellList.Add(er);

            }
        }

        private static void AssignHolesToShells(IList holeList, IList shellList)
        {
            for (var i = holeList.GetEnumerator(); i.MoveNext(); ) 
            {
                var holeER = (EdgeRing) i.Current;
                AssignHoleToShell(holeER, shellList);
            }
        }

        private static void AssignHoleToShell(EdgeRing holeER, IList shellList)
        {
            var shell = EdgeRing.FindEdgeRingContaining(holeER, shellList);
            if (shell != null)
                shell.AddHole(holeER.Ring);
        }
    }
}
