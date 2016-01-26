using System.Data.SQLite;
using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Storage;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an empty or new SQLite database file.
    /// </summary>
    public class StorageSqliteCreator : StorageToDatabaseFile
    {
        /// <summary>
        /// Creates the basic database structure for a Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="UpdateStorageException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        public void CreateDatabaseStructure(string databaseFilePath)
        {
            Connect(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                SQLiteConnection.CreateFile(databaseFilePath);
            }
            ConnectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath);
            using (var dbContext = new SQLiteConnection(ConnectionString))
            {
                using (var command = dbContext.CreateCommand())
                {
                    try
                    {
                        dbContext.Open();
                        command.CommandText = Resources.DatabaseStructure;
                        command.ExecuteNonQuery();
                    }
                    catch (SQLiteException exception)
                    {
                        throw CreateUpdateStorageException(Resources.Error_Write_Structure_to_Database, exception);
                    }
                }
            }
        }
    }
}