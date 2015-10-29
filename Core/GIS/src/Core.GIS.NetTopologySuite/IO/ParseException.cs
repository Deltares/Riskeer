using System;

namespace Core.GIS.NetTopologySuite.IO
{
    /// <summary>  
    /// Thrown by a <c>WKTReader</c> when a parsing problem occurs.
    /// </summary>
    public class ParseException : ApplicationException
    {
        /// <summary>
        /// Creates a <c>ParseException</c> with the given detail message.
        /// </summary>
        /// <param name="message">A description of this <c>ParseException</c>.</param>
        public ParseException(String message) : base(message) {}
    }
}