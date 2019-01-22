// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="GeneralResult{T}"/>
    /// related to creating an instance of <see cref="GeneralResultSubMechanismIllustrationPointEntity"/>.
    /// </summary>
    internal static class GeneralResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GeneralResultSubMechanismIllustrationPointEntity"/> based on 
        /// the information of <paramref name="generalResult"/>.
        /// </summary>
        /// <param name="generalResult">The general result sub mechanism to create a database 
        /// entity for.</param>
        /// <returns>A new <see cref="GeneralResultSubMechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalResult"/>
        /// is <c>null</c>.</exception>
        public static GeneralResultSubMechanismIllustrationPointEntity CreateGeneralResultSubMechanismIllustrationPointEntity(
            this GeneralResult<TopLevelSubMechanismIllustrationPoint> generalResult)
        {
            if (generalResult == null)
            {
                throw new ArgumentNullException(nameof(generalResult));
            }

            var entity = new GeneralResultSubMechanismIllustrationPointEntity();

            SetGoverningWindDirection(entity, generalResult.GoverningWindDirection);

            AddEntitiesForStochasts(entity, generalResult.Stochasts);
            AddEntitiesForTopLevelSubMechanismIllustrationPoints(
                entity,
                generalResult.TopLevelIllustrationPoints);

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="GeneralResultFaultTreeIllustrationPointEntity"/> based on the 
        /// information of <paramref name="generalResult"/>.
        /// </summary>
        /// <param name="generalResult">The general result to create a database entity for.</param>
        /// <returns>A new <see cref="GeneralResultFaultTreeIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalResult"/>
        /// is <c>null</c>.</exception>
        public static GeneralResultFaultTreeIllustrationPointEntity CreateGeneralResultFaultTreeIllustrationPointEntity(
            this GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            if (generalResult == null)
            {
                throw new ArgumentNullException(nameof(generalResult));
            }

            var entity = new GeneralResultFaultTreeIllustrationPointEntity();

            SetGoverningWindDirection(entity, generalResult.GoverningWindDirection);

            AddEntitiesForStochasts(entity, generalResult.Stochasts);
            AddEntitiesForTopLevelFaultTreeIllustrationPoints(
                entity, generalResult.TopLevelIllustrationPoints);

            return entity;
        }

        private static void SetGoverningWindDirection(IGeneralResultEntity generalResultEntity,
                                                      WindDirection windDirection)
        {
            generalResultEntity.GoverningWindDirectionAngle = windDirection.Angle;
            generalResultEntity.GoverningWindDirectionName = windDirection.Name.DeepClone();
        }

        private static void AddEntitiesForStochasts(IGeneralResultEntity entity,
                                                    IEnumerable<Stochast> stochasts)
        {
            var order = 0;
            foreach (Stochast stochast in stochasts)
            {
                entity.StochastEntities.Add(stochast.Create(order++));
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
                    illustrationPoint.Create(order++));
            }
        }

        private static void AddEntitiesForTopLevelFaultTreeIllustrationPoints(
            GeneralResultFaultTreeIllustrationPointEntity entity,
            IEnumerable<TopLevelFaultTreeIllustrationPoint> illustrationPoints)
        {
            var order = 0;
            foreach (TopLevelFaultTreeIllustrationPoint illustrationPoint in illustrationPoints)
            {
                entity.TopLevelFaultTreeIllustrationPointEntities.Add(
                    illustrationPoint.Create(order++));
            }
        }
    }
}