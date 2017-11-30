// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.Utils.Builders;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.Common.IO.Properties;
using Ringtoets.HydraRing.IO.HydraulicBoundaryDatabase;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.Common.IO.FileImporters
{
    /// <summary>
    /// Imports locations read from an Hydraulic boundary .sqlite file (SqlLite database file) to a 
    /// collection of <see cref="HydraulicBoundaryLocation"/> in a <see cref="HydraulicBoundaryDatabase"/>.
    /// </summary>
    public class HydraulicBoundaryDatabaseImporter : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(HydraulicBoundaryDatabaseImporter));
        private HydraulicBoundaryDatabaseReader hydraulicBoundaryDatabaseReader;
        private HydraulicLocationConfigurationDatabaseReader hydraulicLocationConfigurationDatabaseReader;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryDatabase"/>, based upon the data read from 
        /// the hydraulic boundary database file, and saved into <paramref name="targetItem"/>.
        /// </summary>
        /// <param name="targetItem"><see cref="IAssessmentSection"/> to set the newly 
        /// created <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <param name="filePath">The path of the hydraulic boundary database file to import from.</param>
        /// <returns><c>true</c> if the import was successful, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetItem"/> is <c>null</c>.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The given file at <paramref name="filePath"/> cannot be read.</item>
        /// <item>The file 'HLCD.sqlite' in the same folder as <paramref name="filePath"/> cannot be read.</item>
        /// </list>
        /// </exception>
        public bool Import(IAssessmentSection targetItem, string filePath)
        {
            if (targetItem == null)
            {
                throw new ArgumentNullException(nameof(targetItem));
            }
            ValidateAndConnectTo(filePath);

            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = targetItem.HydraulicBoundaryDatabase;
            if (!IsImportRequired(hydraulicBoundaryDatabase))
            {
                bool isNotificationRequired = hydraulicBoundaryDatabase.FilePath != filePath;
                hydraulicBoundaryDatabase.FilePath = filePath;
                if (isNotificationRequired)
                {
                    targetItem.NotifyObservers();
                }
            }
            else
            {
                ReadHydraulicBoundaryDatabase readResult = ReadHydraulicBoundaryDatabase();

                if (readResult == null)
                {
                    return false;
                }

                AddImportedDataToModel(targetItem, readResult, filePath);
                log.Info(Resources.HydraulicBoundaryDatabaseImporter_Import_All_hydraulic_locations_read);
            }

            return true;
        }

        public void Dispose()
        {
            if (hydraulicBoundaryDatabaseReader != null)
            {
                hydraulicBoundaryDatabaseReader.Dispose();
                hydraulicBoundaryDatabaseReader = null;
            }
            if (hydraulicLocationConfigurationDatabaseReader != null)
            {
                hydraulicLocationConfigurationDatabaseReader.Dispose();
                hydraulicLocationConfigurationDatabaseReader = null;
            }
        }

        /// <summary>
        /// Validates the file and opens a connection.
        /// </summary>
        /// <param name="filePath">The path to the file to read.</param>
        /// <exception cref="CriticalFileReadException">Thrown when: 
        /// <list type="bullet">
        /// <item>The given file at <paramref name="filePath"/> cannot be read.</item>
        /// <item>The file 'HLCD.sqlite' in the same folder as <paramref name="filePath"/> cannot be read.</item>
        /// </list>
        /// </exception>
        private void ValidateAndConnectTo(string filePath)
        {
            hydraulicBoundaryDatabaseReader = new HydraulicBoundaryDatabaseReader(filePath);

            string hlcdFilePath = Path.Combine(Path.GetDirectoryName(filePath), "hlcd.sqlite");
            try
            {
                hydraulicLocationConfigurationDatabaseReader = new HydraulicLocationConfigurationDatabaseReader(hlcdFilePath);
            }
            catch (CriticalFileReadException)
            {
                string message = new FileReaderErrorMessageBuilder(filePath).Build(Resources.HydraulicBoundaryDatabaseImporter_HLCD_sqlite_Not_Found);
                throw new CriticalFileReadException(message);
            }
            try
            {
                string settingsDatabaseFileName = HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(filePath);
                using (new DesignTablesSettingsProvider(settingsDatabaseFileName)) {}
                using (new TimeIntegrationSettingsProvider(settingsDatabaseFileName)) {}
                using (new NumericsSettingsProvider(settingsDatabaseFileName)) {}
            }
            catch (CriticalFileReadException e)
            {
                string errorMessage = string.Format(Resources.HydraulicBoundaryDatabaseImporter_Cannot_open_hydaulic_calculation_settings_file_0_,
                                                    e.Message);
                string message = new FileReaderErrorMessageBuilder(filePath).Build(errorMessage);
                throw new CriticalFileReadException(message);
            }
        }

        private bool IsImportRequired(HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            return hydraulicBoundaryDatabase == null || hydraulicBoundaryDatabaseReader.GetVersion() != hydraulicBoundaryDatabase.Version;
        }

        private void HandleException(Exception e)
        {
            log.Error(e.Message, e);
        }

        private ReadHydraulicBoundaryDatabase ReadHydraulicBoundaryDatabase()
        {
            long trackId = GetTrackId();
            if (trackId == 0)
            {
                return null;
            }

            try
            {
                string version = hydraulicBoundaryDatabaseReader.GetVersion();
                bool canUsePreprocessor = hydraulicLocationConfigurationDatabaseReader.GetCanUsePreprocessorByTrackId(trackId);
                var hydraulicBoundaryLocations = new List<HydraulicBoundaryLocation>();

                Dictionary<long, long> locationIdsDictionary = hydraulicLocationConfigurationDatabaseReader.GetLocationIdsByTrackId(trackId);
                var filter = new HydraulicBoundaryLocationFilter(HydraulicBoundaryDatabaseHelper.GetHydraulicBoundarySettingsDatabase(hydraulicBoundaryDatabaseReader.Path));

                hydraulicBoundaryDatabaseReader.PrepareReadLocation();

                while (hydraulicBoundaryDatabaseReader.HasNext)
                {
                    ReadHydraulicBoundaryLocation location = hydraulicBoundaryDatabaseReader.ReadLocation();

                    long locationId;
                    locationIdsDictionary.TryGetValue(location.Id, out locationId);

                    if (filter.ShouldInclude(locationId))
                    {
                        hydraulicBoundaryLocations.Add(new HydraulicBoundaryLocation(locationId,
                                                                                     location.Name,
                                                                                     location.CoordinateX,
                                                                                     location.CoordinateY));
                    }
                }
                return new ReadHydraulicBoundaryDatabase(version, hydraulicBoundaryLocations, canUsePreprocessor);
            }
            catch (Exception e) when (e is LineParseException || e is CriticalFileReadException)
            {
                HandleException(e);
                return null;
            }
        }

        private long GetTrackId()
        {
            try
            {
                return hydraulicBoundaryDatabaseReader.GetTrackId();
            }
            catch (Exception e)
            {
                if (e is LineParseException || e is CriticalFileReadException)
                {
                    HandleException(e);
                    return 0;
                }
                throw;
            }
        }

        private static void AddImportedDataToModel(IAssessmentSection assessmentSection, ReadHydraulicBoundaryDatabase readData, string filePath)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = readData.CanUsePreprocessor
                                                                      ? new HydraulicBoundaryDatabase(true, Path.GetDirectoryName(filePath))
                                                                      : new HydraulicBoundaryDatabase();

            hydraulicBoundaryDatabase.Locations.AddRange(readData.Locations);
            hydraulicBoundaryDatabase.Version = readData.Version;
            hydraulicBoundaryDatabase.FilePath = filePath;

            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            assessmentSection.NotifyObservers();
        }
    }
}