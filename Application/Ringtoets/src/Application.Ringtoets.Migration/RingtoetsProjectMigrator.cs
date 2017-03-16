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
using System.IO;
using Application.Ringtoets.Migration.Core;
using Application.Ringtoets.Migration.Properties;
using Core.Common.Base.Storage;
using Core.Common.Gui;
using Core.Common.Utils;
using log4net;
using Migration.Scripts.Data.Exceptions;
using Ringtoets.Common.Utils;
using MigrationCoreStorageResources = Migration.Core.Storage.Properties.Resources;

namespace Application.Ringtoets.Migration
{
    /// <summary>
    /// A GUI implementation to migrate a Ringtoets database file to a newer version.
    /// </summary>
    public class RingtoetsProjectMigrator : IMigrateProject
    {
        private static readonly string currentDatabaseVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

        private readonly ILog log = LogManager.GetLogger(typeof(RingtoetsProjectMigrator));
        private readonly RingtoetsSqLiteDatabaseFileMigrator fileMigrator;
        private readonly IInquiryHelper inquiryHelper;
        private readonly FileFilterGenerator fileFilter;

        /// <summary>
        /// Instantiates a <see cref="RingtoetsProjectMigrator"/>.
        /// </summary>
        /// <param name="inquiryHelper">Object responsible for inquiring the data.</param>
        public RingtoetsProjectMigrator(IInquiryHelper inquiryHelper)
        {
            if (inquiryHelper == null)
            {
                throw new ArgumentNullException(nameof(inquiryHelper));
            }

            this.inquiryHelper = inquiryHelper;
            fileMigrator = new RingtoetsSqLiteDatabaseFileMigrator();
            fileFilter = new FileFilterGenerator(Resources.RingtoetsProject_FileExtension,
                                                 Resources.RingtoetsProject_TypeDescription);
        }

        public MigrationRequired ShouldMigrate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            ValidateProjectPath(filePath, nameof(filePath), Resources.RingtoetsProjectMigrator_Source_Descriptor);

            var versionedFile = new RingtoetsVersionedFile(filePath);
            string version = versionedFile.GetVersion();

            if (version.Equals(currentDatabaseVersion))
            {
                return MigrationRequired.No;
            }

            if (!fileMigrator.IsVersionSupported(version))
            {
                string errorMessage = string.Format(MigrationCoreStorageResources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                    version, currentDatabaseVersion);
                log.Error(errorMessage);
                return MigrationRequired.Aborted;
            }

            string query = string.Format(Resources.RingtoetsProjectMigrator_Migrate_Outdated_project_file_update_to_current_version_0_inquire,
                                         currentDatabaseVersion);
            if (inquiryHelper.InquireContinuation(query))
            {
                return MigrationRequired.Yes;
            }

            GenerateMigrationCancelledLogMessage(filePath);
            return MigrationRequired.Aborted;
        }

        public string DetermineMigrationLocation(string originalFilePath)
        {
            if (originalFilePath == null)
            {
                throw new ArgumentNullException(nameof(originalFilePath));
            }

            ValidateProjectPath(originalFilePath, nameof(originalFilePath), Resources.RingtoetsProjectMigrator_Source_Descriptor);

            string suggestedFileName = GetSuggestedFileName(originalFilePath);
            string migrationLocation = inquiryHelper.GetTargetFileLocation(fileFilter.Filter, suggestedFileName);
            if (migrationLocation == null)
            {
                GenerateMigrationCancelledLogMessage(originalFilePath);
            }
            return migrationLocation;
        }

        public bool Migrate(string sourceFilePath, string targetFilePath)
        {
            if (sourceFilePath == null)
            {
                throw new ArgumentNullException(nameof(sourceFilePath));
            }
            if (targetFilePath == null)
            {
                throw new ArgumentNullException(nameof(targetFilePath));
            }

            ValidateProjectPath(sourceFilePath, nameof(sourceFilePath), Resources.RingtoetsProjectMigrator_Source_Descriptor);
            ValidateProjectPath(targetFilePath, nameof(targetFilePath), Resources.RingtoetsProjectMigrator_Target_Descriptor);

            return MigrateToTargetLocation(sourceFilePath, targetFilePath);
        }

        private bool MigrateToTargetLocation(string sourceFilePath, string targetLocation)
        {
            try
            {
                var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
                fileMigrator.Migrate(versionedFile, currentDatabaseVersion, targetLocation);

                string message = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Outdated_projectfile_0_succesfully_updated_to_target_filepath_1_version_2_,
                                               sourceFilePath, targetLocation, currentDatabaseVersion);
                log.Info(message);

                return true;
            }
            catch (CriticalMigrationException e)
            {
                string errorMessage = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Updating_outdated_projectfile_0_failed_with_exception_1_,
                                                    sourceFilePath, e.Message);
                log.Error(errorMessage, e);
                return false;
            }
        }

        private static string GetSuggestedFileName(string sourceFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string versionSuffix = currentDatabaseVersion.Replace(".", "-");
            string suggestedFileName = $"{fileName}_{versionSuffix}";

            return suggestedFileName;
        }

        private void GenerateMigrationCancelledLogMessage(string sourceFilePath)
        {
            string warningMessage = string.Format(Resources.RingtoetsProjectMigrator_GenerateMigrationCancelledLogMessage_Updating_projectfile_0_was_cancelled, sourceFilePath);
            log.Warn(warningMessage);
        }

        /// <summary>
        /// Validates a given file path.
        /// </summary>
        /// <param name="filePath">The file path to be validated.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="pathDescription">Prefix, describing the type of file path is being validated.</param>
        /// <exception cref="ArgumentException">Thrown when the file path argument with the
        /// given name is not valid.</exception>
        private static void ValidateProjectPath(string filePath, string argumentName, string pathDescription)
        {
            if (!IOUtils.IsValidFilePath(filePath))
            {
                string message = string.Format(Resources.RingtoetsProjectMigrator_ValidateProjectPath_TypeDescriptor_0_filepath_must_be_a_valid_path,
                                               pathDescription);
                throw new ArgumentException(message, argumentName);
            }
        }
    }
}