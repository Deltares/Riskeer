using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;

namespace NetTopologySuite.Extensions.Features
{
    public class Feature : Unique<long>, IFeature
    {
        private IGeometry geometry;

        private IFeatureAttributeCollection attributes;
        
        /// <summary>
        /// When changing the geometry of a feature make sure to call GeometryChangedAction
        /// </summary>
        public virtual IGeometry Geometry
        {
            get { return geometry; }
            set { geometry = value; }
        }

        public virtual IFeatureAttributeCollection Attributes
        {
            get { return attributes; }
            set { attributes = value; }
        }

        public virtual object Clone()
        {
            return new Feature
            {
                Geometry = Geometry,
                Attributes = Attributes == null ? null : Attributes.Clone() as IFeatureAttributeCollection
            };
        }

        public override string ToString()
        {
            return Id 
                + " "  
                + (Geometry != null ? Geometry.ToString() : "<no geometry>");
        }
    }
}
