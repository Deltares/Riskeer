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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryData"/> related to creating a
    /// <see cref="HydraulicBoundaryDatabaseEntity"/>.
    /// </summary>
    public static class HydraulicLocationConfigurationSettingsCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicBoundaryDatabaseEntity"/> based on the information of the
        /// <see cref="HydraulicBoundaryData"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to create a
        /// <see cref="HydraulicBoundaryDatabaseEntity"/> for.</param>
        /// <returns>A new <see cref="HydraulicBoundaryDatabaseEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryData"/> is <c>null</c>.</exception>
        internal static HydraulicBoundaryDatabaseEntity Create(this HydraulicBoundaryData hydraulicBoundaryData)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryData.HydraulicLocationConfigurationSettings;
            return new HydraulicBoundaryDatabaseEntity
            {
                FilePath = hydraulicBoundaryData.FilePath.DeepClone(),
                Version = hydraulicBoundaryData.Version.DeepClone(),
                HydraulicLocationConfigurationSettingsFilePath = settings.FilePath.DeepClone(),
                HydraulicLocationConfigurationSettingsUsePreprocessorClosure = Convert.ToByte(settings.UsePreprocessorClosure),
                HydraulicLocationConfigurationSettingsScenarioName = settings.ScenarioName.DeepClone(),
                HydraulicLocationConfigurationSettingsYear = settings.Year,
                HydraulicLocationConfigurationSettingsScope = settings.Scope.DeepClone(),
                HydraulicLocationConfigurationSettingsSeaLevel = settings.SeaLevel.DeepClone(),
                HydraulicLocationConfigurationSettingsRiverDischarge = settings.RiverDischarge.DeepClone(),
                HydraulicLocationConfigurationSettingsLakeLevel = settings.LakeLevel.DeepClone(),
                HydraulicLocationConfigurationSettingsWindDirection = settings.WindDirection.DeepClone(),
                HydraulicLocationConfigurationSettingsWindSpeed = settings.WindSpeed.DeepClone(),
                HydraulicLocationConfigurationSettingsComment = settings.Comment.DeepClone()
            };
        }
    }
}