using Core.GIS.GeoApi.Extensions.Feature;
using Core.GIS.GeoApi.Geometries;

namespace Core.GIS.NetTopologySuite.Extensions.Features
{
    public class Feature : IFeature
    {
        /// <summary>
        /// When changing the geometry of a feature make sure to call GeometryChangedAction
        /// </summary>
        public virtual IGeometry Geometry { get; set; }

        public virtual IFeatureAttributeCollection Attributes { get; set; }

        public override string ToString()
        {
            return Geometry != null ? Geometry.ToString() : "<no geometry>";
        }

        public virtual object Clone()
        {
            return new Feature
            {
                Geometry = Geometry,
                Attributes = Attributes == null ? null : Attributes.Clone() as IFeatureAttributeCollection
            };
        }
    }
}