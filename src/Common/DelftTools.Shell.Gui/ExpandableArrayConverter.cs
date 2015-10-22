using System;
using System.ComponentModel;
using System.Globalization;

using DelftTools.Shell.Gui.Properties;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// <see cref="ArrayConverter"/> with modified conversion to string.
    /// </summary>
    public class ExpandableArrayConverter : ArrayConverter
    {
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is Array)
            {
                return string.Format(Resources.ExpandableArrayConverter_ConvertTo_Aantal_0_, ((Array) value).Length);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}