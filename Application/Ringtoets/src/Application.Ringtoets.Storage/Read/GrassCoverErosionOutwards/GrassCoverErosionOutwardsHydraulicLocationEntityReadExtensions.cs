// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionOutwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="HydraulicBoundaryLocation"/> based on the
    /// <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsHydraulicLocationEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="HydraulicLocationEntity"/> and use the information to construct a <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> to create <see cref="HydraulicBoundaryLocation"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="HydraulicBoundaryLocation"/>.</returns>
        internal static HydraulicBoundaryLocation Read(this GrassCoverErosionOutwardsHydraulicLocationEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(
                entity.LocationId,
                entity.Name,
                entity.LocationX.ToNullAsNaN(),
                entity.LocationY.ToNullAsNaN());

            IHydraulicLocationOutputEntity designWaterLevelOutputEntity = GetGrassCoverErosionOutwardsHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.DesignWaterLevel);
            if (designWaterLevelOutputEntity != null)
            {
                hydraulicBoundaryLocation.DesignWaterLevelOutput = designWaterLevelOutputEntity.Read();
            }

            IHydraulicLocationOutputEntity waveHeightOutputEntity = GetGrassCoverErosionOutwardsHydraulicLocationOutputEntity(entity, HydraulicLocationOutputType.WaveHeight);
            if (waveHeightOutputEntity != null)
            {
                hydraulicBoundaryLocation.WaveHeightOutput = waveHeightOutputEntity.Read();
            }

            collector.Read(entity, hydraulicBoundaryLocation);

            return hydraulicBoundaryLocation;
        }

        private static IHydraulicLocationOutputEntity GetGrassCoverErosionOutwardsHydraulicLocationOutputEntity(
            GrassCoverErosionOutwardsHydraulicLocationEntity entity, HydraulicLocationOutputType outputType)
        {
            return entity.GrassCoverErosionOutwardsHydraulicLocationOutputEntities.SingleOrDefault(
                e => e.HydraulicLocationOutputType == (byte) outputType);
        }
    }
}