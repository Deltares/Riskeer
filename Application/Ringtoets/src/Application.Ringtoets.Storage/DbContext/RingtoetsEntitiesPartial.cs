namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Partial implementation of <see cref="RingtoetsEntities"/> that support a connection string and does not read the connection string from the configuration.
    /// </summary>
    public partial class RingtoetsEntities
    {
        /// <summary>
        /// A new instance of <see cref="RingtoetsEntities"/>.
        /// </summary>
        /// <param name="connString">A connection string.</param>
        public RingtoetsEntities(string connString)
            : base(connString) { }
    }
}