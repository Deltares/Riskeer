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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="StochasticSoilProfile"/> based on the
    /// <see cref="StochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class StochasticSoilProfileEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="StochasticSoilProfileEntity"/> and use the information to construct a <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> to create <see cref="StochasticSoilProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="StochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static StochasticSoilProfile Read(this StochasticSoilProfileEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var profile = new StochasticSoilProfile(entity.Probability, (SoilProfileType) entity.Type, -1);
            entity.ReadSoilProfile(profile, collector);

            collector.Read(entity, profile);

            return profile;
        }

        private static void ReadSoilProfile(this StochasticSoilProfileEntity entity, StochasticSoilProfile profile, ReadConversionCollector collector)
        {
            profile.SoilProfile = entity.SoilProfileEntity.Read(collector);
        }
    }
}