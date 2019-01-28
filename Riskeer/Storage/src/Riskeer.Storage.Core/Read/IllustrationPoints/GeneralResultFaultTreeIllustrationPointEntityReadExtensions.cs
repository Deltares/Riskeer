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
using System.Collections.Generic;
using System.Linq;
using Ringtoets.Common.Data.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>
    /// related to creating a <see cref="GeneralResult{T}"/> for fault tree illustration point.
    /// </summary>
    internal static class GeneralResultFaultTreeIllustrationPointEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GeneralResultFaultTreeIllustrationPointEntity"/> and uses 
        /// the information to construct a <see cref="GeneralResult{T}"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>
        /// to create a <see cref="GeneralResult{T}"/> for.</param>
        /// <returns>A new <see cref="GeneralResult{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is 
        /// <c>null</c>.</exception>
        public static GeneralResult<TopLevelFaultTreeIllustrationPoint> Read(
            this GeneralResultFaultTreeIllustrationPointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            WindDirection governingWindDirection = GetGoverningWindDirection(entity);

            IEnumerable<Stochast> stochasts = GetReadStochasts(entity.StochastEntities);

            IEnumerable<TopLevelFaultTreeIllustrationPoint> topLevelIllustrationPoints =
                GetReadTopLevelFaultTreeIllustrationPoint(entity.TopLevelFaultTreeIllustrationPointEntities);

            return new GeneralResult<TopLevelFaultTreeIllustrationPoint>(governingWindDirection,
                                                                         stochasts,
                                                                         topLevelIllustrationPoints);
        }

        private static WindDirection GetGoverningWindDirection(IGeneralResultEntity entity)
        {
            return new WindDirection(entity.GoverningWindDirectionName,
                                     entity.GoverningWindDirectionAngle);
        }

        private static IEnumerable<Stochast> GetReadStochasts(IEnumerable<StochastEntity> stochastEntities)
        {
            return stochastEntities.OrderBy(st => st.Order)
                                   .Select(st => st.Read())
                                   .ToArray();
        }

        private static IEnumerable<TopLevelFaultTreeIllustrationPoint> GetReadTopLevelFaultTreeIllustrationPoint(
            IEnumerable<TopLevelFaultTreeIllustrationPointEntity> topLevelIllustrationPointEntities)
        {
            return topLevelIllustrationPointEntities.OrderBy(ip => ip.Order)
                                                    .Select(ip => ip.Read())
                                                    .ToArray();
        }
    }
}