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
using Core.Common.Util.Extensions;
using Riskeer.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.DuneErosion
{
    /// <summary>
    /// Extension methods for <see cref="DuneLocation"/> related to creating a <see cref="DuneLocationEntity"/>.
    /// </summary>
    internal static class DuneLocationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="DuneLocationEntity"/> based on the information of the <see cref="DuneLocation"/>.
        /// </summary>
        /// <param name="location">The location to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="DuneLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static DuneLocationEntity Create(this DuneLocation location, PersistenceRegistry registry, int order)
        {
            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(location))
            {
                return registry.Get(location);
            }

            var entity = new DuneLocationEntity
            {
                LocationId = location.Id,
                Name = location.Name.DeepClone(),
                LocationX = location.Location.X.ToNaNAsNull(),
                LocationY = location.Location.Y.ToNaNAsNull(),
                CoastalAreaId = location.CoastalAreaId,
                Offset = location.Offset.ToNaNAsNull(),
                Orientation = location.Orientation.ToNaNAsNull(),
                D50 = location.D50.ToNaNAsNull(),
                Order = order
            };

            registry.Register(entity, location);
            return entity;
        }
    }
}