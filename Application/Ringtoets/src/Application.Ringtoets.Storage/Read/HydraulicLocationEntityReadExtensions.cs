// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="HydraulicBoundaryLocation"/> based on the
    /// <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    internal static class HydraulicLocationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="HydraulicLocationEntity"/> and use the information to construct a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> to create <see cref="HydraulicBoundaryLocation"/> for.</param>
        /// <returns>A new <see cref="HydraulicBoundaryLocation"/>.</returns>
        internal static HydraulicBoundaryLocation Read(this HydraulicLocationEntity entity)
        {
            HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(
                entity.LocationId,
                entity.Name,
                Convert.ToDouble(entity.LocationX),
                Convert.ToDouble(entity.LocationY))
            {
                StorageId = entity.HydraulicLocationEntityId
            };

            if (entity.DesignWaterLevel.HasValue)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = Convert.ToDouble(entity.DesignWaterLevel);
            }

            return hydraulicBoundaryLocation;
        }
    }
}