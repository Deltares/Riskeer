// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines  extension methods for read operations for a <see cref="HydraulicBoundaryDatabaseEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryDatabaseEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HydraulicBoundaryDatabaseEntity"/> and uses the information to
        /// update a <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicBoundaryDatabaseEntity"/> to update the
        /// <see cref="HydraulicBoundaryDatabase"/>.</param>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to update.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this HydraulicBoundaryDatabaseEntity entity, HydraulicBoundaryDatabase hydraulicBoundaryDatabase)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            hydraulicBoundaryDatabase.FilePath = entity.FilePath;
            hydraulicBoundaryDatabase.Version = entity.Version;

            hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings.SetValues(
                entity.HydraulicLocationConfigurationSettingsFilePath,
                entity.HydraulicLocationConfigurationSettingsScenarioName,
                entity.HydraulicLocationConfigurationSettingsYear,
                entity.HydraulicLocationConfigurationSettingsScope,
                Convert.ToBoolean(entity.HydraulicLocationConfigurationSettingsUsePreprocessorClosure),
                entity.HydraulicLocationConfigurationSettingsSeaLevel,
                entity.HydraulicLocationConfigurationSettingsRiverDischarge,
                entity.HydraulicLocationConfigurationSettingsLakeLevel,
                entity.HydraulicLocationConfigurationSettingsWindDirection,
                entity.HydraulicLocationConfigurationSettingsWindSpeed,
                entity.HydraulicLocationConfigurationSettingsComment);
        }
    }
}