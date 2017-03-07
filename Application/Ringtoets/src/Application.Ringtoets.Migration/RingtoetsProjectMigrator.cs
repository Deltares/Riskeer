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
        private static readonly string currentProjectVersion = RingtoetsVersionHelper.GetCurrentDatabaseVersion();

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

        public bool ShouldMigrate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            ValidateProjectPath(filePath);

            var versionedFile = new RingtoetsVersionedFile(filePath);
            string version = versionedFile.GetVersion();

            if (version.Equals(currentProjectVersion))
            {
                return false;
            }

            bool isVersionSupported = fileMigrator.IsVersionSupported(version);
            if (!isVersionSupported)
            {
                string errorMessage = string.Format(MigrationCoreStorageResources.Migrate_From_Version_0_To_Version_1_Not_Supported,
                                                    version, currentProjectVersion);
                log.Error(errorMessage);
            }
            return isVersionSupported;
        }

        public string Migrate(string filePath)
        {
            if (filePath == null)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            ValidateProjectPath(filePath);

            string query = Resources.RingtoetsProjectMigrator_Migrate_Outdated_project_file_update_to_current_version_inquire;
            if (inquiryHelper.InquireContinuation(query))
            {
                string suggestedFileName = GetSuggestedFileName(filePath);
                string targetLocation = inquiryHelper.GetTargetFileLocation(fileFilter, suggestedFileName);
                if (!string.IsNullOrEmpty(targetLocation))
                {
                    return MigrateToTargetLocation(filePath, targetLocation);
                }

                GenerateMigrationCancelledLogMessage(filePath);
            }

            GenerateMigrationCancelledLogMessage(filePath);

            return null;
        }

        private string MigrateToTargetLocation(string sourceFilePath, string targetLocation)
        {
            try
            {
                var versionedFile = new RingtoetsVersionedFile(sourceFilePath);
                fileMigrator.Migrate(versionedFile, currentProjectVersion, targetLocation);

                string message = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Outdated_projectfile_0_succesfully_updated_to_target_filepath_1_version_2_,
                                               sourceFilePath, targetLocation, currentProjectVersion);
                log.Info(message);

                return targetLocation;
            }
            catch (CriticalMigrationException e)
            {
                string errorMessage = string.Format(Resources.RingtoetsProjectMigrator_MigrateToTargetLocation_Updating_outdated_projectfile_0_failed_with_exception_1_,
                                                    sourceFilePath, e.Message);
                log.Error(errorMessage, e);
                return null;
            }
        }

        private static string GetSuggestedFileName(string sourceFilePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(sourceFilePath);
            string versionSuffix = currentProjectVersion.Replace(".", "-");
            string suggestedFileName = $"{fileName}_{versionSuffix}";

            return suggestedFileName;
        }

        private void GenerateMigrationCancelledLogMessage(string sourceFilePath)
        {
            string warningMessage = string.Format(Resources.RingtoetsProjectMigrator_GenerateMigrationCancelledLogMessage_Updating_projectfile_0_was_cancelled, sourceFilePath);
            log.Warn(warningMessage);
        }

        private static void ValidateProjectPath(string sourceFilePath)
        {
            if (!IOUtils.IsValidFilePath(sourceFilePath))
            {
                throw new ArgumentException(Resources.RingtoetsProjectMigrator_ValidateProjectPath_Source_filepath_must_be_a_valid_path,
                                            nameof(sourceFilePath));
            }
        }
    }
}