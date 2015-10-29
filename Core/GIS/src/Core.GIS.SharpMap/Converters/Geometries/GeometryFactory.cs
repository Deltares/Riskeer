/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Christian
 * Datum: 20.11.2007
 * Zeit: 21:38
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Algorithm;
using Core.GIS.NetTopologySuite.Geometries;

namespace Core.GIS.SharpMap.Converters.Geometries
{
    /// <summary>
    /// Description of GeometryFactory.
    /// </summary>
    // TODO: remove this, inject it using NTS, DON'T USE IT DIRECTLY
    [Obsolete]
    public class GeometryFactory
    {
        private static readonly NetTopologySuite.Geometries.GeometryFactory geomFactory = new NetTopologySuite.Geometries.GeometryFactory();

        public static ICoordinate CreateCoordinate(double x, double y)
        {
            // use 0.0 as default for z
            return new Coordinate(x, y, 0.0);
        }

        public static IPoint CreatePoint(double x, double y)
        {
            return geomFactory.CreatePoint(new Coordinate(x, y, 0.0));
        }

        public static IPoint CreatePoint(ICoordinate coord)
        {
            return geomFactory.CreatePoint(coord);
        }

        public static IMultiPoint CreateMultiPoint(IPoint[] points)
        {
            return geomFactory.CreateMultiPoint(points);
        }

        public static IEnvelope CreateEnvelope(double minx, double maxx, double miny, double maxy)
        {
            return new Envelope(minx, maxx, miny, maxy);
        }

        public static IEnvelope CreateEnvelope()
        {
            return new Envelope();
        }

        public static ILineString CreateLineString(ICoordinate[] coords)
        {
            return geomFactory.CreateLineString(coords);
        }

        public static IMultiLineString CreateMultiLineString(ILineString[] lineStrings)
        {
            return geomFactory.CreateMultiLineString(lineStrings);
        }

        public static ILinearRing CreateLinearRing(ICoordinate[] coords)
        {
            return geomFactory.CreateLinearRing(coords);
        }

        public static IPolygon CreatePolygon(ILinearRing shell, ILinearRing[] holes)
        {
            return geomFactory.CreatePolygon(shell, holes);
        }

        public static IMultiPolygon CreateMultiPolygon(IPolygon[] polygons)
        {
            return geomFactory.CreateMultiPolygon(polygons);
        }

        public static IMultiPolygon CreateMultiPolygon()
        {
            return geomFactory.CreateMultiPolygon(null);
        }

        public static IGeometryCollection CreateGeometryCollection(IGeometry[] geometries)
        {
            return geomFactory.CreateGeometryCollection(geometries);
        }

        public static IGeometryCollection CreateGeometryCollection()
        {
            return geomFactory.CreateGeometryCollection(null);
        }

        public static bool IsCCW(ICoordinate[] ring)
        {
            return CGAlgorithms.IsCCW(ring);
        }
    }
}