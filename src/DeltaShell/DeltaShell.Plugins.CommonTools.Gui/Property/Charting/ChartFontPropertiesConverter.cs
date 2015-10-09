using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace DeltaShell.Plugins.CommonTools.Gui.Property.Charting
{
    public class ChartFontPropertiesConverter : FontConverter
    {
        private static readonly string[] AllowedProperties = new[]
        {
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
                    properties.OfType<PropertyDescriptor>().Where(p => AllowedProperties.Contains(p.Name)).ToArray());
        }
    }
}