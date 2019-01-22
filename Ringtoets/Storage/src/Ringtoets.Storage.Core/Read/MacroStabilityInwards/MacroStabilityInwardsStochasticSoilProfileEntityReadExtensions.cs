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
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsStochasticSoilProfile"/> 
    /// based on the <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsStochasticSoilProfileEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> and use the information to 
        /// construct a <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> to create 
        /// <see cref="MacroStabilityInwardsStochasticSoilProfile"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public static MacroStabilityInwardsStochasticSoilProfile Read(this MacroStabilityInwardsStochasticSoilProfileEntity entity,
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

            var stochasticSoilProfile = new MacroStabilityInwardsStochasticSoilProfile(entity.Probability, entity.ReadSoilProfile(collector));

            collector.Read(entity, stochasticSoilProfile);

            return stochasticSoilProfile;
        }

        private static IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> ReadSoilProfile(this MacroStabilityInwardsStochasticSoilProfileEntity entity,
                                                                                                          ReadConversionCollector collector)
        {
            return entity.MacroStabilityInwardsSoilProfileOneDEntity != null
                       ? (IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer>) ReadSoilProfile1D(entity, collector)
                       : ReadSoilProfile2D(entity, collector);
        }

        private static IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer1D> ReadSoilProfile1D(this MacroStabilityInwardsStochasticSoilProfileEntity entity,
                                                                                                             ReadConversionCollector collector)
        {
            return entity.MacroStabilityInwardsSoilProfileOneDEntity.Read(collector);
        }

        private static IMacroStabilityInwardsSoilProfile<MacroStabilityInwardsSoilLayer2D> ReadSoilProfile2D(this MacroStabilityInwardsStochasticSoilProfileEntity entity,
                                                                                                             ReadConversionCollector collector)
        {
            return entity.MacroStabilityInwardsSoilProfileTwoDEntity.Read(collector);
        }
    }
}