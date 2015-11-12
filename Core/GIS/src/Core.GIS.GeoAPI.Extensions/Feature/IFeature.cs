using System;
using Core.GIS.GeoAPI.Geometries;

namespace Core.GIS.GeoAPI.Extensions.Feature
{
    public interface IFeature : ICloneable
    {
        /// <summary>
        /// The Name of the feature.
        /// </summary>
        string Name { get; set; }

        IGeometry Geometry { get; set; }

        IFeatureAttributeCollection Attributes { get; set; }
    }
}