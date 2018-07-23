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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsSoilProfile2D"/> 
    /// based on the <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilProfileTwoDEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> and use the information 
        /// to construct a <see cref="MacroStabilityInwardsSoilProfile2D"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> to 
        /// create <see cref="MacroStabilityInwardsSoilProfile2D"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfile2D"/> or one from the <paramref name="collector"/> 
        /// if the <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> has been read before.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilProfile2D Read(this MacroStabilityInwardsSoilProfileTwoDEntity entity,
                                                              ReadConversionCollector collector)
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

            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = entity.MacroStabilityInwardsSoilLayerTwoDEntities
                                                                         .OrderBy(sl => sl.Order)
                                                                         .Select(sl => sl.Read())
                                                                         .ToArray();
            IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses = entity.MacroStabilityInwardsPreconsolidationStressEntities
                                                                                                      .OrderBy(stressEntity => stressEntity.Order)
                                                                                                      .Select(stressEntity => stressEntity.Read())
                                                                                                      .ToArray();
            var soilProfile = new MacroStabilityInwardsSoilProfile2D(entity.Name,
                                                                     layers,
                                                                     preconsolidationStresses);

            collector.Read(entity, soilProfile);
            return soilProfile;
        }
    }
}