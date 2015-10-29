using System;
using System.Collections.Generic;
using System.Linq;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Extensions.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;

namespace Core.GIS.SharpMap.Editors.Snapping
{
    public class SnapRule : ISnapRule
    {
        /// <summary>
        /// Criteria is used to select (filter) feature candidates (where can we snap)
        /// </summary>
        public Func<ILayer, IFeature, bool> Criteria { get; set; }

        /// <summary>
        /// Target layer, where new features will be created.
        /// </summary>
        public ILayer NewFeatureLayer { get; set; }

        public SnapRole SnapRole { get; set; }

        public virtual bool Obligatory { get; set; }

        /// <summary>
        /// Number of pixels where snapping will start working.
        /// 
        /// Used to construct the envelope used to select features which should be evaluated by this snap rule.
        /// </summary>
        public int PixelGravity { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceFeature"></param>
        /// <param name="candidates"></param>
        /// <param name="sourceGeometry"></param>
        /// <param name="snapTargets"></param>
        /// <param name="worldPos"></param>
        /// <param name="envelope"></param>
        /// <param name="trackingIndex"></param>
        /// based of the selected tracker in the snapSource the rule can behave differently
        /// (only snap branches' first and last coordinate).
        /// <returns></returns>
        public virtual SnapResult Execute(IFeature sourceFeature, Tuple<IFeature, ILayer>[] candidates, IGeometry sourceGeometry, IList<IFeature> snapTargets, ICoordinate worldPos, IEnvelope envelope, int trackingIndex)
        {
            if (candidates == null || SnapRole == SnapRole.None)
            {
                return null;
            }

            // hack preserve snapTargets functionality
            var snapTargetGeometries = (snapTargets != null ? snapTargets.Select(t => t.Geometry) : Enumerable.Empty<IGeometry>()).ToList();

            var minDistance = double.MaxValue; // TODO: incapsulate minDistance in ISnapResult
            SnapResult lastSnapResult = null;

            foreach (var candidate in candidates)
            {
                var feature = candidate.Item1;
                var layer = candidate.Item2;

                if ((Criteria != null && !Criteria(layer, feature)) || (snapTargets != null && snapTargetGeometries.IndexOf(feature.Geometry) == -1))
                {
                    continue;
                }

                var geometryToSnap = layer.CoordinateTransformation != null
                                         ? GeometryTransform.TransformGeometry(feature.Geometry, layer.CoordinateTransformation.MathTransform)
                                         : feature.Geometry;

                var snapResult = GetSnapResultForGeometry(worldPos, geometryToSnap, ref minDistance, feature);
                if (snapResult == null)
                {
                    continue;
                }

                snapResult.NewFeatureLayer = NewFeatureLayer;
                lastSnapResult = snapResult;
            }

            return lastSnapResult;
        }

        private SnapResult GetSnapResultForGeometry(ICoordinate worldPos, IGeometry geometry, ref double minDistance, IFeature feature)
        {
            var polygon = geometry as IPolygon;
            if (polygon != null)
            {
                switch (SnapRole)
                {
                    case SnapRole.Free:
                        return PolygonSnapFree(ref minDistance, polygon, worldPos);
                    case SnapRole.AllTrackers:
                        return GeometrySnapAllTrackers(ref minDistance, polygon, worldPos);
                    default:
                        return PolygonSnapFreeAtObject(ref minDistance, polygon, worldPos);
                }
            }

            var lineString = geometry as ILineString;
            if (lineString != null)
            {
                switch (SnapRole)
                {
                    case SnapRole.Free:
                        return LineStringSnapFree(ref minDistance, lineString, worldPos);
                    case SnapRole.FreeAtObject:
                        return LineStringSnapFreeAtObject(ref minDistance, feature, lineString, worldPos);
                    case SnapRole.TrackersNoStartNoEnd:
                        return null;
                    case SnapRole.AllTrackers:
                        return LineStringSnapAllTrackers(ref minDistance, lineString, worldPos);
                    case SnapRole.Start:
                        return LineStringSnapStart(lineString);
                    case SnapRole.End:
                        return LineStringSnapEnd(lineString);
                    case SnapRole.StartEnd:
                        return LineStringSnapStartEnd(ref minDistance, lineString, worldPos);
                }
            }

            var multiLineString = geometry as IMultiLineString;
            if (multiLineString != null)
            {
                foreach (var line in multiLineString.Geometries.OfType<ILineString>())
                {
                    var snapresult = GetSnapResultForGeometry(worldPos, line, ref minDistance, feature);
                    if (snapresult != null)
                    {
                        return snapresult;
                    }
                }
            }

            var multiPolygon = geometry as IMultiPolygon;
            if (multiPolygon != null)
            {
                foreach (var line in multiPolygon.Geometries.OfType<IPolygon>())
                {
                    var snapresult = GetSnapResultForGeometry(worldPos, line, ref minDistance, feature);
                    if (snapresult != null)
                    {
                        return snapresult;
                    }
                }
            }

            if (geometry is IPoint)
            {
                return new SnapResult(geometry.Coordinates[0], null, NewFeatureLayer, geometry, 0, 0)
                {
                    Rule = this
                };
            }

            return null;
        }

        private SnapResult LineStringSnapStartEnd(ref double minDistance, ILineString lineString, ICoordinate worldPos)
        {
            var c1 = lineString.Coordinates[0];
            ICoordinate location;
            int snapIndexPrevious;
            int snapIndexNext;

            var distance = GeometryHelper.Distance(c1.X, c1.Y, worldPos.X, worldPos.Y);

            SnapResult snapResult = null;
            if (distance < minDistance)
            {
                location = c1;
                snapIndexPrevious = 0;
                snapIndexNext = 0;
                minDistance = distance;
                snapResult = new SnapResult(location, null, null, lineString, snapIndexPrevious, snapIndexNext)
                {
                    Rule = this
                };
            }

            var c2 = lineString.Coordinates[lineString.Coordinates.Length - 1];
            distance = GeometryHelper.Distance(c2.X, c2.Y, worldPos.X, worldPos.Y);

            if (distance >= minDistance)
            {
                return snapResult;
            }

            location = c2;
            snapIndexPrevious = lineString.Coordinates.Length - 1;
            snapIndexNext = lineString.Coordinates.Length - 1;
            return new SnapResult(location, null, null, lineString, snapIndexPrevious, snapIndexNext)
            {
                Rule = this
            };
        }

        private SnapResult LineStringSnapEnd(ILineString lineString)
        {
            return new SnapResult(lineString.Coordinates[lineString.Coordinates.Length - 1], null, null, lineString,
                                  lineString.Coordinates.Length - 1, lineString.Coordinates.Length - 1)
            {
                Rule = this
            };
        }

        private SnapResult LineStringSnapStart(ILineString lineString)
        {
            return new SnapResult(lineString.Coordinates[0], null, null, lineString, 0, 0)
            {
                Rule = this
            };
        }

        private SnapResult LineStringSnapAllTrackers(ref double minDistance, ILineString lineString, ICoordinate worldPos)
        {
            return GeometrySnapAllTrackers(ref minDistance, lineString, worldPos);
        }

        private SnapResult GeometrySnapAllTrackers(ref double minDistance, IGeometry geometry, ICoordinate worldPos)
        {
            SnapResult snapResult = null;

            var coordinates = geometry.Coordinates;
            for (int i = 0; i < coordinates.Length; i++)
            {
                var c1 = coordinates[i];
                var distance = GeometryHelper.Distance(c1.X, c1.Y, worldPos.X, worldPos.Y);

                if (distance >= minDistance)
                {
                    continue;
                }

                minDistance = distance;
                snapResult = new SnapResult(coordinates[i], null, null, geometry, i, i)
                {
                    Rule = this
                };
            }

            return snapResult;
        }

        private SnapResult LineStringSnapFreeAtObject(ref double minDistance, IFeature feature, ILineString lineString, ICoordinate worldPos)
        {
            int vertexIndex;
            var nearestPoint = GeometryHelper.GetNearestPointAtLine(lineString, worldPos, minDistance, out vertexIndex);

            if (nearestPoint == null)
            {
                return null;
            }

            minDistance = GeometryHelper.Distance(nearestPoint.X, nearestPoint.Y, worldPos.X, worldPos.Y);
            return new SnapResult(nearestPoint, feature, null, lineString, vertexIndex - 1, vertexIndex)
            {
                Rule = this
            };
        }

        private SnapResult LineStringSnapFree(ref double minDistance, ILineString lineString, ICoordinate worldPos)
        {
            SnapResult snapResult = null;

            for (int i = 1; i < lineString.Coordinates.Length; i++)
            {
                var c1 = lineString.Coordinates[i - 1];
                var c2 = lineString.Coordinates[i];
                var distance = GeometryHelper.LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, worldPos.X, worldPos.Y);

                if (distance >= minDistance)
                {
                    continue;
                }

                minDistance = distance;
                snapResult = new SnapResult(GeometryFactory.CreateCoordinate(worldPos.X, worldPos.Y), null, null, lineString, i - 1, i)
                {
                    Rule = this
                };
            }

            return snapResult;
        }

