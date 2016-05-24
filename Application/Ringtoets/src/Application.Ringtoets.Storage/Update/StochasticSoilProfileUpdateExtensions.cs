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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="StochasticSoilProfile"/> related to updating a <see cref="StochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class StochasticSoilProfileUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="StochasticSoilProfileEntity"/> in the database based on the information of the 
        /// <see cref="StochasticSoilProfile"/>.
        /// </summary>
        /// <param name="profile">The profile to update the database entity for.</param>
        /// <param name="collector">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        internal static void Update(this StochasticSoilProfile profile, PersistenceRegistry collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleStochasticSoilProfile(profile, context);
            entity.Probability = Convert.ToDecimal(profile.Probability);

            if (profile.SoilProfile.IsNew())
            {
                entity.SoilProfileEntity = profile.SoilProfile.Create(collector);
            }
            else
            {
                profile.SoilProfile.Update(collector, context);
            }

            collector.Register(entity, profile);
        }

        private static StochasticSoilProfileEntity GetSingleStochasticSoilProfile(StochasticSoilProfile profile, IRingtoetsEntities context)
        {
            try
            {
                return context.StochasticSoilProfileEntities.Single(sspe => sspe.StochasticSoilProfileEntityId == profile.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(StochasticSoilProfileEntity).Name, profile.StorageId), exception);
            }
        }
    }
}