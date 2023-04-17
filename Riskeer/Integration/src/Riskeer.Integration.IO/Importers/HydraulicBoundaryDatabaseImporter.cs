// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
    /// Importer for hydraulic boundary database files.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : FileImporterBase<HydraulicBoundaryData>
    {
        private const int numberOfSteps = 4;

        private readonly IHydraulicBoundaryDataUpdateHandler updateHandler;
        private readonly List<IObservable> changedObservables = new List<IObservable>();

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabaseImporter"/>.
        /// </summary>
        /// <param name="importTarget">The hydraulic boundary data to import to.</param>
        /// <param name="updateHandler">The object responsible for updating the <see cref="HydraulicBoundaryData"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public HydraulicBoundaryDatabaseImporter(HydraulicBoundaryData importTarget,
                                                 IHydraulicBoundaryDataUpdateHandler updateHandler,
                                                 string filePath)
            : base(filePath, importTarget)
        {
            if (updateHandler == null)
            {
                throw new ArgumentNullException(nameof(updateHandler));
            }

            this.updateHandler = updateHandler;
        }

        protected override bool OnImport()
        {
            if (!IsHrdFileInSameFolderAsHlcdFile() || !IsNewHrdFile())
            {
                return false;
            }

            ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase = TryReadHydraulicBoundaryDatabase();
            if (readHydraulicBoundaryDatabase == null)
            {
                return false;
            }

            ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase = TryReadHydraulicLocationConfigurationDatabase();
            if (readHydraulicLocationConfigurationDatabase == null)
            {
                return false;
            }

            IEnumerable<long> readExcludedLocationIds = TryReadExcludedLocationIds();
            if (readExcludedLocationIds == null)
            {
                return false;
            }

            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocationsToAdd = GetHydraulicBoundaryLocationsToAdd(
                readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase, readExcludedLocationIds.ToArray());

            if (!HydraulicBoundaryLocationsToAddHaveNonExistingId(hydraulicBoundaryLocationsToAdd))
            {
                return false;
            }

            AddHydraulicBoundaryDatabaseToDataModel(readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase, readExcludedLocationIds);

            return true;
        }

        protected override void LogImportCanceledMessage()
        {
            Log.Info(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Import_canceled_No_data_changed);
        }

        protected override void DoPostImportUpdates()
        {
            base.DoPostImportUpdates();

            foreach (IObservable changedObservable in changedObservables)
            {
                changedObservable.NotifyObservers();
            }
        }

        private bool IsHrdFileInSameFolderAsHlcdFile()
        {
            if (Path.GetDirectoryName(ImportTarget.HydraulicLocationConfigurationDatabase.FilePath) == Path.GetDirectoryName(FilePath))
            {
                return true;
            }

            Log.Error(BuildErrorMessage(Resources.HydraulicBoundaryDatabaseImporter_Hrd_file_not_in_same_folder_as_hlcd_file));
            return false;
        }

        private bool IsNewHrdFile()
        {
            if (ImportTarget.HydraulicBoundaryDatabases.All(hbd => hbd.FilePath != FilePath))
            {
                return true;
            }

            Log.Error(BuildErrorMessage(Resources.HydraulicBoundaryDatabaseImporter_Hrd_file_already_added));
            return false;
        }

        private ReadHydraulicBoundaryDatabase TryReadHydraulicBoundaryDatabase()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_Hrd_file, 1, numberOfSteps);

            ReadResult<ReadHydraulicBoundaryDatabase> readHydraulicBoundaryDatabaseResult;

            try
            {
                using (var reader = new HydraulicBoundaryDatabaseReader(FilePath))
                {
                    readHydraulicBoundaryDatabaseResult = new ReadResult<ReadHydraulicBoundaryDatabase>(false)
                    {
                        Items = new[]
                        {
                            reader.Read()
                        }
                    };
                }
            }
            catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
            {
                readHydraulicBoundaryDatabaseResult = HandleCriticalFileReadError<ReadHydraulicBoundaryDatabase>(e);
            }

            if (readHydraulicBoundaryDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return null;
            }

            return readHydraulicBoundaryDatabaseResult.Items.Single();
        }

        private ReadHydraulicLocationConfigurationDatabase TryReadHydraulicLocationConfigurationDatabase()
        {
            NotifyProgress(Resources.HydraulicBoundaryDataImporter_ProgressText_Reading_Hlcd_file, 2, numberOfSteps);

            ReadResult<ReadHydraulicLocationConfigurationDatabase> readHydraulicLocationConfigurationDatabaseResult;

            try
            {
                using (var reader = new HydraulicLocationConfigurationDatabaseReader(ImportTarget.HydraulicLocationConfigurationDatabase.FilePath))
                {
                    try
                    {
                        readHydraulicLocationConfigurationDatabaseResult = new ReadResult<ReadHydraulicLocationConfigurationDatabase>(false)
                        {
                            Items = new[]
                            {
                                reader.Read()
                            }
                        };
                    }
                    catch (Exception e) when (e is CriticalFileReadException || e is LineParseException)
                    {
                        readHydraulicLocationConfigurationDatabaseResult = HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(e);
                    }
                }
            }
            catch (CriticalFileReadException)
            {
                readHydraulicLocationConfigurationDatabaseResult = HandleCriticalFileReadError<ReadHydraulicLocationConfigurationDatabase>(Resources.HydraulicBoundaryDatabaseImporter_Hlcd_sqlite_not_found);
            }

            if (readHydraulicLocationConfigurationDatabaseResult.CriticalErrorOccurred || Canceled)
            {
                return null;
            }

            return readHydraulicLocationConfigurationDatabaseResult.Items.Single();
        }

        private IEnumerable<long> TryReadExcludedLocationIds()
        {
            NotifyProgress(Resources.HydraulicBoundaryDatabaseImporter_ProgressText_Reading_Hrd_settings_file, 3, numberOfSteps);

            ReadResult<IEnumerable<long>> readExcludedLocationIdsResult;

            try
            {
                using (var reader = new HydraRingSettingsDatabaseReader(HydraulicBoundaryDataHelper.GetHydraulicBoundarySettingsDatabaseFilePath(FilePath)))
                {
                    try
                    {
                        readExcludedLocationIdsResult = new ReadResult<IEnumerable<long>>(false)
                        {
                            Items = new[]
                            {
                                reader.ReadExcludedLocations().ToArray()
                            }
                        };
                    }
                    catch (CriticalFileReadException e)
                    {
                        readExcludedLocationIdsResult = HandleCriticalFileReadError<IEnumerable<long>>(e.Message);
                    }
                }
            }
            catch (CriticalFileReadException e)
            {
                readExcludedLocationIdsResult = HandleCriticalFileReadError<IEnumerable<long>>(
                    string.Format(Resources.HydraulicBoundaryDatabaseImporter_Cannot_open_hydraulic_calculation_settings_file_0_, e.Message));
            }

            if (readExcludedLocationIdsResult.CriticalErrorOccurred || Canceled)
            {
                return null;
            }

            return readExcludedLocationIdsResult.Items.Single();
        }

        private static IEnumerable<HydraulicBoundaryLocation> GetHydraulicBoundaryLocationsToAdd(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                                                                                 ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                                                                 long[] readExcludedLocationIds)
        {
            ReadHydraulicLocation[] readHydraulicLocations = readHydraulicLocationConfigurationDatabase.ReadHydraulicLocations
                                                                                                       .Where(rhl => rhl.TrackId == readHydraulicBoundaryDatabase.TrackId)
                                                                                                       .ToArray();

            foreach (ReadHydraulicBoundaryLocation readHydraulicBoundaryLocation in readHydraulicBoundaryDatabase.Locations)
            {
                long hydraulicBoundaryLocationId = readHydraulicLocations.Where(m => m.HrdLocationId == readHydraulicBoundaryLocation.Id)
                                                                         .Select(m => m.HlcdLocationId)
                                                                         .FirstOrDefault();

                if (hydraulicBoundaryLocationId != 0 && !readExcludedLocationIds.Contains(hydraulicBoundaryLocationId))
                {
                    yield return new HydraulicBoundaryLocation(hydraulicBoundaryLocationId, readHydraulicBoundaryLocation.Name,
                                                               readHydraulicBoundaryLocation.CoordinateX, readHydraulicBoundaryLocation.CoordinateY);
                }
            }
        }

        private bool HydraulicBoundaryLocationsToAddHaveNonExistingId(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocationsToAdd)
        {
            long[] existingHydraulicBoundaryLocationIds = ImportTarget.GetLocations().Select(hbl => hbl.Id).ToArray();
            long[] newHydraulicBoundaryLocationIds = hydraulicBoundaryLocationsToAdd.Select(hbl => hbl.Id).ToArray();

            if (newHydraulicBoundaryLocationIds.Except(existingHydraulicBoundaryLocationIds).Count() == newHydraulicBoundaryLocationIds.Length)
            {
                return true;
            }

            Log.Error(BuildErrorMessage(Resources.HydraulicBoundaryDatabaseImporter_Hrd_file_contains_one_or_more_locations_with_existing_id));
            return false;
        }

        private void AddHydraulicBoundaryDatabaseToDataModel(ReadHydraulicBoundaryDatabase readHydraulicBoundaryDatabase,
                                                             ReadHydraulicLocationConfigurationDatabase readHydraulicLocationConfigurationDatabase,
                                                             IEnumerable<long> excludedLocationIds)
        {
            NotifyProgress(RiskeerCommonIOResources.Importer_ProgressText_Adding_imported_data_to_AssessmentSection, 4, numberOfSteps);

            changedObservables.AddRange(updateHandler.AddHydraulicBoundaryDatabase(ImportTarget, readHydraulicBoundaryDatabase, readHydraulicLocationConfigurationDatabase,
                                                                                   excludedLocationIds, FilePath));
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(Exception e)
        {
            Log.Error(string.Format(Resources.HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported,
                                    e.Message));

            return new ReadResult<T>(true);
        }

        private ReadResult<T> HandleCriticalFileReadError<T>(string message)
        {
            Log.Error(BuildErrorMessage(message));

            return new ReadResult<T>(true);
        }

        private string BuildErrorMessage(string message)
        {
            return new FileReaderErrorMessageBuilder(FilePath).Build(
                string.Format(Resources.HydraulicBoundaryDatabaseImporter_HandleCriticalFileReadError_Error_0_No_HydraulicBoundaryDatabase_imported,
                              message));
        }
    }
}