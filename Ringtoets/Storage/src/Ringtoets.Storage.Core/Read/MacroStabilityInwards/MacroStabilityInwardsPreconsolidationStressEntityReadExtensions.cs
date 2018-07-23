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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsPreconsolidationStress"/> 
    /// based on the <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsPreconsolidationStressEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/> and use the information 
        /// to construct a <see cref="MacroStabilityInwardsPreconsolidationStress"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsPreconsolidationStressEntity"/> to create 
        /// <see cref="MacroStabilityInwardsPreconsolidationStress"/> for.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsPreconsolidationStress"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public static MacroStabilityInwardsPreconsolidationStress Read(this MacroStabilityInwardsPreconsolidationStressEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var location = new Point2D(entity.CoordinateX, entity.CoordinateZ);
            var distribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) entity.PreconsolidationStressMean.ToNullAsNaN(),
                CoefficientOfVariation = (RoundedDouble) entity.PreconsolidationStressCoefficientOfVariation.ToNullAsNaN()
            };

            return new MacroStabilityInwardsPreconsolidationStress(location, distribution);
        }
    }
}