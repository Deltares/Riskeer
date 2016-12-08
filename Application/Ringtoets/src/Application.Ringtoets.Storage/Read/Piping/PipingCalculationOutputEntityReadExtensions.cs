// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="PipingOutput"/>
    /// based on the <see cref="PipingCalculationOutputEntity"/>.
    /// </summary>
    internal static class PipingCalculationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="AssessmentSectionEntity"/> and use the information to
        /// construct a <see cref="PipingOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingCalculationOutputEntity"/> to create
        /// <see cref="PipingOutput"/> for.</param>
        /// <returns>A new <see cref="PipingOutput"/>.</returns>
        internal static PipingOutput Read(this PipingCalculationOutputEntity entity)
        {
            return new PipingOutput(entity.UpliftZValue.ToNullAsNaN(), entity.UpliftFactorOfSafety.ToNullAsNaN(),
                                    entity.HeaveZValue.ToNullAsNaN(), entity.HeaveFactorOfSafety.ToNullAsNaN(),
                                    entity.SellmeijerZValue.ToNullAsNaN(), entity.SellmeijerFactorOfSafety.ToNullAsNaN(),
                                    entity.HeaveGradient.ToNullAsNaN(), entity.SellmeijerCreepCoefficient.ToNullAsNaN(),
                                    entity.SellmeijerCriticalFall.ToNullAsNaN(), entity.SellmeijerReducedFall.ToNullAsNaN());
        }
    }
}