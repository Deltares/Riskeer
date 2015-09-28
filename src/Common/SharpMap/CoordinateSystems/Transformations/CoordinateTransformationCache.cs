using System.Collections.Generic;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace SharpMap.CoordinateSystems.Transformations
{
    public class CoordinateTransformationCache
    {
        private ICoordinateTransformation coordinateTransformation;
        private Dictionary<ICoordinate, ICoordinate> transformationCache;

        public void SetTransformation(ICoordinateTransformation coordinateTransformation)
        {
            if (Equals(this.coordinateTransformation, coordinateTransformation)) 
                return;

            this.coordinateTransformation = coordinateTransformation;
            transformationCache = new Dictionary<ICoordinate, ICoordinate>();
        }
        
        public ICoordinate PerformProjection(ICoordinate world)
        {
            if (coordinateTransformation != null)
            {
                // use cache if possible
                ICoordinate transformedCoordinate;
                if (!transformationCache.TryGetValue(world, out transformedCoordinate))
                {
                    var xy = coordinateTransformation.MathTransform.Transform(new[] { world.X, world.Y });
                    transformedCoordinate = new Coordinate(xy[0], xy[1]);
                    transformationCache[world] = transformedCoordinate;
                }
                return transformedCoordinate;
            }
            return world;
        }
    }
}