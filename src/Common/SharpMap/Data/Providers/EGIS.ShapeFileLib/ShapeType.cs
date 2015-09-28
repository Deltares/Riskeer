namespace SharpMap.Data.Providers.EGIS.ShapeFileLib
{
    /// <summary>
    /// Enumeration representing a ShapeType. Currently supported shape types are Point, PolyLine, Polygon and PolyLineM
    /// </summary>
    public enum ShapeType { NullShape = 0, Point = 1, PolyLine = 3, Polygon = 5, MultiPoint = 8, PointZ = 11, PolyLineZ = 13, PolygonZ = 15, MultiPointZ = 18, PointM = 21, PolyLineM = 23 }
}