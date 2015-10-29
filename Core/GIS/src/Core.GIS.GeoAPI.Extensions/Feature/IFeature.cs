using System;
using Core.Gis.GeoApi.Geometries;

namespace Core.Gis.GeoApi.Extensions.Feature
{
    public interface IFeature : ICloneable
    {
        IGeometry Geometry { get; set; }

        IFeatureAttributeCollection Attributes { get; set; }
    }
}