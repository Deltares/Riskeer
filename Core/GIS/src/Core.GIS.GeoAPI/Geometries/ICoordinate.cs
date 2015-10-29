using System;

namespace Core.Gis.GeoApi.Geometries
{
    public interface ICoordinate : ICloneable, IComparable, IComparable<ICoordinate>, IEquatable<ICoordinate>
    {
        double X { get; set; }

        double Y { get; set; }

        double Z { get; set; }

        ICoordinate CoordinateValue { set; }

        double Distance(ICoordinate p);

        bool Equals2D(ICoordinate other);
    }
}