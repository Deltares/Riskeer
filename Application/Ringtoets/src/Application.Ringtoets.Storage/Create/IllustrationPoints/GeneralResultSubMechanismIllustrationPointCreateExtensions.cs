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
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Application.Ringtoets.Storage.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="GeneralResultSubMechanismIllustrationPoint"/>
    /// related to creating an instance of <see cref="GeneralResultSubMechanismIllustrationPointEntity"/>.
    /// </summary>
    internal static class GeneralResultSubMechanismIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GeneralResultSubMechanismIllustrationPointEntity"/> based on the 
        /// information of <paramref name="generalResultSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="generalResultSubMechanismIllustrationPoint">The general result submechanism
        /// to create a database entity for.</param>
        /// <returns>A new <see cref="GeneralResultSubMechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalResultSubMechanismIllustrationPoint"/>
        /// is <c>null</c>.</exception>
        internal static GeneralResultSubMechanismIllustrationPointEntity CreateGeneralResultSubMechanismIllustrationPointEntity(
            this GeneralResultSubMechanismIllustrationPoint generalResultSubMechanismIllustrationPoint)
        {
            if (generalResultSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(generalResultSubMechanismIllustrationPoint));
            }

            WindDirection governingWindDirection = generalResultSubMechanismIllustrationPoint.GoverningWindDirection;
            var entity = new GeneralResultSubMechanismIllustrationPointEntity
            {
                GoverningWindDirectionName = governingWindDirection.Name.DeepClone(),
                GoverningWindDirectionAngle = governingWindDirection.Angle
            };

            AddEntitiesForStochasts(entity, generalResultSubMechanismIllustrationPoint.Stochasts);
            AddEntitiesForTopLevelSubMechanismIllustrationPoints(
                entity,
                generalResultSubMechanismIllustrationPoint.TopLevelSubMechanismIllustrationPoints);

            return entity;
        }

        private static void AddEntitiesForStochasts(GeneralResultSubMechanismIllustrationPointEntity entity,
                                                    IEnumerable<Stochast> stochasts)
        {
            var order = 0;
            foreach (Stochast stochast in stochasts)
            {
                entity.StochastEntities.Add(stochast.CreateStochastEntity(order++));
            }
        }

        private static void AddEntitiesForTopLevelSubMechanismIllustrationPoints(
            GeneralResultSubMechanismIllustrationPointEntity entity,
            IEnumerable<TopLevelSubMechanismIllustrationPoint> illustrationPoints)
        {
            var order = 0;
            foreach (TopLevelSubMechanismIllustrationPoint illustrationPoint in illustrationPoints)
            {
                entity.TopLevelSubMechanismIllustrationPointEntities.Add(
                    illustrationPoint.CreateTopLevelSubMechanismIllustrationPointEntity(order++));
            }
        }
    }
}