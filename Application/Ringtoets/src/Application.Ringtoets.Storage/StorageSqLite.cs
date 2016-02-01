﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using System.Data;
using System.Data.Entity;
using System.IO;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Persistors;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Application.Ringtoets.Storage
{
    /// <summary>
    /// This class interacts with an SQLite database file using the Entity Framework.
    /// </summary>
    public class StorageSqLite : IStoreProject
    {
        private string filePath;
        private string connectionString;

        /// <summary>
        /// Converts <paramref name="project"/> to a new <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when
        /// <list type="bullet">
        /// <item>The database does not contain the table <c>version</c></item>
        /// <item>THe file <paramref name="databaseFilePath"/> was not created.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// </list>
        /// </exception>
        public int SaveProjectAs(string databaseFilePath, Project project)
        {
            SetConnectionToNewFile(databaseFilePath);
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                var projectEntityPersistor = new ProjectEntityPersistor(dbContext.ProjectEntities);
                var projectEntity = projectEntityPersistor.AddEntity(project);
                try
                {
                    var changes = dbContext.SaveChanges();
                    project.StorageId = projectEntity.ProjectEntityId;
                    return changes;
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_During_Connection, exception);
                }
            }
        }

        /// <summary>
        /// Converts <paramref name="project"/> to an existing <see cref="ProjectEntity"/> in the database.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <param name="project"><see cref="Project"/> to save.</param>
        /// <returns>Returns the number of changes that were saved in <see cref="RingtoetsEntities"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="project"/> is null.</exception>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="UpdateStorageException">Thrown when
        /// <list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist.</item>
        /// <item>The database does not contain the table <c>version</c>.</item>
        /// <item>Saving the <paramref name="project"/> to the database failed.</item>
        /// <item>The connection to the database file failed.</item>
        /// <item>The related entity was not found in the database. Therefore, no update was possible.</item>
        /// </list>
        /// </exception>
        public int SaveProject(string databaseFilePath, Project project)
        {
            SetConnectionToFile(databaseFilePath);
            if (project == null)
            {
                throw new ArgumentNullException("project");
            }
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                var projectEntityPersistor = new ProjectEntityPersistor(dbContext.ProjectEntities);
                try
                {
                    projectEntityPersistor.UpdateEntity(project);
                    return dbContext.SaveChanges();
                }
                catch (EntityNotFoundException)
                {
                    throw CreateStorageWriterException(string.Format(Resources.Error_Entity_Not_Found_0_1, "project", project.StorageId));
                }
                catch (DataException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_Update_Database, exception);
                }
                catch (SystemException exception)
                {
                    throw CreateStorageWriterException(Resources.Error_During_Connection, exception);
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
        /// <exception cref="StorageValidationException">Thrown when the database does not contain all tables of <see cref="RingtoetsEntities"/>.</exception>
        public Project LoadProject(string databaseFilePath)
        {
            SetConnectionToFile(databaseFilePath);
            try
            {
                using (var dbContext = new RingtoetsEntities(connectionString))
                {
                    var projectEntityPersistor = new ProjectEntityPersistor(dbContext.ProjectEntities);
                    return projectEntityPersistor.GetEntityAsModel();
                }
            }
            catch (DataException exception)
            {
                throw CreateStorageReaderException(string.Empty, exception);
            }
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when writing to the storage file failed.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageWriterException(string errorMessage, Exception innerException = null)
        {
            var message = new FileWriterErrorMessageBuilder(filePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Throws a configured instance of <see cref="StorageException"/> when reading the storage file failed.
        /// </summary>
        /// <param name="errorMessage">The critical error message.</param>
        /// <param name="innerException">Optional: exception that caused this exception to be thrown.</param>
        /// <returns>Returns a new <see cref="StorageException"/>.</returns>
        private StorageException CreateStorageReaderException(string errorMessage, Exception innerException = null)
        {
            var message = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);
            return new StorageException(message, innerException);
        }

        /// <summary>
        /// Attempts to set the connection to an existing storage file <paramref name="databaseFilePath"/>.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="StorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> does not exist</item>/// <item>the database does not contain the table <c>version</c>.</item>
        /// </list>
        /// </exception>
        private void SetConnectionToFile(string databaseFilePath)
        {
            FileUtils.ValidateFilePath(databaseFilePath);
            filePath = databaseFilePath;

            if (!File.Exists(databaseFilePath))
            {
                throw CreateStorageReaderException(string.Empty, new CouldNotConnectException(UtilsResources.Error_File_does_not_exist));
            }

            connectionString = SqLiteConnectionStringBuilder.BuildSqLiteEntityConnectionString(databaseFilePath);

            ValidateStorage();
        }

        /// <summary>
        /// Sets the connection to a newly created (empty) Ringtoets database file.
        /// </summary>
        /// <param name="databaseFilePath">Path to database file.</param>
        /// <exception cref="System.ArgumentException"><paramref name="databaseFilePath"/> is invalid.</exception>
        /// <exception cref="UpdateStorageException">Thrown when:<list type="bullet">
        /// <item><paramref name="databaseFilePath"/> was not created</item>
        /// <item>executing <c>DatabaseStructure</c> script failed</item>
        /// <item>the database does not contain the table <c>version</c>.</item>
        /// </list>
        /// </exception>
        private void SetConnectionToNewFile(string databaseFilePath)
        {
            StorageSqliteCreator.CreateDatabaseStructure(databaseFilePath);

            SetConnectionToFile(databaseFilePath);
        }

        /// <summary>
        /// Validates if the connected storage is a valid Ringtoets database.
        /// </summary>
        /// <exception cref="StorageException">Thrown when the database does not contain the table <c>version</c>.</exception>
        private void ValidateStorage()
        {
            using (var dbContext = new RingtoetsEntities(connectionString))
            {
                try
                {
                    dbContext.Database.Initialize(true);
                    dbContext.Versions.Load();
                }
                catch
                {
                    throw CreateStorageReaderException(string.Empty, new StorageValidationException(string.Format(Resources.Error_Validating_Database_0, filePath)));
                }
            }
        }
    }
}