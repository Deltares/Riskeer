using System;
using System.Collections.Generic;
using System.Linq;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Operation.Overlay;

namespace NetTopologySuite.Extensions.Geometries
{
    public static class GeometryHelper
    {
        // TODO: check if we can use IGeometry methods here (OGC)

        /// <summary>
        /// The SharpMap implementation of Collection<IGeometry>.IndexOf(IGeometry) will use  
        /// bool Geometry::Equals(IGeometry g) which returns true if geometries are of equal shape.
        /// In many cases this is not desired behaviour. For example if we are looking for a BranchSegmentBoundary
        /// it is possible the Node at the same location is returned.
        /// IndexOfGeometry only compares the geometry references
        /// 
        /// NOTE: performance tips: 
        /// If possible minimize calls to 
        ///  - IGeometry.Coordinates converts an internal ICoordinateSequence to an ICoordinate array
        ///    not expensive as such but may be called many times: 
        ///  - ILineString.Length is expensive; move outsize loops as much as possible
        /// </summary>
        /// <param name="geometries"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        static public int IndexOfGeometry(IList<IGeometry> geometries, IGeometry geometry)
        {
            for (int i = 0; i < geometries.Count; i++)
            {
                if (ReferenceEquals(geometries[i], geometry))
                    return i;
            }
            return -1;
        }

