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
using System.Collections.Generic;
using Core.Common.Util.Extensions;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsSoilProfile1D"/> related to creating 
    /// a <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilProfile1DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> based on the information 
        /// of the <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> or one from the 
        /// <paramref name="registry"/> if it was created for the <see cref="soilProfile"/> earlier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilProfileOneDEntity Create(this MacroStabilityInwardsSoilProfile1D soilProfile,
                                                                        PersistenceRegistry registry)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(soilProfile))
            {
                return registry.Get(soilProfile);
            }

            var entity = new MacroStabilityInwardsSoilProfileOneDEntity
            {
                Name = soilProfile.Name.DeepClone(),
                Bottom = soilProfile.Bottom.ToNaNAsNull()
            };

            AddEntitiesForSoilLayers(soilProfile.Layers, entity);

            registry.Register(entity, soilProfile);
            return entity;
        }

        private static void AddEntitiesForSoilLayers(IEnumerable<MacroStabilityInwardsSoilLayer1D> layers,
                                                     MacroStabilityInwardsSoilProfileOneDEntity entity)
        {
            var index = 0;
            foreach (MacroStabilityInwardsSoilLayer1D layer in layers)
            {
                entity.MacroStabilityInwardsSoilLayerOneDEntities.Add(layer.Create(index++));
            }
        }
    }
}