using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using Application.Ringtoets.Storage.Converters;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Data;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : StorageToFile
    {
        /// <summary>
        /// Creates a new instance of <see cref="StorageSqLite"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException">Thrown when <paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="FileNotFoundException">Thrown when <paramref name="databaseFilePath"/> does not exist.</exception>
        public StorageSqLite(string databaseFilePath)
            : base(databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);

            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new FileNotFoundException(message);
            }

            ConnectionString = SqLiteStorageConnection.BuildSqLiteEntityConnectionString(databaseFilePath);
        }

        /// <summary>
        /// Tests if a connection can be made to a Ringtoets project file by verifying if the table 'Version' exists.
        /// </summary>
        /// <returns>Returns <c>true</c> if a valid connection can be made, <c>false</c> otherwise.</returns>
        public bool TestConnection()
        {
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.Versions.Load();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to a new <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="UpdateDatabaseException">Thrown when
        /// <list type="bullet">
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public int SaveProjectAs(Project project)
        {
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                try
                {
                    var projectEntity = new ProjectEntity();
                    ProjectEntityConverter.ProjectToProjectEntity(project, projectEntity);
                    dbContext.ProjectEntities.Add(projectEntity);
                    return dbContext.SaveChanges();
                }
                catch (DataException exception)
                {
                    throw CreateUpdateDatabaseException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    throw CreateUpdateDatabaseException(Resources.Error_During_Connection, exception);
                }
            }
        }

        /// <summary>
        /// Attempts to load the <see cref="Project"/> from the SQLite database.
        /// </summary>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when ProjectEntities is null.</exception>
        public Project LoadProject()
        {
            using (var dbContext = new RingtoetsEntities(ConnectionString))
            {
                return ProjectEntityConverter.GetProject(dbContext.ProjectEntities);
            }
        }
    }
}