        /// <summary>
        /// Return the coordinate of the point nearest worldPos at lineString.
        /// TODO: Is it distance along the polyline????!!! NAME IT CORRECTLY
        /// TODO: can we merge it with the next method?
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="worldPos"></param>
        /// <returns></returns>
        public static double Distance(ILineString lineString, ICoordinate worldPos)
        {
            // Distance along linestring
            double minDistance = Double.MaxValue;
            if (lineString == null || worldPos == null) return minDistance;

            ICoordinate min_c1 = null;
            ICoordinate min_c2 = null;

            int index = -1;
            ICoordinate c1;
            ICoordinate c2;
            double pointDistance = 0;

            ICoordinate[] coordinates = lineString.Coordinates;
            for (int i = 1; i < coordinates.Length; i++)
            {
                c1 = coordinates[i - 1];
                c2 = coordinates[i];
                double distance = LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, worldPos.X, worldPos.Y);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    min_c1 = c1;
                    min_c2 = c2;
                    index = i;
                }
            }
            if (-1 != index)
            {
                ICoordinate location = NearestPointAtSegment(min_c1.X, min_c1.Y,
                                                             min_c2.X, min_c2.Y, worldPos.X, worldPos.Y);
                for (int i = 1; i < index; i++)
                {
                    c1 = coordinates[i - 1];
                    c2 = coordinates[i];
                    pointDistance += Distance(c1.X, c1.Y, c2.X, c2.Y);
                }
                pointDistance += Distance(min_c1.X, min_c1.Y, location.X, location.Y);
            }
            return pointDistance;
        }

        /// <summary>
        /// Returns the minimum distance of <paramref name="geometry"/> to <paramref name="lineString"/>.
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="geometry"> </param>
        /// <returns></returns>
        static public double Distance(ILineString lineString, IGeometry geometry)
        {
            double minDistance = Double.MaxValue;
            if (lineString == null) return minDistance;
            
            ICoordinate c1;
            ICoordinate c2;
            ICoordinate[] coordinates = lineString.Coordinates;

            if (geometry is IPoint)
            {
                var point = (IPoint)geometry;
                for (int i = 1; i < coordinates.Length; i++)
                {
                    c1 = coordinates[i - 1];
                    c2 = coordinates[i];
                    double distance = LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, point.X, point.Y);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                }
            }
            else if (geometry is ILineString)
            {
                return LineStringFirstIntersectionOffset(lineString, (ILineString)geometry);
            }
            else if(geometry != null)
            {
                return lineString.Distance(geometry);

/* TODO: check if above code doesn't work good for line string - use trickery trick below 
                // trickery trick someone changed cross section geometry from point to linestring
                // non geometry based cross sections are represented using a 2 point linestring.
                // Use the center of this linestring 
                IPoint crossSectionCenter = GeometryFactory.CreatePoint(CrossSectionHelper.CrossSectionCoordinate(crossSection));
                double distance = GeometryHelper.Distance((ILineString)branch.Geometry, crossSectionCenter);

                float limit;

                if (map != null)
                {
                    limit = (float)MapControlHelper.ImageToWorld(map, 1);
                }
                else
                {
                    limit = (float)(0.1 * Math.Max(branch.Geometry.EnvelopeInternal.Width, branch.Geometry.EnvelopeInternal.Height));
                }


                if (distance < limit)
                {
                    crossSection.Branch = branch;
                    CalculateCrossSectionOffset(crossSection);
                    CrossSectionHelper.UpdateDefaultGeometry(crossSection, crossSection.Geometry.Length / 2);
                }
*/

            }

            return minDistance;
        }

        //http://www.topcoder.com/tc?module=Static&d1=tutorials&d2=geometry1
        //Line-Point Distance = (AB x AC)/|AB|.
        //    //Compute the distance from A to B
        //    double distance(int[] A, int[] B){
        //        int d1 = A[0] - B[0];
        //        int d2 = A[1] - B[1];
        //        return sqrt(d1*d1+d2*d2);
        //    }

        static public double Distance(double x1, double y1, double X2, double Y2)
        {
            return Math.Sqrt((x1 - X2)*(x1 - X2) + (y1 - Y2)*(y1 - Y2));
        }

        //http://www.topcoder.com/tc?module=Static&d1=tutorials&d2=geometry1
        //Line-Point Distance = (AB x AC)/|AB|.
        //   //Compute the cross product AB x AC
        //    int cross(int[] A, int[] B, int[] C){
        //        AB = new int[2];
        //        AC = new int[2];
        //        AB[0] = B[0]-A[0];
        //        AB[1] = B[1]-A[1];
        //        AC[0] = C[0]-A[0];
        //        AC[1] = C[1]-A[1];
        //        int cross = AB[0] * AC[1] - AB[1] * AC[0];
        //        return cross;
        //    }

        static public double CrossProduct(double Ax, double Ay, double Bx, double By, 
                                          double cx , double cy )
        {
            return (Bx - Ax)*(cy - Ay) - (By - Ay)*(cx - Ax);
        }

        //http://www.topcoder.com/tc?module=Static&d1=tutorials&d2=geometry1
        //Line-Point Distance = (AB x AC)/|AB|.
        ////Compute the dot product AB · BC
        //    int dot(int[] A, int[] B, int[] C){
        //        AB = new int[2];
        //        BC = new int[2];
        //        AB[0] = B[0]-A[0];
        //        AB[1] = B[1]-A[1];
        //        BC[0] = C[0]-B[0];
        //        BC[1] = C[1]-B[1];
        //        int dot = AB[0] * BC[0] + AB[1] * BC[1];
        //        return dot;
        //    }

        static public double Dot(double Ax, double Ay, double Bx, double By, 
                                 double cx, double cy)
        {
            return (Bx - Ax)*(cx - Bx) + (By - Ay)*(cy - By);
        }

        //http://www.topcoder.com/tc?module=Static&d1=tutorials&d2=geometry1
        //Line-Point Distance = (AB x AC)/|AB|.
        ////Compute the distance from AB to C
        //    //if isSegment is true, AB is a segment, not a line.
        //    double linePointDist(int[] A, int[] B, int[] C, boolean isSegment){
        //        double dist = cross(A,B,C) / distance(A,B);
        //        if(isSegment){
        //            int dot1 = dot(A,B,C);
        //            if(dot1 > 0)return distance(B,C);
        //            int dot2 = dot(B,A,C);
        //            if(dot2 > 0)return distance(A,C);
        //        }
        //        return abs(dist);
        //    }

        static public double LinePointDistance(double Ax, double Ay, double Bx, double By, 
                                               double cx, double cy)
        {
            double dist, dot1, dot2;

            dist = Distance(Ax, Ay, Bx, By);
            if (dist < 0.000001)
            {
                return Double.MaxValue;
            }
            dist = CrossProduct(Ax, Ay, Bx, By, cx, cy)/dist;
            // if (isSegment) always true
            dot1 = Dot(Ax, Ay, Bx, By, cx, cy);
            if (dot1 > 0)
                return Distance(Bx, By, cx, cy);
            dot2 = Dot(Bx, By, Ax, Ay, cx, cy);
            if (dot2 > 0)
                return Distance(Ax, Ay, cx, cy);
            return Math.Abs(dist);
        }

        static public ICoordinate NearestPointAtSegment(double Ax, double Ay, double Bx, double By, 
                                                        double cx, double cy)
        {
            // if (AB . BC) > 0) 
            if (Dot(Ax, Ay, Bx, By, cx, cy) > 0)
            {
                return GeometryFactory.CreateCoordinate(Bx, By);
            }
                // else if ((BA . AC) > 0)
            else if (Dot(Bx, By, Ax, Ay, cx, cy) > 0)
            {
                return GeometryFactory.CreateCoordinate(Ax, Ay);
            }
            else
                // both dot products < 0 -> point between A and B
            {
                double AC = Distance(Ax, Ay, cx, cy);
                double BC = Distance(Bx, By, cx, cy);
                return GeometryFactory.CreateCoordinate(Ax + ((AC) / (AC + BC))*(Bx-Ax),
                                                        Ay + ((AC) / (AC + BC)) * (By - Ay));
            }
        }

        public static IGeometry SetCoordinate(IGeometry geometry, int coordinateIndex, ICoordinate coordinate)
        {
            var newGeometry = (IGeometry)geometry.Clone();
            newGeometry.Coordinates[coordinateIndex].X = coordinate.X;
            newGeometry.Coordinates[coordinateIndex].Y = coordinate.Y;
            newGeometry.GeometryChangedAction();
            return newGeometry;

        }

        public static void MoveCoordinate(IGeometry geometry, int coordinateIndex, double deltaX, double deltaY)
        {
            geometry.Coordinates[coordinateIndex].X += deltaX;
            geometry.Coordinates[coordinateIndex].Y += deltaY;
            geometry.GeometryChangedAction();
        }

        public static void MoveCoordinate(IGeometry targetGeometry, IGeometry sourceGeometry, int coordinateIndex, double deltaX, double deltaY)
        {
            MoveCoordinate(targetGeometry.Coordinates, sourceGeometry.Coordinates, coordinateIndex, deltaX, deltaY);
            targetGeometry.GeometryChangedAction();
        }

        public static void MoveCoordinate(ICoordinate[] targetCoordinates, ICoordinate[] sourceCoordinates, int coordinateIndex, double deltaX, double deltaY)
        {
            targetCoordinates[coordinateIndex].X = sourceCoordinates[coordinateIndex].X + deltaX;
            targetCoordinates[coordinateIndex].Y = sourceCoordinates[coordinateIndex].Y + deltaY;
        }

        /// <summary>
        /// Returns the coordinate at an offset of the lineString
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        /// <remarks>
        /// Be aware that this function can suffer from double precision issue, like: 100.0*0.55 evaluates to 55.000000000000007
        /// </remarks>
        public static ICoordinate LineStringCoordinate(ILineString lineString, double distance)
        {
            double partialDistance = 0;

            ICoordinate[] coordinates = lineString.Coordinates;
            for (int i = 1; i < coordinates.Length; i++)
            {
                ICoordinate c1 = coordinates[i - 1];
                ICoordinate c2 = coordinates[i];
                double segmentDistance = Distance(c1.X, c1.Y, c2.X, c2.Y);
                if ((partialDistance + segmentDistance) > distance)
                {
                    double factor = (distance - partialDistance)/(segmentDistance);
                    return GeometryFactory.CreateCoordinate(
                        coordinates[i - 1].X + factor * (coordinates[i].X - coordinates[i - 1].X),
                        coordinates[i - 1].Y + factor * (coordinates[i].Y - coordinates[i - 1].Y));
                }
                partialDistance += segmentDistance;
            }
            return (ICoordinate) lineString.Coordinates[lineString.Coordinates.Length-1].Clone();
        }

        /// <summary>
        /// Returns the offset to the first intersection of lineString by cutLineString.
        /// </summary>
        /// <param name="lineString"></param>
        /// <param name="cutLineString"></param>
        /// <returns>offset to the first intersection, -1 if there is no intersection -1</returns>
        public static double LineStringFirstIntersectionOffset(ILineString lineString, ILineString cutLineString)
        {
            if(!lineString.Intersects(cutLineString))
            {
                return -1;
            }

            IGeometry intersection = lineString.Difference(cutLineString);

            if (intersection is IMultiLineString)
            {
                var result = (IMultiLineString) lineString.Difference(cutLineString);
                return result.Geometries[0].Length;
            }
            
            return intersection.Length;
        }

        public static ICoordinate GetNearestPointAtLine(ILineString lineString, ICoordinate coordinate, double tolerance, out int snapVertexIndex)
        {
            snapVertexIndex = -1; 
            ICoordinate nearestPoint = null;

            ICoordinate minC1;
            ICoordinate minC2;

            for (var i = 1; i < lineString.Coordinates.Length; i++)
            {
                var c1 = lineString.Coordinates[i - 1];
                var c2 = lineString.Coordinates[i];
                var distance = LinePointDistance(c1.X, c1.Y, c2.X, c2.Y, coordinate.X, coordinate.Y);

                if (distance >= tolerance)
                {
                    continue;
                }
                tolerance = distance;
                minC1 = c1;
                minC2 = c2;

                nearestPoint = NearestPointAtSegment(minC1.X, minC1.Y, minC2.X, minC2.Y, coordinate.X, coordinate.Y);

                snapVertexIndex = i;
            }

            return nearestPoint;
        }

        public static IFeature GetNearestFeature(ICoordinate coordinate, IEnumerable<IFeature> features, double tolerance)
        {
            var minDistance = tolerance;
            IFeature minDistanceFeature = null;

            var point = new Point(coordinate);

            foreach (var feature in features)
            {
                if(feature.Geometry == null) continue;

                var distance = point.Distance(feature.Geometry);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                    minDistanceFeature = feature;
                }
            }

            return minDistanceFeature;
        }

        public static IEnumerable<IFeature> GetFeaturesInRange(ICoordinate coordinate, IEnumerable<IFeature> features, double tolerance)
        {
            var minDistance = tolerance;
            var point = new Point(coordinate);

            return (from feature in features
                    where feature.Geometry != null // Distance method requires a defined Geometry
                    let distance = point.Distance(feature.Geometry)
                    where distance <= minDistance
                    select feature).ToList();
        }

        /// <summary>
        /// Splits the polygon at <para>splitPointX</para> and returns a left and right half
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="splitPointX"></param>
        /// <returns></returns>
        public static DelftTools.Utils.Tuple<IGeometry, IGeometry> SplitGeometryVerticalAt(IGeometry geometry, double splitPointX)
        {
            ThrowIfArgumentInvalid(geometry, splitPointX);

            var rightHalf = GetRightHalfGeometry(geometry, splitPointX);
            var leftHalf = GetLeftHalfGeometry(geometry, splitPointX);
            return new DelftTools.Utils.Tuple<IGeometry, IGeometry>(leftHalf, rightHalf);
        }

        private static void ThrowIfArgumentInvalid(IGeometry polygon, double splitPointX)
        {

            if (polygon.EnvelopeInternal.MinX > splitPointX || polygon.EnvelopeInternal.MaxX < splitPointX)
                throw new ArgumentOutOfRangeException("splitPointX",string.Format("Splitpoint at x {0:0.00} not within polygon. ",
                                                                    splitPointX));
    
        }

        private static IGeometry GetLeftHalfGeometry(IGeometry geometry, double splitPointX)
        {
            double minXValue = geometry.EnvelopeInternal.MinX - 1;
            double minYValue = geometry.EnvelopeInternal.MinY - 1;
            double maxYValue = geometry.EnvelopeInternal.MaxY + 1;
            
            var coordinatesLeft= new[]
                                       {
                                           new Coordinate(minXValue, maxYValue),
                                           new Coordinate(splitPointX, maxYValue),
                                           new Coordinate(splitPointX, minYValue),
                                           new Coordinate(minXValue, minYValue),
                                           new Coordinate(minXValue, maxYValue)
                                       };
            //
            var leftRectangle = new Polygon(new LinearRing(coordinatesLeft));
            return geometry.Intersection(leftRectangle);
        }

        private static IGeometry GetRightHalfGeometry(IGeometry geometry, double splitPointX)
        {
            double maxXValue = geometry.EnvelopeInternal.MaxX + 1;
            double minYValue = geometry.EnvelopeInternal.MinY - 1;
            double maxYValue = geometry.EnvelopeInternal.MaxY + 1;

            var coordinatesRight = new[]
                                       {
                                           new Coordinate(splitPointX, maxYValue),
                                           new Coordinate(splitPointX, minYValue),
                                           new Coordinate(maxXValue, minYValue),
                                           new Coordinate(maxXValue, maxYValue),
                                           new Coordinate(splitPointX, maxYValue)
                                       };
            //
            var leftRectangle = new Polygon(new LinearRing(coordinatesRight));
            return geometry.Intersection(leftRectangle);
        }


        public static IGeometry NormalizeGeometry(IGeometry geometryToNormalize)
        {
            if (geometryToNormalize is IPoint)
            {
                return geometryToNormalize;
            }
            
            if (geometryToNormalize is IGeometryCollection)
            {
                var geometries = (IGeometryCollection)geometryToNormalize.Clone();
                for (int i=0;i<geometries.Geometries.Length;i++)
                {
                    geometries.Geometries[i] = NormalizeGeometry(geometries.Geometries[i]);
                }
                return geometries;
            }

            var resultCoordinates = geometryToNormalize.Coordinates.ToList();


            var removed = true;
            while (removed)
            {
                removed = RemoveRedundantPoint(resultCoordinates);
            }

            if (geometryToNormalize is IPolygon)
            {
                //re-add the closing point as it was removed by removedundantpoint
                resultCoordinates.Add(resultCoordinates[0]);
                return new Polygon(new LinearRing(resultCoordinates.ToArray()));
            }
            return new LineString(resultCoordinates.ToArray());
        }

        private static bool RemoveRedundantPoint(List<ICoordinate> resultCoordinates)
        {
            var removed = false;
            int count = resultCoordinates.Count;
            for (int i = 0; i < count;i++ )
            {
                var nextPoint = resultCoordinates[(i + 1) %count];
                //make sure the index is always positive
                int previousPointIndex = ((i+count) - 1) % count;
                var previousPoint = resultCoordinates[previousPointIndex];
                var point = resultCoordinates[i];

                if ((PointIsSame(point, nextPoint)) ||
                    (PointIsOnLineBetweenPreviousAndNext(previousPoint, point, nextPoint)))
                {
                    resultCoordinates.RemoveAt(i%count);
                    removed = true;
                    break;
                }
            }
            return removed;
        }

        public static bool PointIsSame(ICoordinate point,ICoordinate previousPoint)
        {
            return point.X == previousPoint.X && point.Y == previousPoint.Y;
        }

        public static bool PointIsOnLineBetweenPreviousAndNext(ICoordinate previousPoint, ICoordinate point, ICoordinate nextPoint)
        {
            //vertical
            if (nextPoint.X == point.X && point.X == previousPoint.X)
                return true;
            //one part is vertical..so not redundant
            if (nextPoint.X == point.X || point.X == previousPoint.X)
                return false;

            //check the x is monotonous and the point.x if between the other points
            bool monotonous = (previousPoint.X < point.X && point.X < nextPoint.X )  || (previousPoint.X > point.X && point.X > nextPoint.X  );
            if (!monotonous) 
                return false;

            var nextRiCo = (nextPoint.Y - point.Y)/(nextPoint.X - point.X);
            var previousRiCo = (point.Y - previousPoint.Y)/(point.X - previousPoint.X);
            //the point slope is equal after and before the point
            return (Math.Abs(nextRiCo - previousRiCo) < 0.0001);
        }


        public static double GetIntersectionArea(IGeometry geometry, IGeometry other)
        {
            if (geometry is IMultiPolygon)
            {
                return IntersectionArea(geometry as IMultiPolygon, other);
            }
            return IntersectionArea(geometry, other);
        }

        private static double IntersectionArea(IMultiPolygon multiPolygon, IGeometry other)
        {
            var area = 0.0;
            foreach (var polygon in multiPolygon.Geometries.Cast<IPolygon>())
            {
                area += IntersectionArea(polygon, other);
            }
            return area;
        }

        private static double IntersectionArea(IGeometry geometry, IGeometry other)
        {
            if (geometry.Intersects(other.Envelope))
            {
                OverlayOp.NodingValidatorDisabled = true;
                double area;
                try
                {
                    area = geometry.Intersection(other).Area;
                }
                catch (Exception)
                {
                    area = GetSampledIntersectionArea(geometry, other);
                }
                OverlayOp.NodingValidatorDisabled = false;
                return area;
            }
            return 0.0;
        }

        public static double GetSampledIntersectionArea(IGeometry geometry, IGeometry other)
        {
            const int steps = 10;
            var area = 0.0;
            var envelopeInternal = geometry.EnvelopeInternal;
            var width = envelopeInternal.Width;
            var height = envelopeInternal.Height;
            var widthStep = width/steps;
            var heightStep = height/steps;
            var minX = envelopeInternal.MinX + widthStep/2.0;
            var minY = envelopeInternal.MinY + heightStep/2.0;
            var boxArea = width*height;
            var cellArea = boxArea/(steps*steps);

            for (var i = 0; i < steps; i++)
            {
                var x = minX + i*widthStep;
                for(var j = 0; j < steps; j++)
                {
                    var y = minY + j*heightStep;
                    if (geometry.Contains(new Point(x, y)) && other.Contains(new Point(x,y)))
                    {
                        area += cellArea;
                    }
                }
            }
            return area;
        }

        public static IGeometry InsertCurvePoint(IGeometry geometry, ICoordinate coordinate, int index)
        {
            var vertices = new List<ICoordinate>(geometry.Coordinates);
            vertices.Insert(index, coordinate);
            var geometryFactory = new GeometryFactory();

            if (geometry is ILineString)
            {
                return geometryFactory.CreateLineString(vertices.ToArray());
            }
            if (geometry is IPolygon)
            {
                return geometryFactory.CreatePolygon(geometryFactory.CreateLinearRing(vertices.ToArray()), null);
            }
            return geometry.Union(geometryFactory.CreatePoint(coordinate));
        }

        public static IGeometry RemoveCurvePoint(IGeometry geometry, int index, bool keepLineStringEndPoints=false)
        {
            var vertices = new List<ICoordinate>(geometry.Coordinates);
            vertices.RemoveAt(index);
            var geometryFactory = new GeometryFactory();
            var lastIndex = geometry.Coordinates.Length - 1;
            if (geometry is ILineString)
            {
                if (vertices.Count < 2)
                {
                    return null;
                }
                if (keepLineStringEndPoints && (index == 0 || index == lastIndex))
                {
                    return null;
                }
                return geometryFactory.CreateLineString(vertices.ToArray());
            }
            if (geometry is IPolygon)
            {
                // If first or last index is removed -> remove corresponding duplicate at the other end and close the ring.
                if (index == lastIndex)
                {
                    vertices[0] = vertices[lastIndex];
                }
                if (index == 0)
                {
                    vertices[lastIndex - 1] = vertices[0];
                }

                if (vertices.Count < 4)
                {
                    return null;
                }
                return geometryFactory.CreatePolygon(geometryFactory.CreateLinearRing(vertices.ToArray()), null);
            }
            if (index < geometry.Coordinates.Length)
            {
                var coordinate = geometry.Coordinates[index];
                var point = geometryFactory.CreatePoint(coordinate);
                return geometry.Difference(point);
            }
            return geometry;
        }
    }
}