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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.IO;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Riskeer.Common.IO.HydraRing
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
        /// Gets <see cref="PreprocessorSetting"/> based on the provided location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="PreprocessorSetting"/> for.</param>
        /// <param name="usePreprocessor">Indicator whether the preprocessor must be taken into account.</param>
        /// <returns>The <see cref="PreprocessorSetting"/>.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        public PreprocessorSetting GetPreprocessorSetting(long locationId, bool usePreprocessor)
        {
            if (!usePreprocessor || LocationExcluded(locationId))
            {
                return new PreprocessorSetting();
            }

            ReadPreprocessorSetting readSetting = preprocessorSettingsReader.ReadPreprocessorSetting(locationId) ?? defaultPreprocessorSetting;

            return new PreprocessorSetting(readSetting.ValueMin, readSetting.ValueMax, GetNumericsSetting(locationId));
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                preprocessorSettingsReader.Dispose();
            }
        }

        /// <summary>
        /// Checks whether the location corresponding to the provided id is an excluded location.
        /// </summary>
        /// <param name="locationId">The location id to check exclusion for.</param>
        /// <returns><c>true</c> when <paramref name="locationId"/> is excluded; <c>false</c> otherwise.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        private bool LocationExcluded(long locationId)
        {
            IEnumerable<long> excludedIds = preprocessorSettingsReader.ReadExcludedPreprocessorLocations();

            return excludedIds.Contains(locationId);
        }

        /// <summary>
        /// Returns <see cref="NumericsSetting"/> based on the provided location id.
        /// </summary>
        /// <param name="locationId">The location id to obtain the <see cref="NumericsSetting"/> for.</param>
        /// <returns>A new <see cref="NumericsSetting"/> containing values corresponding to the provided location id.</returns>
        /// <exception cref="CriticalFileReadException">Thrown when a column that is being read doesn't
        /// contain expected type.</exception>
        private NumericsSetting GetNumericsSetting(long locationId)
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