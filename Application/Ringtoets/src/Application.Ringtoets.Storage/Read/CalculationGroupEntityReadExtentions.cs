// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections;

using Application.Ringtoets.Storage.DbContext;

using Ringtoets.Common.Data.Calculation;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="CalculationGroup"/> based on the
    /// <see cref="CalculationGroupEntity"/>.
    /// </summary>
    internal static class CalculationGroupEntityReadExtentions
    {
        /// <summary>
        /// Read the <see cref="CalculationGroupEntity"/> and use the information to construct
        /// a <see cref="CalculationGroup"/>.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to create 
        /// <see cref="CalculationGroup"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="CalculationGroup"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static CalculationGroup Read(this CalculationGroupEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var group = new CalculationGroup(entity.Name, Convert.ToBoolean(entity.IsEditable))
            {
                StorageId = entity.CalculationGroupEntityId
            };
            foreach (object childEntity in GetChildEntitiesInOrder(entity))
            {
                var childCalculationGroupEntity = childEntity as CalculationGroupEntity;
                if (childCalculationGroupEntity != null)
                {
                    group.Children.Add(childCalculationGroupEntity.Read(collector));
                }
            }

            return group;
        }

        private static IEnumerable GetChildEntitiesInOrder(CalculationGroupEntity entity)
        {
            var sortedList = new SortedList();
            foreach (CalculationGroupEntity groupEntity in entity.CalculationGroupEntity1)
            {
                sortedList.Add(groupEntity.Order, groupEntity);
            }
            return sortedList.Values;
        }
    }
}