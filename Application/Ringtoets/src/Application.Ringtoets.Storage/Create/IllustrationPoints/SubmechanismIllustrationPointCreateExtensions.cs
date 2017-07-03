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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Application.Ringtoets.Storage.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="SubmechanismIllustrationPointStochast"/> 
    /// related to creating an instance of <see cref="SubmechanismIllustrationPointEntity"/>.
    /// </summary>
    internal static class SubmechanismIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SubmechanismIllustrationPointEntity"/> based on 
        /// the information of <paramref name="submechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="submechanismIllustrationPoint">The submechanism illustration
        /// point to create a database entity for.</param>
        /// <returns>A new <see cref="SubmechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="submechanismIllustrationPoint"/> is <c>null</c>.</exception>
        public static SubmechanismIllustrationPointEntity CreateSubmechanismIllustrationPointEntity(
            this SubmechanismIllustrationPoint submechanismIllustrationPoint)
        {
            if (submechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(submechanismIllustrationPoint));
            }

            var entity = new SubmechanismIllustrationPointEntity
            {
                Beta = submechanismIllustrationPoint.Beta,
                Name = submechanismIllustrationPoint.Name
            };

            CreateSubmechanismIllustrationPointStochastEntities(submechanismIllustrationPoint.Stochasts, entity);
            CreateIllustrationPointResultEntities(submechanismIllustrationPoint.IllustrationPointResults, entity);

            return entity;
        }

        private static void CreateIllustrationPointResultEntities(IEnumerable<IllustrationPointResult> illustrationPointResults, SubmechanismIllustrationPointEntity entity)
        {
            var order = 0;
            foreach (IllustrationPointResult illustrationPointResult in illustrationPointResults)
            {
                entity.IllustrationPointResultEntities.Add(
                    illustrationPointResult.CreateIllustrationPointResultEntity(order++));
            }
        }

        private static void CreateSubmechanismIllustrationPointStochastEntities(IEnumerable<SubmechanismIllustrationPointStochast> stochasts, SubmechanismIllustrationPointEntity entity)
        {
            var order = 0;
            foreach (SubmechanismIllustrationPointStochast submechanismIllustrationPointStochast in stochasts)
            {
                entity.SubmechanismIllustrationPointStochastEntities.Add(
                    submechanismIllustrationPointStochast.CreateSubmechanismIllustrationPointStochastEntity(order++));
            }
        }
    }
}