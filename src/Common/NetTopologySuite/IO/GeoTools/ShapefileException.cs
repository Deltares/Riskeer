using System;

namespace GisSharpBlog.NetTopologySuite.IO.GeoTools
{
    /// <summary>
    /// The exception that is thrown when a non-fatal application error occurs related to Topology functionality.
    /// </summary>
    internal class ShapefileException : ApplicationException
    {
        /// <summary>
        /// Initializes a new instance of the ShapefileException class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error. </param>
        public ShapefileException(string message) : base(message) {}
    }
}