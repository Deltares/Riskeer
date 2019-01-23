// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsStochasticSoilProfile"/> related to 
    /// creating a <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsStochasticSoilProfileCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> based on 
        /// the information of the <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="stochasticSoilProfile">The stochastic soil profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="MacroStabilityInwardsStochasticSoilProfile.SoilProfile"/> is 
        /// not of type <see cref="MacroStabilityInwardsSoilProfile1D"/> or <see cref="MacroStabilityInwardsSoilProfile2D"/>.</exception>
        public static MacroStabilityInwardsStochasticSoilProfileEntity Create(this MacroStabilityInwardsStochasticSoilProfile stochasticSoilProfile,
                                                                              PersistenceRegistry registry,
                                                                              int order)
        {
            if (stochasticSoilProfile == null)
            {
                throw new ArgumentNullException(nameof(stochasticSoilProfile));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(stochasticSoilProfile))
            {
                return registry.Get(stochasticSoilProfile);
            }

            var entity = new MacroStabilityInwardsStochasticSoilProfileEntity
            {
                Probability = stochasticSoilProfile.Probability,
                Order = order
            };

            AddEntityForProfile(stochasticSoilProfile.SoilProfile, entity, registry);

            registry.Register(entity, stochasticSoilProfile);
            return entity;
        }

        /// <summary>
        /// Adds the entity representation of <paramref name="soilProfile"/> to the <paramref name="entity"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to store.</param>
        /// <param name="entity">The entity to update.</param>
        /// <param name="registry">The registry to use for persisting entities.</param>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="soilProfile"/> is 
        /// not of type <see cref="MacroStabilityInwardsSoilProfile1D"/> or <see cref="MacroStabilityInwardsSoilProfile2D"/>.</exception>
        private static void AddEntityForProfile(IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile,
                                                MacroStabilityInwardsStochasticSoilProfileEntity entity,
                                                PersistenceRegistry registry)
        {
            var soilProfile1D = soilProfile as MacroStabilityInwardsSoilProfile1D;
            if (soilProfile1D != null)
            {
                entity.MacroStabilityInwardsSoilProfileOneDEntity = soilProfile1D.Create(registry);
                return;
            }

            var soilProfile2D = soilProfile as MacroStabilityInwardsSoilProfile2D;
            if (soilProfile2D != null)
            {
                entity.MacroStabilityInwardsSoilProfileTwoDEntity = soilProfile2D.Create(registry);
                return;
            }

            string exceptionMessage = $"{soilProfile.GetType().Name} is not supported. " +
                                      $"Supported types are: {nameof(MacroStabilityInwardsSoilProfile1D)} and {nameof(MacroStabilityInwardsSoilProfile2D)}.";
            throw new NotSupportedException(exceptionMessage);
        }
    }
}