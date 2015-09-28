using System;

namespace GeoAPI.Extensions.Feature
{
    /// <summary>
    /// Attribute to be used for properties which need to be declared as Feature Attributes.
    /// </summary>
    public class FeatureAttributeAttribute: Attribute
    {
        // default value makes the undefined properties appear at the end
        private int order = 9999;

        public int Order
        {
            get { return order; }
            set { order = value; }
        }

        private string exportName = null;

        // used for export/import to/from shapefiles.
        public string ExportName
        {
            get { return exportName; }
            set { exportName = value; }
        }
    }
}