        private SnapResult PolygonSnapFreeAtObject(ref double minDistance, IPolygon polygon, ICoordinate worldPos)
        {
            SnapResult snapResult = null;

            for (int i = 1; i < polygon.Coordinates.Length; i++)
            {
                var c1 = polygon.Coordinates[i - 1];
                var c2 = polygon.Coordinates[i];
                double distance = GeometryHelper.LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, worldPos.X, worldPos.Y);
                if (distance >= minDistance)
                {
                    continue;
                }

                minDistance = distance;

                var min_c1 = polygon.Coordinates[i - 1];
                var min_c2 = polygon.Coordinates[i];

                snapResult = new SnapResult(GeometryHelper.NearestPointAtSegment(min_c1.X, min_c1.Y, min_c2.X, min_c2.Y, worldPos.X,
                                                                                 worldPos.Y), null, null, polygon, i - 1, i)
                {
                    Rule = this
                };
            }

            return snapResult;
        }

        private SnapResult PolygonSnapFree(ref double minDistance, IPolygon polygon, ICoordinate worldPos)
        {
            SnapResult snapResult = null;

            for (int i = 1; i < polygon.Coordinates.Length; i++)
            {
                var c1 = polygon.Coordinates[i - 1];
                var c2 = polygon.Coordinates[i];

                var distance = GeometryHelper.LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, worldPos.X, worldPos.Y);
                if (distance >= minDistance)
                {
                    continue;
                }

                minDistance = distance;
                snapResult = new SnapResult(GeometryFactory.CreateCoordinate(worldPos.X, worldPos.Y), null, null, polygon, i - 1, i)
                {
                    Rule = this
                };
            }

            return snapResult;
        }
    }
}