using System.Collections;

namespace Core.GIS.GeoApi.Geometries
{
    /// <summary>
    /// 
    /// </summary>
    public interface IGeometryFactory
    {
        ICoordinateSequenceFactory CoordinateSequenceFactory { get; }

        int SRID { get; }
        IPrecisionModel PrecisionModel { get; }

        IGeometry BuildGeometry(ICollection geomList);

        IPoint CreatePoint(ICoordinate coordinate);
        IPoint CreatePoint(ICoordinateSequence coordinates);

        ILineString CreateLineString(ICoordinate[] coordinates);
        ILineString CreateLineString(ICoordinateSequence coordinates);

        ILinearRing CreateLinearRing(ICoordinate[] coordinates);
        ILinearRing CreateLinearRing(ICoordinateSequence coordinates);

        IPolygon CreatePolygon(ILinearRing shell, ILinearRing[] holes);

        IMultiPoint CreateMultiPoint(ICoordinate[] coordinates);
        IMultiPoint CreateMultiPoint(IPoint[] point);

        IMultiLineString CreateMultiLineString(ILineString[] lineStrings);

        IMultiPolygon CreateMultiPolygon(IPolygon[] polygons);

        IGeometryCollection CreateGeometryCollection(IGeometry[] geometries);

        IGeometry ToGeometry(IEnvelope envelopeInternal);
    }
}