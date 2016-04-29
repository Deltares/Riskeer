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
using Application.Ringtoets.Storage.Create;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryLocation"/> related to creating a <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    public static class HydraulicBoundaryLocationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicLocationEntity"/> based on the information of the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="location">The location to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="HydraulicLocationEntity"/>.</returns>
        public static HydraulicLocationEntity Create(this HydraulicBoundaryLocation location, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new HydraulicLocationEntity
            {
                LocationId = location.Id,
                Name = location.Name,
                LocationX = Convert.ToDecimal(location.Location.X),
                LocationY = Convert.ToDecimal(location.Location.Y),
                DesignWaterLevel = Double.IsNaN(location.DesignWaterLevel) ? (double?) null : Convert.ToDouble(location.DesignWaterLevel)
            };

            collector.Create(entity, location);
            return entity;
        }
    }
}