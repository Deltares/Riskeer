using System.Collections;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Algorithm;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.NetTopologySuite.GeometriesGraph;
using Core.GIS.NetTopologySuite.Index.Quadtree;
using Core.GIS.NetTopologySuite.Utilities;

namespace Core.GIS.NetTopologySuite.Operation.Valid
{
    /// <summary>
    /// Tests whether any of a set of <c>LinearRing</c>s are
    /// nested inside another ring in the set, using a <c>Quadtree</c>
    /// index to speed up the comparisons.
    /// </summary>
    public class QuadtreeNestedRingTester
    {
        private readonly GeometryGraph graph; // used to find non-node vertices
        private readonly IList rings = new ArrayList();
        private readonly IEnvelope totalEnv = new Envelope();
        private Quadtree quadtree;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graph"></param>
        public QuadtreeNestedRingTester(GeometryGraph graph)
        {
            this.graph = graph;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate NestedPoint { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ring"></param>
        public void Add(ILinearRing ring)
        {
            rings.Add(ring);
            totalEnv.ExpandToInclude(ring.EnvelopeInternal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsNonNested()
        {
            BuildQuadtree();

            for (int i = 0; i < rings.Count; i++)
            {
                ILinearRing innerRing = (ILinearRing) rings[i];
                ICoordinate[] innerRingPts = innerRing.Coordinates;

                IList results = quadtree.Query(innerRing.EnvelopeInternal);
                for (int j = 0; j < results.Count; j++)
                {
                    ILinearRing searchRing = (ILinearRing) results[j];
                    ICoordinate[] searchRingPts = searchRing.Coordinates;

                    if (innerRing == searchRing)
                    {
                        continue;
                    }

                    if (!innerRing.EnvelopeInternal.Intersects(searchRing.EnvelopeInternal))
                    {
                        continue;
                    }

                    ICoordinate innerRingPt = IsValidOp.FindPointNotNode(innerRingPts, searchRing, graph);
                    Assert.IsTrue(innerRingPt != null, "Unable to find a ring point not a node of the search ring");

                    bool isInside = CGAlgorithms.IsPointInRing(innerRingPt, searchRingPts);
                    if (isInside)
                    {
                        NestedPoint = innerRingPt;
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        private void BuildQuadtree()
        {
            quadtree = new Quadtree();

            for (int i = 0; i < rings.Count; i++)
            {
                ILinearRing ring = (ILinearRing) rings[i];
                Envelope env = (Envelope) ring.EnvelopeInternal;
                quadtree.Insert(env, ring);
            }
        }
    }
}