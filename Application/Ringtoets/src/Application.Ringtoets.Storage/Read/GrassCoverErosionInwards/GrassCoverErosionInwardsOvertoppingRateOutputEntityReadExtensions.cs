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

using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>
    /// related to creating a <see cref="OvertoppingRateOutput"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsOvertoppingRateOutputEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/> and use
        /// the information to construct a <see cref="OvertoppingRateOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsOvertoppingRateOutputEntity"/>
        /// to create <see cref="OvertoppingRateOutput"/> for.</param>
        /// <returns>A new <see cref="OvertoppingRateOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        internal static OvertoppingRateOutput Read(this GrassCoverErosionInwardsOvertoppingRateOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new OvertoppingRateOutput(entity.OvertoppingRate.ToNullAsNaN(),
                                             entity.TargetProbability.ToNullAsNaN(),
                                             entity.TargetReliability.ToNullAsNaN(),
                                             entity.CalculatedProbability.ToNullAsNaN(),
                                             entity.CalculatedReliability.ToNullAsNaN(),
                                             (CalculationConvergence) entity.CalculationConvergence);
        }
    }
}