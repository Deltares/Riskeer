﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.IO;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Readers;

namespace Ringtoets.Common.IO.HydraRing
{
    /// <summary>
    /// Provider of <see cref="PreprocessorSetting"/> by reading from the settings database or by resorting
    /// to defaults if no settings could be found.
    /// </summary>
    public class PreprocessorSettingsProvider : IDisposable
    {
        private readonly string databaseFilePath;
        private readonly HydraRingSettingsDatabaseReader preprocessorSettingsReader;
        private ReadPreprocessorSetting defaultPreprocessorSetting;

        /// <summary>
        /// Creates a new instance of the <see cref="PreprocessorSettingsProvider"/> class.
        /// </summary>
        /// <param name="databaseFilePath">The full path to the database file to use when reading settings.</param>
        /// <exception cref="CriticalFileReadException">Thrown when:
        /// <list type="bullet">
        /// <item>The <paramref name="databaseFilePath"/> contains invalid characters.</item>
        /// <item>No file could be found at <paramref name="databaseFilePath"/>.</item>
        /// <item>Unable to open database file.</item>
        /// <item>The opened database doesn't have the expected schema.</item>
        /// </list>
        /// </exception>
        public PreprocessorSettingsProvider(string databaseFilePath)
        {
            this.databaseFilePath = databaseFilePath;
            preprocessorSettingsReader = new HydraRingSettingsDatabaseReader(databaseFilePath);

            InitializeDefaultPreprocessorSettings();
        }

        /// <summary>
        /// Gets <see cref="PreprocessorSetting"/> based on the provided failure mechanism type and location id.
        /// </summary>
        /// <returns>The <see cref="PreprocessorSetting"/> .</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public PreprocessorSetting GetPreprocessorSetting(long locationId, bool usePreprocessor)
        {
            if (!usePreprocessor || LocationExcluded(locationId))
            {
                return new PreprocessorSetting();
            }

            ReadPreprocessorSetting readSetting = preprocessorSettingsReader.ReadPreprocessorSetting(locationId) ?? defaultPreprocessorSetting;

            return new PreprocessorSetting(readSetting.ValueMin, readSetting.ValueMax, GetNumericSetting(locationId));
        }

        public void Dispose()
        {
            preprocessorSettingsReader.Dispose();
        }

        private bool LocationExcluded(long locationId)
        {
            IEnumerable<long> excludedIds = preprocessorSettingsReader.ReadExcludedPreprocessorLocations();

            return excludedIds.Contains(locationId);
        }

        private NumericsSetting GetNumericSetting(long locationId)
        {
            using (var numericsSettingsProvider = new NumericsSettingsProvider(databaseFilePath))
            {
                return numericsSettingsProvider.GetNumericsSettingForPreprocessor(locationId);
            }
        }

        private void InitializeDefaultPreprocessorSettings()
        {
            defaultPreprocessorSetting = new ReadPreprocessorSetting(1, 6);
        }
    }
}