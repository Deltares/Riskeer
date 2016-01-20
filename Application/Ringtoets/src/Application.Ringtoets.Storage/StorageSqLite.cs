using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : StorageToDatabaseFile, IStoreProject
    {
        /// <summary>
        /// Converts <paramref name="project"/> to a new <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when <paramref name="databaseFilePath"/> was not created.</exception>
        /// <exception cref="UpdateStorageException">Thrown when
        /// <list type="bullet">
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public int SaveProjectAs(string databaseFilePath, Project project)
        {
            ConnectToNew(databaseFilePath);
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                ProjectEntityConverter.InsertProjectEntity(dbContext.ProjectEntities, project);
                try
                {
                    return dbContext.SaveChanges();
                }
                catch (DataException exception)
                {
                    throw CreateUpdateStorageException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    throw CreateUpdateStorageException(Resources.Error_During_Connection, exception);
                }
            }
        }

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the SQLite database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when <paramref name="databaseFilePath"/> does not exist.</exception>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        public Project LoadProject(string databaseFilePath)
        {
            Connect(databaseFilePath);
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                return ProjectEntityConverter.GetProject(dbContext.ProjectEntities);
            }
        }

        /// <summary>
        /// Attempts to connect to existing storage file <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when <paramref name="databaseFilePath"/> does not exist.</exception>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        protected override void Connect(string databaseFilePath)
        {
            base.Connect(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new CouldNotConnectException(message);
            }

            ConnectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(databaseFilePath);

            ValidateStorage();
        }

        /// <summary>
        /// Creates a new (empty) Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">Thrown when <paramref name="databaseFilePath"/> was not created.</exception>
        /// <exception cref="UpdateStorageException">Thrown when executing <c>DatabaseStructure</c> script fails.</exception>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void ConnectToNew(string databaseFilePath)
        {
            base.Connect(databaseFilePath);

            var creator = new StorageSqliteCreator();
            creator.CreateDatabaseStructure(databaseFilePath);

            Connect(databaseFilePath);
        }

        /// <summary>
        /// Validates if the connected storage is a valid Ringtoets database.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <c>ConnectionString</c> has not been set.</exception>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void ValidateStorage()
        {
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.Versions.Load();
                }
                catch
                {
                    throw new StorageValidationException(string.Format(Resources.Error_Validating_Database_0, FilePath));
                }
            }
        }
    }
}