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
using Core.Common.Base.Geometry;
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.DuneErosion
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="DuneLocation"/> based on the
    /// <see cref="DuneLocationEntity"/>.
    /// </summary>
    internal static class DuneLocationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="DuneLocationEntity"/> and use the information to construct a <see cref="DuneLocation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationEntity"/> to create <see cref="DuneLocation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="DuneLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static DuneLocation Read(this DuneLocationEntity entity, ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var duneLocation = new DuneLocation(entity.HydraulicLocationEntity.Read(collector),
                                                entity.Name,
                                                new Point2D(entity.LocationX.ToNullAsNaN(), entity.LocationY.ToNullAsNaN()),
                                                new DuneLocation.ConstructionProperties
                                                {
                                                    CoastalAreaId = entity.CoastalAreaId,
                                                    Offset = entity.Offset.ToNullAsNaN(),
                                                    Orientation = entity.Orientation.ToNullAsNaN(),
                                                    D50 = entity.D50.ToNullAsNaN()
                                                });

            collector.Read(entity, duneLocation);
            return duneLocation;
        }
    }
}