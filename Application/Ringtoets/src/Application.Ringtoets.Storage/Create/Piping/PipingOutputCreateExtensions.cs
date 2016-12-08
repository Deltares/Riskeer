﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Create.Piping
{
    /// <summary>
    /// Extension methods for <see cref="PipingOutput"/> related to creating a <see cref="PipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class PipingOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="PipingCalculationOutputEntity"/> based on the information
        /// of the <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for piping failure mechanism to 
        /// create a database entity for.</param>
        /// <returns>A new <see cref="PipingCalculationOutputEntity"/>.</returns>
        internal static PipingCalculationOutputEntity Create(this PipingOutput output)
        {
            var entity = new PipingCalculationOutputEntity
            {
                HeaveFactorOfSafety = output.HeaveFactorOfSafety.ToNaNAsNull(),
                HeaveZValue = output.HeaveZValue.ToNaNAsNull(),
                SellmeijerFactorOfSafety = output.SellmeijerFactorOfSafety.ToNaNAsNull(),
                SellmeijerZValue = output.SellmeijerZValue.ToNaNAsNull(),
                UpliftFactorOfSafety = output.UpliftFactorOfSafety.ToNaNAsNull(),
                UpliftZValue = output.UpliftZValue.ToNaNAsNull(),
                HeaveGradient = output.HeaveGradient.Value.ToNaNAsNull(),
                SellmeijerCreepCoefficient = output.SellmeijerCreepCoefficient.Value.ToNaNAsNull(),
                SellmeijerCriticalFall = output.SellmeijerCriticalFall.Value.ToNaNAsNull(),
                SellmeijerReducedFall = output.SellmeijerReducedFall.Value.ToNaNAsNull()
            };
            return entity;
        }
    }
}