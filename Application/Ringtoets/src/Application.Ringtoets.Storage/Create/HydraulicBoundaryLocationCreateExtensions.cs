﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="HydraulicBoundaryLocation"/> related to creating a <see cref="HydraulicLocationEntity"/>.
    /// </summary>
    internal static class HydraulicBoundaryLocationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HydraulicLocationEntity"/> based on the information of the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="location">The location to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="HydraulicLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static HydraulicLocationEntity Create(this HydraulicBoundaryLocation location, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(location))
            {
                return registry.Get<HydraulicLocationEntity>(location);
            }

            var entity = new HydraulicLocationEntity
            {
                LocationId = location.Id,
                Name = location.Name.DeepClone(),
                LocationX = location.Location.X.ToNaNAsNull(),
                LocationY = location.Location.Y.ToNaNAsNull(),
                DesignWaterLevel = double.IsNaN(location.DesignWaterLevel) ? (double?) null : location.DesignWaterLevel,
                WaveHeight = double.IsNaN(location.WaveHeight) ? (double?) null : location.WaveHeight,
                DesignWaterLevelCalculationConvergence = (byte) location.DesignWaterLevelCalculationConvergence,
                WaveHeightCalculationConvergence = (byte) location.WaveHeightCalculationConvergence,
                Order = order
            };

            registry.Register(entity, location);
            return entity;
        }

        /// <summary>
        /// Creates a <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> based on the information of the <see cref="HydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="location">The location to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="HydraulicLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionOutwardsHydraulicLocationEntity CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(
            this HydraulicBoundaryLocation location, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(location))
            {
                return registry.Get<GrassCoverErosionOutwardsHydraulicLocationEntity>(location);
            }

            var entity = new GrassCoverErosionOutwardsHydraulicLocationEntity
            {
                LocationId = location.Id,
                Name = location.Name.DeepClone(),
                LocationX = location.Location.X.ToNaNAsNull(),
                LocationY = location.Location.Y.ToNaNAsNull(),
                DesignWaterLevel = double.IsNaN(location.DesignWaterLevel) ? (double?)null : location.DesignWaterLevel,
                WaveHeight = double.IsNaN(location.WaveHeight) ? (double?)null : location.WaveHeight,
                DesignWaterLevelCalculationConvergence = (byte)location.DesignWaterLevelCalculationConvergence,
                WaveHeightCalculationConvergence = (byte)location.WaveHeightCalculationConvergence,
                Order = order
            };

            registry.Register(entity, location);
            return entity;
        }
    }
}