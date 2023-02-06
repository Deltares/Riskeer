﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.IO.HydraRing;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.Integration.IO.Handlers;
using Riskeer.Integration.IO.Properties;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.Integration.IO.Importers
{
    /// <summary>
    /// Importer for hydraulic location configuration database files.
    /// </summary>
    public class HydraulicLocationConfigurationDatabaseImporter : FileImporterBase<HydraulicLocationConfigurationSettings>
    {
        private const int numberOfSteps = 3;
        private readonly List<IObservable> changedObservables = new List<IObservable>();
        private readonly IHydraulicLocationConfigurationDatabaseUpdateHandler updateHandler;
        private readonly HydraulicBoundaryDatabase hydraulicBoundaryDatabase;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicLocationConfigurationDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic location configuration settings to import to.</param>
        /// <param name="updateHandler">The handler responsible for updating the <see cref="HydraulicLocationConfigurationSettings"/>.</param>
        /// <param name="hydraulicBoundaryDatabase">The hydraulic boundary database the settings belong to.</param>
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

            this.updateHandler = updateHandler;
            this.hydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
        }

        protected override bool OnImport()
        {
            InquireConfirmation();

            if (Canceled)
            {
                return false;
            }

            if (Path.GetDirectoryName(FilePath) != Path.GetDirectoryName(hydraulicBoundaryDatabase.FilePath))
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_HLCD_not_in_same_folder_as_HRD));
                return false;
            }

            ReadResult<long> readTrackIdResult = ReadTrackId();

            if (readTrackIdResult.CriticalErrorOccurred || Canceled)
            {
                return false;
            }

            long trackId = readTrackIdResult.Items.Single();

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult = ReadHydraulicLocationConfigurationDatabase(trackId);

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred || Canceled)
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

            if (readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure
                && !File.Exists(HydraulicBoundaryDatabaseHelper.GetPreprocessorClosureFilePath(FilePath)))
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicBoundaryDatabaseImporter_PreprocessorClosure_sqlite_Not_Found));
                return false;
            }

            IEnumerable<long> locationIds = hydraulicBoundaryDatabase.Locations.Select(l => l.Id);
            long[] intersect = locationIds.Intersect(readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations
                                                                                               .Where(rhl => rhl.TrackId == trackId)
                                                                                               .Select(rhl => rhl.HlcdLocationId))
                                          .ToArray();

            if (intersect.Length != locationIds.Count())
            {
                Log.Error(BuildErrorMessage(FilePath, Resources.HydraulicLocationConfigurationDatabaseImporter_Invalid_locationIds));
                return false;
            }

            AddHydraulicLocationConfigurationSettingsToDataModel(
                readHydraulicLocationConfigurationDatabase.ReadHydraulicLocationConfigurationDatabaseSettings?.Single(),
                readHydraulicLocationConfigurationDatabase.UsePreprocessorClosure);

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.HydraulicLocationConfigurationDatabaseImporter_ProgressText_Import_canceled_No_data_changed);
        }

        protected override void DoPostImportUpdates()
        {
            base.DoPostImportUpdates();

            foreach (IObservable changedObservable in changedObservables)
            {
                changedObservable.NotifyObservers();
            }
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

        private void InquireConfirmation()
        {
            if (!updateHandler.InquireConfirmation())
            {
                Cancel();
            }
        }

        private void AddHydraulicLocationConfigurationSettingsToDataModel(ReadHydraulicLocationConfigurationDatabaseSettings readHydraulicLocationConfigurationDatabaseSettings,
                                                                          bool usePrepocessorClosure)
        {
            NotifyProgress(RiskeerCommonIOResources.Importer_ProgressText_Adding_imported_data_to_AssessmentSection, 3, numberOfSteps);
            changedObservables.AddRange(updateHandler.Update(hydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabaseSettings, usePrepocessorClosure, FilePath));
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