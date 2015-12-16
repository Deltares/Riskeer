using System;

namespace Core.Common.Utils.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayFormatAttribute : Attribute
    {
        public DisplayFormatAttribute(string formatString)
        {
            FormatString = formatString;
        }

        public string FormatString { get; private set; }
    }
}