using System;

namespace DelftTools.Controls.Swf.Table
{
    /// <summary>
    /// Wrapper for ICustomFormatter, so that IFormatProvider won't have to be implemented.
    /// http://documentation.devexpress.com/#WindowsForms/CustomDocument3045
    /// </summary>
    internal class TableViewCellFormatterProvider : IFormatProvider
    {
        private readonly ICustomFormatter formatter;

        public TableViewCellFormatterProvider(ICustomFormatter formatter)
        {
            this.formatter = formatter;
        }

        /// <summary>
        /// Returns an object that provides formatting services for the specified type.
        /// </summary>
        /// <returns>
        /// An instance of the object specified by <paramref name="formatType"/>, 
        /// if the <see cref="T:System.IFormatProvider"/> implementation can supply that type of object; otherwise, null.
        /// </returns>
        /// <param name="formatType">An object that specifies the type of format object to return. </param>
        /// <filterpriority>1</filterpriority>
        public object GetFormat(Type formatType)
        {
            return formatter;
        }
    }
}