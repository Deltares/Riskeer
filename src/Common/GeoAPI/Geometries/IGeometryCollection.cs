using System.Collections;

namespace GeoAPI.Geometries
{
    public interface IGeometryCollection : IGeometry, IEnumerable
    {
        IGeometry this[int i] { get; }
        int Count { get; }

        IGeometry[] Geometries { get; }

        bool IsHomogeneous { get; }
    }
}