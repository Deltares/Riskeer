// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    /// Extension methods for <see cref="HydraulicBoundaryDatabase"/> related to creating a
    /// <see cref="HydraulicBoundaryDatabaseEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryDatabaseCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicBoundaryDatabaseEntity"/> based on the information of the
        /// <see cref="HydraulicBoundaryDatabase"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryDatabase">The <see cref="HydraulicBoundaryDatabase"/> to create a
        /// <see cref="HydraulicBoundaryDatabaseEntity"/> for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="HydraulicBoundaryDatabaseEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryDatabase"/>
        /// or <paramref name="registry"/> is <c>null</c>.</exception>
        public static HydraulicBoundaryDatabaseEntity Create(this HydraulicBoundaryDatabase hydraulicBoundaryDatabase,
                                                             PersistenceRegistry registry, int order)
        {
            if (hydraulicBoundaryDatabase == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryDatabase));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = hydraulicBoundaryDatabase.FilePath.DeepClone(),
                Version = hydraulicBoundaryDatabase.Version.DeepClone(),
                UsePreprocessorClosure = Convert.ToByte(hydraulicBoundaryDatabase.UsePreprocessorClosure),
                Order = order
            };

            for (var i = 0; i < hydraulicBoundaryDatabase.Locations.Count; i++)
            {
                HydraulicBoundaryLocation location = hydraulicBoundaryDatabase.Locations[i];
                entity.HydraulicLocationEntities.Add(location.Create(registry, i));
            }

            return entity;
        }
    }
}