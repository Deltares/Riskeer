using System;
using System.ComponentModel;
using System.Globalization;
using Core.GIS.SharpMap.UI.Properties;

namespace Core.GIS.SharpMap.UI.Forms
{
    public class CoordinateSystemStringTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            return null;
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            return value != null ? value.ToString() : Resources.CoordinateSystemStringTypeConverter_ConvertTo_empty;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return true;
        }
    }
}