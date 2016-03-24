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
using System.Collections.Generic;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    ///  Converter for <see cref="HydraulicLocationEntity"/> to <see cref="HydraulicBoundaryLocation"/>
    /// and <see cref="HydraulicBoundaryLocation"/> to <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    public class HydraulicLocationConverter // : IEntityConverter<HydraulicBoundaryLocation, HydraulicLocationEntity>
    {
        /// <summary>
        /// Converts <paramref name="entities"/> to an <see cref="ICollection{T}"/> of <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="entities">The <see cref="ICollection{T}"/> of <see cref="HydraulicLocationEntity"/> to convert.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="HydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entities"/> is <c>null</c>.</exception>
        public IEnumerable<HydraulicBoundaryLocation> ConvertEntityToModel(ICollection<HydraulicLocationEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException("entities");
            }

            foreach (var entity in entities)
            {
                HydraulicBoundaryLocation hydraulicBoundaryLocation = new HydraulicBoundaryLocation(entity.LocationId, entity.Name, Convert.ToDouble(entity.LocationX), Convert.ToDouble(entity.LocationY))
                {
                    StorageId = entity.HydraulicLocationEntityId,
                };

                if (entity.DesignWaterLevel.HasValue)
                {
                    hydraulicBoundaryLocation.DesignWaterLevel = (double)entity.DesignWaterLevel;
                }

                yield return hydraulicBoundaryLocation;
            }
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="HydraulicBoundaryLocation"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="HydraulicLocationEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="modelObject"/> or <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="OverflowException">Thrown when <paramref name="modelObject.Location"/> cannot be converted.</exception>
        public void ConvertModelToEntity(HydraulicBoundaryLocation modelObject, HydraulicLocationEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }

            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            entity.HydraulicLocationEntityId = modelObject.StorageId;
            entity.Name = modelObject.Name;
            entity.LocationId = modelObject.Id;

            if (!double.IsNaN(modelObject.DesignWaterLevel))
            {
                entity.DesignWaterLevel = modelObject.DesignWaterLevel;
            }

            if (modelObject.Location != null)
            {
                entity.LocationX = Convert.ToDecimal(modelObject.Location.X);
                entity.LocationY = Convert.ToDecimal(modelObject.Location.Y);
            }
        }
    }
}