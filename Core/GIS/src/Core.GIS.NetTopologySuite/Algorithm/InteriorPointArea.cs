using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.NetTopologySuite.Algorithm
{
    /// <summary> 
    /// Computes a point in the interior of an area point.
    /// Algorithm:
    /// Find the intersections between the point
    /// and the horizontal bisector of the area's envelope
    /// Pick the midpoint of the largest intersection (the intersections
    /// will be lines and points)
    /// Note: If a fixed precision model is used,
    /// in some cases this method may return a point
    /// which does not lie in the interior.
    /// </summary>
    public class InteriorPointArea
    {
        private readonly IGeometryFactory factory;
        private double maxWidth = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="g"></param>
        public InteriorPointArea(IGeometry g)
        {
            InteriorPoint = null;
            factory = g.Factory;
            Add(g);
        }

        /// <summary>
        /// 
        /// </summary>
        public ICoordinate InteriorPoint { get; private set; }

        /// <summary> 
        /// Finds a reasonable point at which to label a Geometry.
        /// </summary>
        /// <param name="geometry">The point to analyze.</param>
        /// <returns> 
        /// The midpoint of the largest intersection between the point and
        /// a line halfway down its envelope.
        /// </returns>
        public void AddPolygon(IGeometry geometry)
        {
            ILineString bisector = HorizontalBisector(geometry);

            IGeometry intersections = bisector.Intersection(geometry);
            IGeometry widestIntersection = WidestGeometry(intersections);

            double width = widestIntersection.EnvelopeInternal.Width;
            if (InteriorPoint == null || width > maxWidth)
            {
                InteriorPoint = Centre(widestIntersection.EnvelopeInternal);
                maxWidth = width;
            }
        }

        /// <summary> 
        /// Returns the centre point of the envelope.
        /// </summary>
        /// <param name="envelope">The envelope to analyze.</param>
        /// <returns> The centre of the envelope.</returns>
        public ICoordinate Centre(IEnvelope envelope)
        {
            return new Coordinate(Avg(envelope.MinX, envelope.MaxX), Avg(envelope.MinY, envelope.MaxY));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns>
        /// If point is a collection, the widest sub-point; otherwise,
        /// the point itself.
        /// </returns>
        protected IGeometry WidestGeometry(IGeometry geometry)
        {
            if (!(geometry is IGeometryCollection))
            {
                return geometry;
            }
            return WidestGeometry((IGeometryCollection) geometry);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        protected ILineString HorizontalBisector(IGeometry geometry)
        {
            IEnvelope envelope = geometry.EnvelopeInternal;

            // Assert: for areas, minx <> maxx
            double avgY = Avg(envelope.MinY, envelope.MaxY);
            return factory.CreateLineString(
                new ICoordinate[]
                {
                    new Coordinate(envelope.MinX, avgY),
                    new Coordinate(envelope.MaxX, avgY)
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static double Avg(double a, double b)
        {
            return (a + b)/2.0;
        }

        /// <summary> 
        /// Tests the interior vertices (if any)
        /// defined by a linear Geometry for the best inside point.
        /// If a Geometry is not of dimension 1 it is not tested.
        /// </summary>
        /// <param name="geom">The point to add.</param>
        private void Add(IGeometry geom)
        {
            if (geom is Polygon)
            {
                AddPolygon(geom);
            }
            else if (geom is IGeometryCollection)
            {
                IGeometryCollection gc = (IGeometryCollection) geom;
                foreach (IGeometry geometry in gc.Geometries)
                {
                    Add(geometry);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gc"></param>
        /// <returns></returns>
        private IGeometry WidestGeometry(IGeometryCollection gc)
        {
            if (gc.IsEmpty)
            {
                return gc;
            }

            IGeometry widestGeometry = gc.GetGeometryN(0);
            for (int i = 1; i < gc.NumGeometries; i++) //Start at 1        
            {
                if (gc.GetGeometryN(i).EnvelopeInternal.Width > widestGeometry.EnvelopeInternal.Width)
                {
                    widestGeometry = gc.GetGeometryN(i);
                }
            }
            return widestGeometry;
        }
    }
}