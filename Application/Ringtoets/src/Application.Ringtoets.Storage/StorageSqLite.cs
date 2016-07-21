// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Read;
using Application.Ringtoets.Storage.Update;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Ringtoets.Integration.Data;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : IStoreProject
    {
        private string connectionString;

        public string FileFilter
        {
            get
            {
                return Resources.Ringtoets_project_file_filter;
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to a new <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="IProject"/> to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">No file is present at <paramref name="databaseFilePath"/>
        /// at the time a connection is made.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>The database does not contain the table <c>version</c></item>
        /// <item>The file <paramref name="databaseFilePath"/> was not created.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public void SaveProjectAs(string databaseFilePath, IProject project)
        {
            if (!(project is RingtoetsProject))
            {
                throw new ArgumentNullException("project");
            }

            SafeOverwriteFileHelper overwriteHelper = GetSafeOverwriteFileHelper(databaseFilePath);
            if (overwriteHelper != null)
            {
                try
                {
                    SetConnectionToNewFile(databaseFilePath);
                    SaveProjectInDatabase(databaseFilePath, (RingtoetsProject) project);
                }
                catch
                {
                    CleanUpTemporaryFile(overwriteHelper, true);
                }
                CleanUpTemporaryFile(overwriteHelper, false);
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to an existing <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="IProject"/> to save.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">No file is present at <paramref name="databaseFilePath"/>
        /// at the time a connection is made.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist.</item>
        /// <item>The database does not contain the table <c>version</c>.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// <item>The related entity was not found in the database. Therefore, no update was possible.</item>
        /// </list>
        /// </exception>
        public void SaveProject(string databaseFilePath, IProject project)
        {
            if (!(project is RingtoetsProject))
            {
                throw new ArgumentNullException("project");
            }

            try
            {
                SetConnectionToExistingFile(databaseFilePath);
            }
            catch
            {
                SaveProjectAs(databaseFilePath, project);
            }

            UpdateProjectInDatabase(databaseFilePath, (RingtoetsProject) project);
        }

        /// <summary>
        /// Attempts to load the <see cref="IProject"/> from the SQLite database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <returns>Returns a new instance of <see cref="Project"/> with the data from the database or <c>null</c> when not found.</returns>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist.</item>
        /// <item>The database does not contain all requested tables.</item>
        /// <item>The connection to the database file failed.</item>
        /// <item>The related entity was not found in the database.</item>
        /// </list>
        /// </exception>
        public IProject LoadProject(string databaseFilePath)
        {
            SetConnectionToExistingFile(databaseFilePath);
            try
            {
                using (var dbContext = new RingtoetsEntities(connectionString))
                {
                    ProjectEntity projectEntity;

                    try
                    {
                        projectEntity = dbContext.ProjectEntities.Single();
                    }
                    catch (InvalidOperationException exception)
                    {
                        throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
                    }

                    var project = projectEntity.Read(new ReadConversionCollector());

                    project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                    return project;
                }
            }
            catch (DataException exception)
            {
                throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
            }
            catch (SystemException exception)
            {
                throw CreateStorageReaderException(databaseFilePath, Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, exception);
            }
        }

        public void CloseProject()
        {
            connectionString = null;
        }

        public bool HasChanges(IProject project)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return true;
            }

            if (!(project is RingtoetsProject))
            {
                throw new ArgumentNullException("project");
            }

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var ringtoetsProject = (RingtoetsProject) project;
                    var persistenceRegistry = new PersistenceRegistry();
                    ringtoetsProject.Update(persistenceRegistry, dbContext);
                    persistenceRegistry.RemoveUntouched(dbContext);

                    return dbContext.ChangeTracker.HasChanges();
                }
                catch (EntityNotFoundException)
                {
                    return true;
                }
                catch (SystemException)
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// Cleans up a new <see cref="SafeOverwriteFileHelper"/>.
        /// </summary>
        /// <param name="overwriteHelper">The <see cref="SafeOverwriteFileHelper"/> to use for cleaning up.</param>
        /// <param name="revert">Value indicating whether the <paramref name="overwriteHelper"/> should revert to
        /// original file or keep the new file.</param>
        /// <exception cref="StorageException">A file IO operation fails while cleaning up using the 
        /// <paramref name="overwriteHelper"/>.</exception>
        private void CleanUpTemporaryFile(SafeOverwriteFileHelper overwriteHelper, bool revert)
        {
            try
            {
                overwriteHelper.Finish(revert);
            }
            catch (IOException e)
            {
                throw new StorageException(e.Message, e);
            }
        }

        /// <summary>
        /// Creates a new <see cref="SafeOverwriteFileHelper"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path for which to create the <see cref="SafeOverwriteFileHelper"/>.</param>
        /// <returns>A new <see cref="SafeOverwriteFileHelper"/></returns>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">A file IO operation fails while initializing the 
        /// <see cref="SafeOverwriteFileHelper"/>.</exception>
        private SafeOverwriteFileHelper GetSafeOverwriteFileHelper(string databaseFilePath)
        {
            try
            {
                return new SafeOverwriteFileHelper(databaseFilePath);
            }
            catch (IOException e)
            {
                throw new StorageException(e.Message, e);
            }
        }

        private void SaveProjectInDatabase(string databaseFilePath, RingtoetsProject project)
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var registry = new PersistenceRegistry();
                    dbContext.ProjectEntities.Add(project.Create(registry));
                    dbContext.SaveChanges();
                    registry.TransferIds();

                    project.Name = Path.GetFileNameWithoutExtension(databaseFilePath);
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(databaseFilePath, Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(databaseFilePath, Resources.Error_During_Connection, exception);
                    }
                    throw;
                }
            }
        }

        private void UpdateProjectInDatabase(string databaseFilePath, RingtoetsProject project)
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    var updateCollector = new PersistenceRegistry();
                    project.Update(updateCollector, dbContext);
                    updateCollector.RemoveUntouched(dbContext);
                    dbContext.SaveChanges();
                    updateCollector.TransferIds();
                }
                catch (EntityNotFoundException e)
                {
                    throw CreateStorageWriterException(databaseFilePath, e.Message, e);
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(databaseFilePath, Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    if (exception is InvalidOperationException || exception is NotSupportedException)
                    {
                        throw CreateStorageWriterException(databaseFilePath, Resources.Error_During_Connection, exception);
                    }
                    throw;
                }
            }
        }

        /// <summary>
        /// Attempts to set the connection to an existing storage file <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist</item>
        /// <item>the database has an invalid schema.</item>
        /// </list>
        /// </exception>
        private void SetConnectionToExistingFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            SetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to a newly created (empty) Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="CouldNotConnectException">No file is present at <paramref name="databaseFilePath"/>
        /// at the time a connection is made.</exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> was not created</item>
        /// <item>executing <c>DatabaseStructure</c> script failed</item>
        /// </list>
        /// </exception>
        /// <exception cref="IOException">Thrown when the <paramref name="databaseFilePath"/> could not 
        /// be overwritten.</exception>
        private void SetConnectionToNewFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            StorageSqliteCreator.CreateDatabaseStructure(databaseFilePath);
            SetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Establishes a connection to an existing <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">The path of the database file to connect to.</param>
        /// <exception cref="CouldNotConnectException">No file exists at <paramref name="databaseFilePath"/>.</exception>
        private void SetConnectionToFile(string databaseFilePath)
        {
            if (!File.Exists(databaseFilePath))
            {
                var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(UtilsResources.Error_File_does_not_exist);
                throw new CouldNotConnectException(message);
            }

            SetConnectionToStorage(databaseFilePath);
        }

        /// <summary>
        /// Sets the connection to the Ringtoets database.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file, which is used for creating exceptions.</param>
        /// <exception cref="StorageValidationException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void SetConnectionToStorage(string databaseFilePath)
        {
            connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(databaseFilePath);

            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.VersionEntities.Load();
                }
                catch (Exception exception)
                {
                    var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(string.Format(Resources.StorageSqLite_LoadProject_Invalid_Ringtoets_database_file, databaseFilePath));
                    throw new StorageValidationException(message, exception);
                }
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when writing to the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageWriterException(string databaseFilePath, string errorMessage, Exception innerException)
        {
            var message = new FileWriterErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when reading the storage file failed.
        /// </summary>
        /// <param name="databaseFilePath">The path of the file that was attempted to connect with.</param>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageReaderException(string databaseFilePath, string errorMessage, Exception innerException)
        {
            var message = new FileReaderErrorMessageBuilder(databaseFilePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }
    }
}