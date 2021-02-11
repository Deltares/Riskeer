﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.Piping.SemiProbabilistic
{
    /// <summary>
    /// Extension methods for <see cref="SemiProbabilisticPipingOutput"/> related to creating
    /// a <see cref="SemiProbabilisticPipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class SemiProbabilisticPipingOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SemiProbabilisticPipingCalculationOutputEntity"/> based on the information
        /// of the <see cref="SemiProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for piping failure mechanism to 
        /// create a database entity for.</param>
        /// <returns>A new <see cref="SemiProbabilisticPipingCalculationOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/>
        /// is <c>null</c>.</exception>
        public static SemiProbabilisticPipingCalculationOutputEntity Create(this SemiProbabilisticPipingOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            var entity = new SemiProbabilisticPipingCalculationOutputEntity
            {
                HeaveFactorOfSafety = output.HeaveFactorOfSafety.ToNaNAsNull(),
                SellmeijerFactorOfSafety = output.SellmeijerFactorOfSafety.ToNaNAsNull(),
                UpliftFactorOfSafety = output.UpliftFactorOfSafety.ToNaNAsNull(),
                UpliftEffectiveStress = output.UpliftEffectiveStress.ToNaNAsNull(),
                HeaveGradient = output.HeaveGradient.ToNaNAsNull(),
                SellmeijerCreepCoefficient = output.SellmeijerCreepCoefficient.ToNaNAsNull(),
                SellmeijerCriticalFall = output.SellmeijerCriticalFall.ToNaNAsNull(),
                SellmeijerReducedFall = output.SellmeijerReducedFall.ToNaNAsNull()
            };
            return entity;
        }
    }
}