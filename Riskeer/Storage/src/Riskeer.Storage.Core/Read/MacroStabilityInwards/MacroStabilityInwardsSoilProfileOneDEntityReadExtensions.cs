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
using System.Linq;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsSoilProfile1D"/> 
    /// based on the <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSoilProfileOneDEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> and use the information 
        /// to construct a <see cref="MacroStabilityInwardsSoilProfile1D"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> to 
        /// create <see cref="MacroStabilityInwardsSoilProfile1D"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSoilProfile1D"/> or one from the <paramref name="collector"/> 
        /// if the <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> has been read before.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsSoilProfile1D Read(this MacroStabilityInwardsSoilProfileOneDEntity entity,
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

            IEnumerable<MacroStabilityInwardsSoilLayer1D> layers = entity.MacroStabilityInwardsSoilLayerOneDEntities.OrderBy(sl => sl.Order)
                                                                         .Select(sl => sl.Read())
                                                                         .ToArray();
            var macroStabilityInwardsSoilProfile = new MacroStabilityInwardsSoilProfile1D(entity.Name,
                                                                                          entity.Bottom.ToNullAsNaN(),
                                                                                          layers);

            collector.Read(entity, macroStabilityInwardsSoilProfile);
            return macroStabilityInwardsSoilProfile;
        }
    }
}