using System;
using DelftTools.Utils.Data;
using GeoAPI.Geometries;

namespace GeoAPI.Extensions.Feature
{
    public interface IFeature : IUnique<long>, ICloneable
    {
        IGeometry Geometry { get; set; }

        IFeatureAttributeCollection Attributes { get; set; }
    }
}