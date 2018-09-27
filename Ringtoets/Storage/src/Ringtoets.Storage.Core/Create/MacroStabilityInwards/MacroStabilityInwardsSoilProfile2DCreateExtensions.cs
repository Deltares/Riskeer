// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsSoilProfile2D"/> related to creating 
    /// a <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilProfile2DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> based on the information 
        /// of the <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <param name="soilProfile">The soil profile to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> or one from the 
        /// <paramref name="registry"/> if it was created for the <see cref="soilProfile"/> earlier.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilProfileTwoDEntity Create(this MacroStabilityInwardsSoilProfile2D soilProfile,
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

            var entity = new MacroStabilityInwardsSoilProfileTwoDEntity
            {
                Name = soilProfile.Name.DeepClone()
            };

            AddEntitiesForSoilLayers(soilProfile.Layers, entity);
            AddEntitiesForPreconsolidationStresses(soilProfile.PreconsolidationStresses, entity);

            registry.Register(entity, soilProfile);
            return entity;
        }

        private static void AddEntitiesForSoilLayers(IEnumerable<MacroStabilityInwardsSoilLayer2D> layers,
                                                     MacroStabilityInwardsSoilProfileTwoDEntity entity)
        {
            var index = 0;
            foreach (MacroStabilityInwardsSoilLayer2D layer in layers)
            {
                entity.MacroStabilityInwardsSoilLayerTwoDEntities.Add(layer.Create(index++));
            }
        }

        private static void AddEntitiesForPreconsolidationStresses(IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses,
                                                                   MacroStabilityInwardsSoilProfileTwoDEntity entity)
        {
            var index = 0;
            foreach (MacroStabilityInwardsPreconsolidationStress preconsolidationStress in preconsolidationStresses)
            {
                entity.MacroStabilityInwardsPreconsolidationStressEntities.Add(preconsolidationStress.Create(index++));
            }
        }
    }
}