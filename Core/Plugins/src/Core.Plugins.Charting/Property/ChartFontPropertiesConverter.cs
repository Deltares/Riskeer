using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace Core.Plugins.Charting.Property
{
    public class ChartFontPropertiesConverter : FontConverter
    {
        private static readonly string[] AllowedProperties = {
            "Size",
            "Bold",
            "Italic",
            "Underline",
            "Strikeout"
        };

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value,
                                                                   Attribute[] attributes)
        {
            var properties = base.GetProperties(context, value, attributes);

            return
                new PropertyDescriptorCollection(
                    Enumerable.OfType<PropertyDescriptor>(properties).Where(p => AllowedProperties.Contains(p.Name)).ToArray());
        }
    }
}