// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
    /// <see cref="HydraulicBoundaryDataEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryDataCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicBoundaryDataEntity"/> based on the information of the
        /// <see cref="HydraulicBoundaryData"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryData">The <see cref="HydraulicBoundaryData"/> to create a
        /// <see cref="HydraulicBoundaryDataEntity"/> for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="HydraulicBoundaryDataEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryData"/>
        /// or <paramref name="registry"/> is <c>null</c>.</exception>
        public static HydraulicBoundaryDataEntity Create(this HydraulicBoundaryData hydraulicBoundaryData, PersistenceRegistry registry)
        {
            if (hydraulicBoundaryData == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryData));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;

            var entity = new HydraulicBoundaryDataEntity
            {
                HydraulicLocationConfigurationDatabaseFilePath = hydraulicLocationConfigurationDatabase.FilePath.DeepClone(),
                HydraulicLocationConfigurationDatabaseScenarioName = hydraulicLocationConfigurationDatabase.ScenarioName.DeepClone(),
                HydraulicLocationConfigurationDatabaseYear = hydraulicLocationConfigurationDatabase.Year,
                HydraulicLocationConfigurationDatabaseScope = hydraulicLocationConfigurationDatabase.Scope.DeepClone(),
                HydraulicLocationConfigurationDatabaseSeaLevel = hydraulicLocationConfigurationDatabase.SeaLevel.DeepClone(),
                HydraulicLocationConfigurationDatabaseRiverDischarge = hydraulicLocationConfigurationDatabase.RiverDischarge.DeepClone(),
                HydraulicLocationConfigurationDatabaseLakeLevel = hydraulicLocationConfigurationDatabase.LakeLevel.DeepClone(),
                HydraulicLocationConfigurationDatabaseWindDirection = hydraulicLocationConfigurationDatabase.WindDirection.DeepClone(),
                HydraulicLocationConfigurationDatabaseWindSpeed = hydraulicLocationConfigurationDatabase.WindSpeed.DeepClone(),
                HydraulicLocationConfigurationDatabaseComment = hydraulicLocationConfigurationDatabase.Comment.DeepClone()
            };

            for (var i = 0; i < hydraulicBoundaryData.HydraulicBoundaryDatabases.Count; i++)
            {
                HydraulicBoundaryDatabase hydraulicBoundaryDatabase = hydraulicBoundaryData.HydraulicBoundaryDatabases[i];
                entity.HydraulicBoundaryDatabaseEntities.Add(hydraulicBoundaryDatabase.Create(registry, i));
            }

            return entity;
        }
    }
}