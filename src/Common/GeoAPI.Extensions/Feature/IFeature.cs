using System;
using GeoAPI.Geometries;

namespace GeoAPI.Extensions.Feature
{
    public interface IFeature : ICloneable
    {
        IGeometry Geometry { get; set; }

        IFeatureAttributeCollection Attributes { get; set; }
    }
}