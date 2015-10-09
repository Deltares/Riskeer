using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

namespace SharpMap.CoordinateSystems
{
    public static class CoordinateSystemValidator
    {
        /// <summary>
        ///  Checks whether the coordinate system will is valid for the extent of the argument coordinate list. 
        ///  Note that this only works if the tranformation maps the inner points of the extent to the inner points of the mapped extent.
        /// </summary>
        public static bool CanAssignCoordinateSystem(IEnumerable<ICoordinate> sourceCoordinates,
                                                     ICoordinateSystem coordinateSystem)
        {
            var coordinates = sourceCoordinates as ICoordinate[] ?? sourceCoordinates.ToArray();
            return !coordinates.Any() || CanAssignCoordinateSystem(GetEnvelope(coordinates), coordinateSystem);
        }

        /// <summary>
        ///  Checks whether the coordinate system will is valid for the extent of the argument geometry list. 
        ///  Note that this only works if the tranformation maps the inner points of the extent to the inner points of the mapped extent.
        /// </summary>
        public static bool CanAssignCoordinateSystem(IEnumerable<IGeometry> sourceGeometries,
                                                     ICoordinateSystem coordinateSystem)
        {
            var geometries = sourceGeometries as IGeometry[] ?? sourceGeometries.ToArray();
            return !geometries.Any() || CanAssignCoordinateSystem(GetEnvelope(geometries), coordinateSystem);
        }

        /// <summary>
        ///  Checks whether the coordinate transformation will give a valid result on the extent of the argument coordinate list. 
        ///  Note that this only works if the tranformation maps the inner points of the extent to the inner points of the mapped extent.
        /// </summary>
        public static bool CanConvertByTransformation(IEnumerable<ICoordinate> sourceCoordinates,
                                                      ICoordinateTransformation transformation)
        {
            var coordinates = sourceCoordinates as ICoordinate[] ?? sourceCoordinates.ToArray();
            return !coordinates.Any() || CanConvertByTransformation(GetEnvelope(coordinates), transformation);
        }

        /// <summary>
        ///  Checks whether the coordinate transformation will give a valid result on the extent of the argument geometry list. 
        ///  Note that this only works if the tranformation maps the inner points of the extent to the inner points of the mapped extent.
        /// </summary>
        public static bool CanConvertByTransformation(IEnumerable<IGeometry> sourceCoordinates,
                                                      ICoordinateTransformation transformation)
        {
            var coordinates = sourceCoordinates as IGeometry[] ?? sourceCoordinates.ToArray();
            return !coordinates.Any() || CanConvertByTransformation(GetEnvelope(coordinates), transformation);
        }

        public static bool CanAssignCoordinateSystem(IEnvelope envelope, ICoordinateSystem coordinateSystem)
        {
            if (envelope == null)
            {
                return true;
            }

            var cornerPoints = new[]
            {
                new Coordinate(envelope.MinX, envelope.MinY),
                new Coordinate(envelope.MaxX, envelope.MinY),
                new Coordinate(envelope.MaxX, envelope.MaxY),
                new Coordinate(envelope.MinX, envelope.MaxY),
            };

            var transformation = CoordinateSystemFactory.CreateTransformation(coordinateSystem, Wgs84CoordinateSystem);

            try
            {
                return
                    cornerPoints.Select(
                        coordinate => transformation.MathTransform.Transform(new[]
                        {
                            coordinate.X,
                            coordinate.Y
                        }))
                                .All(ValidatePointInWgs84);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool CanConvertByTransformation(IEnvelope envelope, ICoordinateTransformation transformation)
        {
            if (envelope == null)
            {
                return true;
            }

            var cornerPoints = new[]
            {
                new Coordinate(envelope.MinX, envelope.MinY),
                new Coordinate(envelope.MaxX, envelope.MinY),
                new Coordinate(envelope.MaxX, envelope.MaxY),
                new Coordinate(envelope.MinX, envelope.MaxY),
            };

            try
            {
                var transformedEnvelope = cornerPoints.Select(
                    coordinate => transformation.MathTransform.Transform(new[]
                    {
                        coordinate.X,
                        coordinate.Y
                    })).ToList();

                var deltaXold = Math.Abs(envelope.MaxX - envelope.MinX);
                var deltaYold = Math.Abs(envelope.MaxY - envelope.MinY);

                var deltaXnew = Math.Abs(transformedEnvelope[0][0] - transformedEnvelope[1][0]);
                var deltaYnew = Math.Abs(transformedEnvelope[0][1] - transformedEnvelope[2][1]);

                if (deltaXold > 1e-5 && deltaYold > 1e-5 && (deltaXnew < 1e-5 || deltaYnew < 1e-5))
                {
                    return false;
                }

                return transformedEnvelope.All(ValidatePoint);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static ICoordinateSystemFactory CoordinateSystemFactory
        {
            get
            {
                return Map.CoordinateSystemFactory;
            }
        }

        private static ICoordinateSystem Wgs84CoordinateSystem
        {
            get
            {
                return CoordinateSystemFactory == null ? null : CoordinateSystemFactory.CreateFromEPSG(4326);
            }
        }

        private static IEnvelope GetEnvelope(IList<ICoordinate> coordinates)
        {
            var firstCoordinate = coordinates.FirstOrDefault();

            if (firstCoordinate == null)
            {
                return null;
            }

            var envelope = new Envelope(firstCoordinate);

            foreach (var coordinate in coordinates.Skip(1))
            {
                envelope.ExpandToInclude(coordinate);
            }

            return envelope;
        }

        private static IEnvelope GetEnvelope(IList<IGeometry> geometries)
        {
            var firstGeometry = geometries.FirstOrDefault();

            if (firstGeometry == null)
            {
                return null;
            }

            var envelope = firstGeometry.EnvelopeInternal;

            foreach (var geometry in geometries.Skip(1))
            {
                envelope.ExpandToInclude(geometry.EnvelopeInternal);
            }

            return envelope;
        }

        private static bool ValidatePoint(double[] coordinate)
        {
            return !double.IsInfinity(coordinate[0]) && !double.IsNaN(coordinate[0]) &&
                   !double.IsInfinity(coordinate[1]) && !double.IsNaN(coordinate[1]);
        }

        private static bool ValidatePointInWgs84(double[] coordinate)
        {
            return coordinate[0] > -180.0 && coordinate[0] < 180.0 && coordinate[1] > -90.0 && coordinate[1] < 90.0;
        }
    }
}