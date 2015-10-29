using System;

namespace Core.Common.Utils
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