using System;
using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.GeoApi.Extensions.Feature
{
    public interface IFeature : ICloneable
    {
        IGeometry Geometry { get; set; }

        IFeatureAttributeCollection Attributes { get; set; }
    }
}