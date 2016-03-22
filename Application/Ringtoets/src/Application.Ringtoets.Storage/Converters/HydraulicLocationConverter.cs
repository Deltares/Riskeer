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
using Core.Common.Base.Geometry;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    ///  Converter for <see cref="HydraulicLocationEntity"/> to <see cref="HydraulicBoundaryLocation"/>
    /// and <see cref="HydraulicBoundaryLocation"/> to <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    public class HydraulicLocationConverter : IEntityConverter<HydraulicBoundaryLocation, HydraulicLocationEntity>
    {
        public HydraulicBoundaryLocation ConvertEntityToModel(HydraulicLocationEntity entity, Func<HydraulicBoundaryLocation> model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            try
            {
                if (model() == null)
                {
                    throw new ArgumentNullException("model");
                }
            }
            catch (NullReferenceException)
            {
                throw new ArgumentNullException("model");
            }

            HydraulicBoundaryLocation hydraulicBoundaryLocation = model();
            hydraulicBoundaryLocation.Id = entity.LocationId;
            hydraulicBoundaryLocation.StorageId = entity.HydraulicLocationEntityId;
            hydraulicBoundaryLocation.Name = entity.Name;
            hydraulicBoundaryLocation.Location = new Point2D(Convert.ToDouble(entity.LocationX), Convert.ToDouble(entity.LocationY));

            if (entity.DesignWaterLevel.HasValue)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = (double)entity.DesignWaterLevel;
            }

            return hydraulicBoundaryLocation;
        }

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
            entity.DesignWaterLevel = modelObject.DesignWaterLevel;
            entity.LocationId = modelObject.Id;

            if (modelObject.Location != null)
            {
                entity.LocationX = Convert.ToDecimal(modelObject.Location.X);
                entity.LocationY = Convert.ToDecimal(modelObject.Location.Y);
            }
        }
    }
}