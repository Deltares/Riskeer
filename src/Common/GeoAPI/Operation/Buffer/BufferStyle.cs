namespace GeoAPI.Operations.Buffer
{
    /// <summary>
    /// Buffer style.
    /// </summary>
    public enum BufferStyle
    {
        /// <summary> 
        /// Specifies a round line buffer end cap endCapStyle (Default).
        /// </summary>/
        CapRound = 1,

        /// <summary> 
        /// Specifies a butt (or flat) line buffer end cap endCapStyle.
        /// </summary>
        CapButt = 2,

        /// <summary>
        /// Specifies a square line buffer end cap endCapStyle.
        /// </summary>
        CapSquare = 3,
    }
}
