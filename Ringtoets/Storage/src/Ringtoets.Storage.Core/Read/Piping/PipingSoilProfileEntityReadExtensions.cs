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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingSoilProfile"/> based on the
    /// <see cref="PipingSoilProfileEntity"/>.
    /// </summary>
    internal static class PipingSoilProfileEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="PipingSoilProfileEntity"/> and use the information to construct a <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilProfileEntity"/> to create <see cref="PipingSoilProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingSoilProfile"/> or one from the <paramref name="collector"/> if the 
        /// <see cref="PipingSoilProfileEntity"/> has been read before.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static PipingSoilProfile Read(this PipingSoilProfileEntity entity, ReadConversionCollector collector)
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

            IEnumerable<PipingSoilLayer> layers = entity.PipingSoilLayerEntities.OrderBy(sl => sl.Order)
                                                        .Select(sl => sl.Read())
                                                        .ToArray();
            var pipingSoilProfile = new PipingSoilProfile(entity.Name,
                                                          entity.Bottom.ToNullAsNaN(),
                                                          layers,
                                                          (SoilProfileType) entity.SourceType);

            collector.Read(entity, pipingSoilProfile);
            return pipingSoilProfile;
        }
    }
}