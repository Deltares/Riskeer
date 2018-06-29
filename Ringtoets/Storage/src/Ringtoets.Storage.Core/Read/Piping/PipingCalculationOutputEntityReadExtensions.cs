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
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="PipingOutput"/>
    /// based on the <see cref="PipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class PipingCalculationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="PipingCalculationOutputEntity"/> and use the information to
        /// construct a <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingCalculationOutputEntity"/> to create
        /// <see cref="PipingOutput"/> for.</param>
        /// <returns>A new <see cref="PipingOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        public static PipingOutput Read(this PipingCalculationOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new PipingOutput(new PipingOutput.ConstructionProperties
            {
                UpliftZValue = entity.UpliftZValue.ToNullAsNaN(),
                UpliftFactorOfSafety = entity.UpliftFactorOfSafety.ToNullAsNaN(),
                HeaveZValue = entity.HeaveZValue.ToNullAsNaN(),
                HeaveFactorOfSafety = entity.HeaveFactorOfSafety.ToNullAsNaN(),
                SellmeijerZValue = entity.SellmeijerZValue.ToNullAsNaN(),
                SellmeijerFactorOfSafety = entity.SellmeijerFactorOfSafety.ToNullAsNaN(),
                UpliftEffectiveStress = entity.UpliftEffectiveStress.ToNullAsNaN(),
                HeaveGradient = entity.HeaveGradient.ToNullAsNaN(),
                SellmeijerCreepCoefficient = entity.SellmeijerCreepCoefficient.ToNullAsNaN(),
                SellmeijerCriticalFall = entity.SellmeijerCriticalFall.ToNullAsNaN(),
                SellmeijerReducedFall = entity.SellmeijerReducedFall.ToNullAsNaN()
            });
        }
    }
}