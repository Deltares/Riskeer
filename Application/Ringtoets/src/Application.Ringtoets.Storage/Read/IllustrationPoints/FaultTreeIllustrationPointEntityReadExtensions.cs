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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Application.Ringtoets.Storage.Read.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="FaultTreeIllustrationPointEntity"/>
    /// related to creating a <see cref="IllustrationPointNode"/>.
    /// </summary>
    internal static class FaultTreeIllustrationPointEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="FaultTreeIllustrationPointEntity"/> and uses 
        /// the information to construct a <see cref="IllustrationPointNode"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FaultTreeIllustrationPointEntity"/>
        /// to create a <see cref="IllustrationPointNode"/> for.</param>
        /// <returns>A new <see cref="IllustrationPointNode"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        public static IllustrationPointNode Read(this FaultTreeIllustrationPointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return new IllustrationPointNode(GetFaultTreeIllustrationPoint(entity));
        }

        private static FaultTreeIllustrationPoint GetFaultTreeIllustrationPoint(FaultTreeIllustrationPointEntity entity)
        {
            IEnumerable<Stochast> stochasts = GetReadStochasts(entity.StochastEntities);

            return new FaultTreeIllustrationPoint(entity.Name, entity.Beta, stochasts, GetCombinationType(entity));
        }

        private static CombinationType GetCombinationType(FaultTreeIllustrationPointEntity entity)
        {
            return entity.CombinationType == (byte) CombinationType.And
                       ? CombinationType.And
                       : CombinationType.Or;
        }

        private static IEnumerable<Stochast> GetReadStochasts(IEnumerable<StochastEntity> stochastEntities)
        {
            return stochastEntities.OrderBy(st => st.Order)
                                   .Select(st => st.Read());
        }
    }
}