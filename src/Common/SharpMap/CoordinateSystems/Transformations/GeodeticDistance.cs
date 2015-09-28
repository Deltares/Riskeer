using System;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;

namespace SharpMap.CoordinateSystems.Transformations
{
    /// <summary>
    /// Calculate distance between two coordinates, taking coordinate system into account. Uses haversine formula as approximation.
    /// </summary>
    public class GeodeticDistance
    {
        /// <summary>
        /// Returns the (great-circle) distance (in meters) between the two coordinates.
        /// </summary>
        /// <returns></returns>
        public static double Distance(ICoordinateSystem coordinateSystem, ICoordinate c1, ICoordinate c2)
        {
            return new GeodeticDistance(coordinateSystem).Distance(c1, c2);
        }

        /// <summary>
        /// Returns the (great-circle) length (in meters) of a geometry (calculated as the sum of distances between the coordinates of the geometry)
        /// </summary>
        /// <returns></returns>
        public static double Length(ICoordinateSystem coordinateSystem, IGeometry geometry)
        {
            var distance = 0.0;
            var haversine = new GeodeticDistance(coordinateSystem);
            for (var i = 1; i < geometry.Coordinates.Length; i++)
                distance += haversine.Distance(geometry.Coordinates[i - 1], geometry.Coordinates[i]);
            return distance;
        }

        /// <summary>
        /// Returns the (great-circle) distance (in meters) between the two coordinates.
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public double Distance(ICoordinate c1, ICoordinate c2)
        {
            var transformedC1 = transformation.MathTransform.Transform(new[] {c1.X, c1.Y});
            var transformedC2 = transformation.MathTransform.Transform(new[] {c2.X, c2.Y});
            return GetHaversineDistanceInMeters(transformedC1[1], transformedC1[0], transformedC2[1], transformedC2[0]);
        }

        // http://rosettacode.org/wiki/Haversine_formula#C.23
        private static double GetHaversineDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6372800; // in meters
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);
            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat/2)*Math.Sin(dLat/2) +
                    Math.Sin(dLon/2)*Math.Sin(dLon/2)*Math.Cos(lat1)*Math.Cos(lat2);
            return R*2*Math.Asin(Math.Sqrt(a));
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        private static ICoordinateSystem Wgs84;
        private readonly ICoordinateTransformation transformation;

        public GeodeticDistance(ICoordinateSystem sourceSystem)
        {
            if (sourceSystem == null)
                throw new InvalidOperationException("You must supply the source coordinate system");

            if (Wgs84 == null)
                Wgs84 = Map.CoordinateSystemFactory.CreateFromEPSG(4326);

            transformation = Map.CoordinateSystemFactory.CreateTransformation(sourceSystem, Wgs84);
        }
    }
}