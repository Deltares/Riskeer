using System.Data.Entity.Infrastructure;
using System.Data.SQLite;
using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Utils;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an empty or new SQLite database file.
    /// </summary>
    public class RingtoetsDatabaseCreator : StorageToFile
    {
        /// <summary>
        /// Creates a new instance of <see cref="RingtoetsDatabaseCreator"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        public RingtoetsDatabaseCreator(string databaseFilePath)
            : base(databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                SQLiteConnection.CreateFile(databaseFilePath);
            }
            ConnectionString = SqLiteStorageConnection.BuildSqLiteConnectionString(databaseFilePath);
        }

        /// <summary>
        /// Creates the basic database structure for a Ringtoets database file.
        /// </summary>
        /// <exception cref="DbUpdateException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        public void CreateDatabaseStructure()
        {
            using (var dbContext = new SQLiteConnection(ConnectionString))
            {
                using (var command = dbContext.CreateCommand())
                {
                    dbContext.Open();
                    command.CommandText = Resources.DatabaseStructure;
                    try
                    {
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException exception)
                    {
                        throw CreateUpdateDatabaseException(Resources.Error_Write_Structure_to_Database, exception);
                    }
                    dbContext.Close();
                }
            }
        }
    }
}