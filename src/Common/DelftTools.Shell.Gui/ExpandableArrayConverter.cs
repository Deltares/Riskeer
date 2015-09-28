using System;
using System.ComponentModel;
using System.Globalization;

namespace DelftTools.Shell.Gui
{
    public class ExpandableArrayConverter : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Array)
            {
                return string.Format("Count ({0})", ((Array)value).Length);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}