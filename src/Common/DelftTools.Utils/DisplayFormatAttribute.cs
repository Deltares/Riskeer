using System;

namespace DelftTools.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DisplayFormatAttribute : Attribute
    {
        public DisplayFormatAttribute(string formatString)
        {
            this.FormatString = formatString;
        }

        public string FormatString { get; private set; }
    }
}