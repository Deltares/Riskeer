using System.Collections.Generic;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries.Utilities;
using GisSharpBlog.NetTopologySuite.Triangulate.QuadEdge;
using Wintellect.PowerCollections;

namespace GisSharpBlog.NetTopologySuite.Triangulate
{
    /// <summary>
    /// A utility class which creates Conforming Delaunay Trianglulations
    /// from collections of points and linear constraints, and extract the resulting 
    /// triangulation edges or triangles as geometries. 
    /// </summary>
    /// <author>Martin Davis</author>
    public class ConformingDelaunayTriangulationBuilder
    {
        private readonly IDictionary<ICoordinate, Vertex> _constraintVertexMap = new OrderedDictionary<ICoordinate, Vertex>();
        private ICollection<ICoordinate> _siteCoords;
        private IGeometry _constraintLines;
        private QuadEdgeSubdivision _subdiv;

        /// <summary>
        /// Sets the linear constraints to be conformed to.
        /// All linear components in the input will be used as constraints.
        /// The constraint vertices do not have to be disjoint from 
        /// the site vertices.
        /// </summary>
        /// <remarks>The lines to constraint to</remarks>
        ///
        public IGeometry Constraints
        {
            set
            {
                _constraintLines = value;
            }
        }

        /// <summary>
        /// Sets the snapping tolerance which will be used
        /// to improved the robustness of the triangulation computation.
        /// A tolerance of 0.0 specifies that no snapping will take place.
        /// </summary>
        /// <remarks>The tolerance distance to use</remarks>
        public double Tolerance { get; set; }

        /// <summary>
        /// Sets the sites (point or vertices) which will be triangulated.
        /// All vertices of the given geometry will be used as sites.
        /// The site vertices do not have to contain the constraint
        /// vertices as well; any site vertices which are 
        /// identical to a constraint vertex will be removed
        /// from the site vertex set.
        /// </summary>
        /// <remarks>The geometry from which the sites will be extracted.</remarks>
        public void SetSites(IGeometry sites)
        {
            _siteCoords = DelaunayTriangulationBuilder.ExtractUniqueCoordinates(sites);
        }

        /// <summary>
        /// Gets the QuadEdgeSubdivision which models the computed triangulation.
        /// </summary>
        /// <returns>The subdivision containing the triangulation</returns>
        public QuadEdgeSubdivision GetSubdivision()
        {
            Create();
            return _subdiv;
        }

        /// <summary>
        /// Gets the edges of the computed triangulation as a <see cref="IMultiLineString"/>.
        /// </summary>
        /// <param name="geomFact">The geometry factory to use to create the output</param>
        /// <returns>the edges of the triangulation</returns>
        public IMultiLineString GetEdges(IGeometryFactory geomFact)
        {
            Create();
            return _subdiv.GetEdges(geomFact);
        }

        /// <summary>
        /// Gets the faces of the computed triangulation as a <see cref="IGeometryCollection"/> 
        /// of <see cref="Polygon"/>.
        /// </summary>
        /// <param name="geomFact">the geometry factory to use to create the output</param>
        /// <returns>the faces of the triangulation</returns>
        public IGeometryCollection GetTriangles(IGeometryFactory geomFact)
        {
            Create();
            return _subdiv.GetTriangles(geomFact);
        }

        private void Create()
        {
            if (_subdiv != null)
            {
                return;
            }

            var siteEnv = DelaunayTriangulationBuilder.Envelope(_siteCoords);

            IList<Segment> segments = new List<Segment>();
            if (_constraintLines != null)
            {
                siteEnv.ExpandToInclude(_constraintLines.EnvelopeInternal);
                CreateVertices(_constraintLines);
                segments = CreateConstraintSegments(_constraintLines);
            }

            var sites = CreateSiteVertices(_siteCoords);

            var cdt = new ConformingDelaunayTriangulator(sites, Tolerance);

            cdt.SetConstraints(segments, new List<Vertex>(_constraintVertexMap.Values));

            cdt.FormInitialDelaunay();
            cdt.EnforceConstraints();
            _subdiv = cdt.Subdivision;
        }

        private IEnumerable<Vertex> CreateSiteVertices(IEnumerable<ICoordinate> coords)
        {
            var verts = new List<Vertex>();
            foreach (var coord in coords)
            {
                if (_constraintVertexMap.ContainsKey(coord))
                {
                    continue;
                }
                verts.Add(new ConstraintVertex(coord));
            }
            return verts;
        }

        private void CreateVertices(IGeometry geom)
        {
            var coords = geom.Coordinates;
            for (int i = 0; i < coords.Length; i++)
            {
                Vertex v = new ConstraintVertex(coords[i]);
                _constraintVertexMap.Add(coords[i], v);
            }
        }

        private static IList<Segment> CreateConstraintSegments(IGeometry geom)
        {
            var lines = LinearComponentExtracter.GetLines(geom);
            var constraintSegs = new List<Segment>();
            foreach (var line in lines)
            {
                CreateConstraintSegments((ILineString) line, constraintSegs);
            }
            return constraintSegs;
        }

        private static void CreateConstraintSegments(ILineString line, IList<Segment> constraintSegs)
        {
            var coords = line.Coordinates;
            for (int i = 1; i < coords.Length; i++)
            {
                constraintSegs.Add(new Segment(coords[i - 1], coords[i]));
            }
        }
    }
}