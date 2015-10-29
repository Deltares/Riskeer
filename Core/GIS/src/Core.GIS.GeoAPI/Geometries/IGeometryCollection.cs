using System.Collections;

namespace Core.GIS.GeoApi.Geometries
{
    public interface IGeometryCollection : IGeometry, IEnumerable
    {
        IGeometry this[int i] { get; }

        int Count { get; }

        IGeometry[] Geometries { get; }
    }
}