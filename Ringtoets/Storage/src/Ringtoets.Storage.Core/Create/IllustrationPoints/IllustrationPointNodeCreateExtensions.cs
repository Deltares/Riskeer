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
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="FaultTreeIllustrationPoint"/> 
    /// related to creating an instance of <see cref="FaultTreeIllustrationPointEntity"/>.
    /// </summary>
    internal static class IllustrationPointNodeCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FaultTreeIllustrationPointEntity"/> based on the information 
        /// of <paramref name="illustrationPointNode"/>.
        /// </summary>
        /// <param name="illustrationPointNode">The illustration point node to create a database 
        /// entity for.</param>
        /// <param name="order">The index at which <paramref name="illustrationPointNode"/>
        /// resides within its parent.</param>
        /// <returns>A new <see cref="FaultTreeIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="illustrationPointNode"/> 
        /// is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="IllustrationPointNode.Data"/> 
        /// is not of type <see cref="FaultTreeIllustrationPoint"/>.</exception>
        public static FaultTreeIllustrationPointEntity Create(
            this IllustrationPointNode illustrationPointNode,
            int order)
        {
            if (illustrationPointNode == null)
            {
                throw new ArgumentNullException(nameof(illustrationPointNode));
            }

            var faultTreeIllustrationPoint = illustrationPointNode.Data as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint == null)
            {
                throw new NotSupportedException($"Illustration point type '{illustrationPointNode.Data.GetType()}' is not supported.");
            }

            FaultTreeIllustrationPointEntity entity = CreateFaultTreeIllustrationPoint(faultTreeIllustrationPoint,
                                                                                       order);

            CreateChildElements(illustrationPointNode.Children, entity);
            return entity;
        }

        private static FaultTreeIllustrationPointEntity CreateFaultTreeIllustrationPoint(
            FaultTreeIllustrationPoint illustrationPoint,
            int order)
        {
            var entity = new FaultTreeIllustrationPointEntity
            {
                Beta = illustrationPoint.Beta,
                CombinationType = Convert.ToByte(illustrationPoint.CombinationType),
                Name = illustrationPoint.Name.DeepClone(),
                Order = order
            };
            AddEntitiesForStochasts(entity, illustrationPoint.Stochasts);

            return entity;
        }

        private static void AddEntitiesForStochasts(FaultTreeIllustrationPointEntity entity,
                                                    IEnumerable<Stochast> stochasts)
        {
            var order = 0;
            foreach (Stochast stochast in stochasts)
            {
                entity.StochastEntities.Add(stochast.Create(order++));
            }
        }

        private static void CreateChildElements(IEnumerable<IllustrationPointNode> illustrationPointNodes,
                                                FaultTreeIllustrationPointEntity entity)
        {
            var i = 0;
            foreach (IllustrationPointNode node in illustrationPointNodes)
            {
                var faultTreeIllustrationPoint = node.Data as FaultTreeIllustrationPoint;
                if (faultTreeIllustrationPoint != null)
                {
                    entity.FaultTreeIllustrationPointEntity1.Add(node.Create(i));
                }

                var subMechanismIllustrationPoint = node.Data as SubMechanismIllustrationPoint;
                if (subMechanismIllustrationPoint != null)
                {
                    entity.SubMechanismIllustrationPointEntities.Add(subMechanismIllustrationPoint.Create(i));
                }

                i++;
            }
        }
    }
}