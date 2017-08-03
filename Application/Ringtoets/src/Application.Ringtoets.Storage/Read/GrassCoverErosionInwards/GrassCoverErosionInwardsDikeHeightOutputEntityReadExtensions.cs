﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    /// Extension methods for <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/>
    /// related to creating a <see cref="DikeHeightOutput"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsDikeHeightOutputEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/> and use
        /// the information to construct a <see cref="DikeHeightOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsDikeHeightOutputEntity"/>
        /// to create <see cref="DikeHeightOutput"/> for.</param>
        /// <returns>A new <see cref="DikeHeightOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        internal static DikeHeightOutput Read(this GrassCoverErosionInwardsDikeHeightOutputEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new DikeHeightOutput(entity.DikeHeight.ToNullAsNaN(),
                                        entity.TargetProbability.ToNullAsNaN(),
                                        entity.TargetReliability.ToNullAsNaN(),
                                        entity.CalculatedProbability.ToNullAsNaN(),
                                        entity.CalculatedReliability.ToNullAsNaN(),
                                        (CalculationConvergence) entity.CalculationConvergence,
                                        null);
        }
    }
}