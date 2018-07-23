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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="SubMechanismIllustrationPoint"/> 
    /// related to creating an instance of <see cref="SubMechanismIllustrationPointEntity"/>.
    /// </summary>
    internal static class SubMechanismIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SubMechanismIllustrationPointEntity"/> based on 
        /// the information of <paramref name="subMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="subMechanismIllustrationPoint">The sub mechanism illustration
        /// point to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="subMechanismIllustrationPoint"/>
        /// resides within its parent.</param>
        /// <returns>A new <see cref="SubMechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="subMechanismIllustrationPoint"/> is <c>null</c>.</exception>
        internal static SubMechanismIllustrationPointEntity Create(
            this SubMechanismIllustrationPoint subMechanismIllustrationPoint,
            int order)
        {
            if (subMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(subMechanismIllustrationPoint));
            }

            var entity = new SubMechanismIllustrationPointEntity
            {
                Beta = subMechanismIllustrationPoint.Beta,
                Name = subMechanismIllustrationPoint.Name.DeepClone(),
                Order = order
            };

            AddEntitiesForSubMechanismIllustrationPointsStochasts(entity, subMechanismIllustrationPoint.Stochasts);
            AddEntitiesForIllustrationPointResults(entity, subMechanismIllustrationPoint.IllustrationPointResults);

            return entity;
        }

        private static void AddEntitiesForIllustrationPointResults(SubMechanismIllustrationPointEntity entity,
                                                                   IEnumerable<IllustrationPointResult> illustrationPointResults)
        {
            var order = 0;
            foreach (IllustrationPointResult illustrationPointResult in illustrationPointResults)
            {
                entity.IllustrationPointResultEntities.Add(
                    illustrationPointResult.Create(order++));
            }
        }

        private static void AddEntitiesForSubMechanismIllustrationPointsStochasts(
            SubMechanismIllustrationPointEntity entity,
            IEnumerable<SubMechanismIllustrationPointStochast> stochasts)
        {
            var order = 0;
            foreach (SubMechanismIllustrationPointStochast subMechanismIllustrationPointStochast in stochasts)
            {
                entity.SubMechanismIllustrationPointStochastEntities.Add(
                    subMechanismIllustrationPointStochast.Create(order++));
            }
        }
    }
}