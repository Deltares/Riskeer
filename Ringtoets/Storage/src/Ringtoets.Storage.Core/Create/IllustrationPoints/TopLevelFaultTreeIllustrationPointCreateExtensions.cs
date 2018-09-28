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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="TopLevelFaultTreeIllustrationPoint"/>
    /// related to creating an instance of <see cref="TopLevelFaultTreeIllustrationPointEntity"/>.
    /// </summary>
    internal static class TopLevelFaultTreeIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="TopLevelFaultTreeIllustrationPointEntity"/>
        /// based on the information of <paramref name="topLevelFaultTreeIllustrationPoint"/>.
        /// </summary>
        /// <param name="topLevelFaultTreeIllustrationPoint">The top level illustration point to 
        /// create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="topLevelFaultTreeIllustrationPoint"/>
        /// resides within its parent.</param>
        /// <returns>A <see cref="TopLevelFaultTreeIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="topLevelFaultTreeIllustrationPoint"/> is <c>null</c>.</exception>
        public static TopLevelFaultTreeIllustrationPointEntity Create(
            this TopLevelFaultTreeIllustrationPoint topLevelFaultTreeIllustrationPoint,
            int order)
        {
            if (topLevelFaultTreeIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(topLevelFaultTreeIllustrationPoint));
            }

            WindDirection windDirection = topLevelFaultTreeIllustrationPoint.WindDirection;
            return new TopLevelFaultTreeIllustrationPointEntity
            {
                ClosingSituation = topLevelFaultTreeIllustrationPoint.ClosingSituation.DeepClone(),
                WindDirectionName = windDirection.Name.DeepClone(),
                WindDirectionAngle = windDirection.Angle,
                FaultTreeIllustrationPointEntity =
                    topLevelFaultTreeIllustrationPoint.FaultTreeNodeRoot.Create(0),
                Order = order
            };
        }
    }
}