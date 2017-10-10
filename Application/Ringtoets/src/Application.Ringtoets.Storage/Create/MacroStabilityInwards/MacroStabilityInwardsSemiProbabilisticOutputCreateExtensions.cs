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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/> related to creating 
    /// a <see cref="MacroStabilityInwardsSemiProbabilisticOutputEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsSemiProbabilisticOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsSemiProbabilisticOutputEntity"/> based on the information
        /// of the <see cref="MacroStabilityInwardsSemiProbabilisticOutput"/>.
        /// </summary>
        /// <param name="semiProbabilisticOutput">The semi-probabilistic calculation output
        /// for piping to create a database entity for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSemiProbabilisticOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="semiProbabilisticOutput"/>
        /// is <c>null</c>.</exception>
        public static MacroStabilityInwardsSemiProbabilisticOutputEntity Create(this MacroStabilityInwardsSemiProbabilisticOutput semiProbabilisticOutput)
        {
            if (semiProbabilisticOutput == null)
            {
                throw new ArgumentNullException(nameof(semiProbabilisticOutput));
            }

            return new MacroStabilityInwardsSemiProbabilisticOutputEntity
            {
                FactorOfStability = semiProbabilisticOutput.FactorOfStability.ToNaNAsNull(),
                MacroStabilityInwardsFactorOfSafety = semiProbabilisticOutput.MacroStabilityInwardsFactorOfSafety.ToNaNAsNull(),
                MacroStabilityInwardsProbability = semiProbabilisticOutput.MacroStabilityInwardsProbability.ToNaNAsNull(),
                MacroStabilityInwardsReliability = semiProbabilisticOutput.MacroStabilityInwardsReliability.ToNaNAsNull(),
                RequiredProbability = semiProbabilisticOutput.RequiredProbability.ToNaNAsNull(),
                RequiredReliability = semiProbabilisticOutput.RequiredReliability.ToNaNAsNull()
            };
        }
    }
}