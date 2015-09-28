// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// SharpMap is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with SharpMap; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA 

using System;
using System.Drawing;
using GeoAPI.Geometries;
using SharpMap.Api;

namespace SharpMap.Utilities
{
	/// <summary>
	/// Class for transforming between world and image coordinate
	/// </summary>
	public class Transform
	{
		/// <summary>
		/// Transforms from world coordinate system (WCS) to image coordinates
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in WCS</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in image coordinates</returns>
		public static System.Drawing.Point WorldtoMap(ICoordinate p, IMap map)
		{
			//if (map.MapTransform != null && !map.MapTransform.IsIdentity)
			//	map.MapTransform.TransformPoints(new System.Drawing.PointF[] { p });
            return new Point((int)((p.X - map.WorldLeft) / map.PixelWidth), (int)((map.WorldTop - p.Y) / map.PixelHeight));
		}

		/// <summary>
		/// Transforms from image coordinates to world coordinate system (WCS).
		/// NOTE: This method DOES NOT take the MapTransform property into account (use SharpMap.Map.MapToWorld instead)
		/// </summary>
		/// <param name="p">Point in image coordinate system</param>
		/// <param name="map">Map reference</param>
		/// <returns>Point in WCS</returns>
		public static ICoordinate MapToWorld(System.Drawing.PointF p, SharpMap.Map map)
		{
            if (map.PixelHeight == double.PositiveInfinity)
                throw new ArgumentException("Can not convert map coordinates to world coordinates, pixelwidth (mapsize) not set.", "map");
            //if (this.MapTransform != null && !this.MapTransform.IsIdentity)
            //{
            //    System.Drawing.PointF[] p2 = new System.Drawing.PointF[] { p };
            //    this.MapTransform.TransformPoints(new System.Drawing.PointF[] { p });
            //    this.MapTransformInverted.TransformPoints(p2);
            //    return Utilities.Transform.MapToWorld(p2[0], this);
            //}
            //else 
			IEnvelope env = map.Envelope;

            return SharpMap.Converters.Geometries.GeometryFactory.CreateCoordinate(env.MinX + p.X * map.PixelWidth,
                    env.MaxY - p.Y * map.PixelHeight);
            
		}
		
		
		#region Transform Methods
		/// <summary>
		/// Transforms the point to image coordinates, based on the map
		/// </summary>
		/// <param name="map">Map to base coordinates on</param>
		/// <returns>point in image coordinates</returns>
		public static System.Drawing.PointF TransformToImage(IPoint p, Map map)
		{
			return SharpMap.Utilities.Transform.WorldtoMap(p.Coordinate, map);
		}

        public static Point[] TransformToImageI(ILineString line, Map map, bool simplifyGeometry, ref int pointCount)
        {
            var length = line.Coordinates.Length;
            var points = new Point[length];

            points[0] = WorldtoMap(line.Coordinates[0], map);
            Point pt = points[0];
            Point pt2 = pt;
            pointCount = 1;
            int i = 1;
            for (; i < length - 1; i++)
            {
                pt = WorldtoMap(line.Coordinates[i], map);

                if (!simplifyGeometry || Math.Abs(pt2.X - pt.X) > 0 || Math.Abs(pt2.Y - pt.Y) > 0)
                {
                    points[pointCount] = pt;
                    pointCount++;

                    pt2 = pt;
                }
            }

            if (length > 1)
                points[pointCount++] = WorldtoMap(line.Coordinates[i], map);

            return points;
        }

        public static PointF[] TransformToImage(ILineString line, IMap map, bool simplifyGeometry = false)
        {
            var length = line.Coordinates.Length;
            var points = new PointF[length];

            points[0] = WorldtoMap(line.Coordinates[0], map);
            PointF pt = points[0];
            PointF pt2 = pt;
            int pointCount = 1;
            int i = 1;
            for (; i < length - 2; i++)
            {
                pt = WorldtoMap(line.Coordinates[i], map);

                if (!simplifyGeometry || Math.Abs(pt2.X - pt.X) > 1 || Math.Abs(pt2.Y - pt.Y) > 1)
                {
                    points[pointCount] = pt;
                    pointCount++;

                    pt2 = pt;
                }
            }

            // always add last 2 points
            if(length > 1)
                points[pointCount++] = WorldtoMap(line.Coordinates[i++], map);

            if (length > 2)
                points[pointCount++] = WorldtoMap(line.Coordinates[i], map);

            var result = new PointF[pointCount];
            Array.Copy(points, result, pointCount);

            return result;
        }	

		/// Transforms the polygon to image coordinates, based on the map
		/// </summary>
		/// <param name="map">Map to base coordinates on</param>
		/// <returns>Polygon in image coordinates</returns>
		public static System.Drawing.PointF[] TransformToImage(IPolygon poly, SharpMap.Map map)
		{
			int vertices = poly.Shell.Coordinates.Length;
			for (int i = 0; i < poly.Holes.Length;i++)
				vertices += poly.Holes[i].Coordinates.Length;

			System.Drawing.PointF[] v = new System.Drawing.PointF[vertices];
			for (int i = 0; i < poly.Shell.Coordinates.Length; i++)
				v[i] = SharpMap.Utilities.Transform.WorldtoMap(poly.Shell.Coordinates[i], map);
			int j = poly.Shell.Coordinates.Length;
			for (int k = 0; k < poly.Holes.Length;k++)
			{
				for (int i = 0; i < poly.Holes[k].Coordinates.Length; i++)
					v[j + i] = SharpMap.Utilities.Transform.WorldtoMap(poly.Holes[k].Coordinates[i], map);
				j += poly.Holes[k].Coordinates.Length;
			}
			return v;
		}

		
		#endregion
		
	}
}
