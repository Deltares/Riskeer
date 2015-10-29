using System;

namespace Core.Gis.GeoApi.Extensions.Feature
{
    /// <summary>
    /// Attribute to be used for properties which need to be declared as Feature Attributes.
    /// </summary>
    public class FeatureAttributeAttribute : Attribute
    {
        // default value makes the undefined properties appear at the end
        private int order = 9999;

        public FeatureAttributeAttribute()
        {
            ExportName = null;
        }

        public int Order
        {
            get
            {
                return order;
            }
            set
            {
                order = value;
            }
        }

        // used for export/import to/from shapefiles.
        public string ExportName { get; set; }
    }
}