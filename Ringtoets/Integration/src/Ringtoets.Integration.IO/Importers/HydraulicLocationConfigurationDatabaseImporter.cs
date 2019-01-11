// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Ringtoets.Integration.IO.Handlers;
using Ringtoets.Integration.IO.Properties;

namespace Ringtoets.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic location configuration database files.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseImporter : FileImporterBase<HydraulicLocationConfigurationSettings>
    {
        private const int numberOfSteps = 2;
        private readonly HydraulicBoundaryDatabase hydraulicBoundaryDatabase;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic location configuration settings to import to.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicLocationConfigurationSettings"/>.</param>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database the settings belongs to.</param>
        /// <param name="filePath">The path of the hydraulic location configuration settings file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicLocationConfigurationDatabaseImporter(HydraulicLocationConfigurationSettings importTarget,
                                                              IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler,
                                                              HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                              string filePath)
            : base(filePath, importTarget)
        {
            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            this.hydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
        }

        protected override bool OnImport()
        {
            if (Path.GetDirectoryName(FilePath) != Path.GetDirectoryName(hydraulicBoundaryDatabase.FilePath))
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD));
                return false;
            }

            ReadResult<long> readTrackIdResult = ReadTrackId();

            if (readTrackIdResult.CriticalErrorOccurred)
            {
                return false;
            }

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase(
                readTrackIdResult.Items.Single());

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred)
            {
                return false;
            }

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = readHydraulicLocationConfigurationDatabaseResult.Items.Single();
            if (readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings != null 
                && readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings.Count() != 1)
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_Invalid_number_of_ScenarioInformation_entries));
                return false;
            }

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            throw new NotImplementedException();
        }

        private ReadResult<long> ReadTrackId()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HRD_file, 1, numberOfSteps);
            try
            {
                using (var reader = new HydraulicBoundaryDatabaseReader(hydraulicBoundaryDatabase.FilePath))
                {
                    return new ReadResult<long>(false)
                    {
                        Items = new[]
                        {
                            reader.ReadTrackId()
                        }
                    };
                }
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                return HandleCriticalFileReadError<long>(e);
            }
        }

        private ReadResult<ReadHydraulicLocationConfigurationDatabase> ReadHydraulicLocationConfigurationDatabase(long trackId)
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_HLCD_file, 2, numberOfSteps);
            try
            {
                using (var reader = new HydraulicLocationConfigurationDatabaseReader(FilePath))
                {
                    return new ReadResult<ReadHydraulicLocationConfigurationDatabase>(false)
                    {
                        Items = new[]
                        {
                            reader.Read(trackId)
                        }
                    };
                }
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                return HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(e);
            }
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(Exception e)
        {
            string errorMessage = string.Format(Resources.HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported,
                                                e.Message);
            Log.Error(errorMessage);
            return new ReadResult<T>(true);
        }

        private static string BuildErrorMessage(string filePath, string message)
        {
            return new FileReaderErrorMessageBuilder(filePath).Build(
                string.Format(Resources.HydraulicLocationConfigurationDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicLocationConfigurationDatabase_imported,
                              message));
        }
    }
}