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

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsCalculation"/> related
    /// to creating a <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionInwardsCalculationEntity"/> based
        /// on the information of the <see cref="GrassCoverErosionInwardsCalculation"/>.
        /// </summary>
        /// <param name="calculation">The calculation to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at where <paramref name="calculation"/> resides
        /// in its parent container.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionInwardsCalculationEntity Create(this GrassCoverErosionInwardsCalculation calculation, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new GrassCoverErosionInwardsCalculationEntity
            {
                Name = calculation.Name,
                Comments = calculation.Comments,
                Order = order
            };
            registry.Register(entity, calculation);
            return entity;
        }
    }
}