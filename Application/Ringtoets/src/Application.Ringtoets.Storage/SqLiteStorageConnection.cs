using System.Data.Entity.Core.EntityClient;
using System.Data.SQLite;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class builds a connection string to an SQLite database file.
    /// </summary>
    public static class SqLiteStorageConnection
    {
        /// <summary>
        /// Constructs a connection string to connect the Entity Framework to <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns>A new connection string.</returns>
        public static string BuildSqLiteEntityConnectionString(string filePath)
        {
            return new EntityConnectionStringBuilder
            {
                Metadata = string.Format(@"res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl", "DbContext.RingtoetsEntities"),
                Provider = @"System.Data.SQLite.EF6",
                ProviderConnectionString = new SQLiteConnectionStringBuilder()
                {
                    FailIfMissing = true,
                    DataSource = filePath,
                    ReadOnly = false,
                    ForeignKeys = true,
                    Version = 3,
                    Pooling = true
                }.ConnectionString
            }.ConnectionString;
        }

        /// <summary>
        /// Constructs a connection string to connect to <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns>A new connection string.</returns>
        public static string BuildSqLiteConnectionString(string filePath)
        {
            return new SQLiteConnectionStringBuilder()
            {
                FailIfMissing = true,
                DataSource = filePath,
                ReadOnly = false,
                ForeignKeys = true,
                Version = 3
            }.ConnectionString;
        }
    }
}