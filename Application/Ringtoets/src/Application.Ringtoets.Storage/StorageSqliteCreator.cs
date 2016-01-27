using System.Data.SQLite;
using System.IO;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an empty or new SQLite database file.
    /// </summary>
    public static class StorageSqliteCreator
    {
        /// <summary>
        /// Creates the basic database structure for a Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        public static void CreateDatabaseStructure(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                SQLiteConnection.CreateFile(databaseFilePath);
            }
            var connectionString = SqLiteConnectionStringBuilder.BuildSqLiteConnectionString(databaseFilePath);
            using (var dbContext = new SQLiteConnection(connectionString))
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
                        var message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(Resources.Error_Write_Structure_to_Database);
                        throw new StorageException(message, new UpdateStorageException("", exception));
                    }
                }
            }
        }
    }
}