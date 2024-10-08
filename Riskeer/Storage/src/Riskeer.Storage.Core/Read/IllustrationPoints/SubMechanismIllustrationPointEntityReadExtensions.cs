﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="SubMechanismIllustrationPointEntity"/>
    /// related to creating a <see cref="SubMechanismIllustrationPoint"/>.
    /// </summary>
    internal static class SubMechanismIllustrationPointEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="SubMechanismIllustrationPointEntity"/> and uses 
        /// the information to construct a <see cref="SubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SubMechanismIllustrationPointEntity"/>
        /// to create a <see cref="SubMechanismIllustrationPoint"/> for.</param>
        /// <returns>A new <see cref="SubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        internal static SubMechanismIllustrationPoint Read(this SubMechanismIllustrationPointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            IEnumerable<SubMechanismIllustrationPointStochast> stochasts =
                GetReadSubMechanismIllustrationPointStochasts(entity.SubMechanismIllustrationPointStochastEntities);

            IEnumerable<IllustrationPointResult> illustrationPointResults =
                GetReadIllustrationPointResults(entity.IllustrationPointResultEntities);

            return new SubMechanismIllustrationPoint(entity.Name,
                                                     entity.Beta,
                                                     stochasts,
                                                     illustrationPointResults);
        }

        private static IEnumerable<SubMechanismIllustrationPointStochast> GetReadSubMechanismIllustrationPointStochasts(
            IEnumerable<SubMechanismIllustrationPointStochastEntity> stochastEntities)
        {
            return stochastEntities.OrderBy(st => st.Order)
                                   .Select(st => st.Read())
                                   .ToArray();
        }

        private static IEnumerable<IllustrationPointResult> GetReadIllustrationPointResults(
            IEnumerable<IllustrationPointResultEntity> illustrationPointResultEntities)
        {
            return illustrationPointResultEntities.OrderBy(ipr => ipr.Order)
                                                  .Select(ipr => ipr.Read())
                                                  .ToArray();
        }
    }
}