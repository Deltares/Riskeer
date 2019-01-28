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
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.MacroStabilityInwards
{
    /// <summary>
    /// Extension methods for <see cref="MacroStabilityInwardsPreconsolidationStress"/> related to creating 
    /// a <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsPreconsolidationStressCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/> based on the information 
        /// of the <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="preconsolidationStress">PreconsolidationStress to create a database entity for.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="preconsolidationStress"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsPreconsolidationStressEntity Create(this MacroStabilityInwardsPreconsolidationStress preconsolidationStress,
                                                                               int order)
        {
            if (preconsolidationStress == null)
            {
                throw new ArgumentNullException(nameof(preconsolidationStress));
            }

            return new MacroStabilityInwardsPreconsolidationStressEntity
            {
                CoordinateX = preconsolidationStress.Location.X,
                CoordinateZ = preconsolidationStress.Location.Y,
                PreconsolidationStressMean = preconsolidationStress.Stress.Mean.ToNaNAsNull(),
                PreconsolidationStressCoefficientOfVariation = preconsolidationStress.Stress.CoefficientOfVariation.ToNaNAsNull(),
                Order = order
            };
        }
    }
}