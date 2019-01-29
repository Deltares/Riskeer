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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="TopLevelSubMechanismIllustrationPoint"/>
    /// related to creating an instance of <see cref="TopLevelSubMechanismIllustrationPointEntity"/>.
    /// </summary>
    internal static class TopLevelSubMechanismIllustrationPointCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="TopLevelSubMechanismIllustrationPointEntity"/>
        /// based on the information of <paramref name="topLevelSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="topLevelSubMechanismIllustrationPoint">The top level illustration point
        /// to create a database entity for.</param>
        /// <param name="order">The index at which <paramref name="topLevelSubMechanismIllustrationPoint"/>
        /// resides within its parent.</param>
        /// <returns>A <see cref="TopLevelSubMechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="topLevelSubMechanismIllustrationPoint"/> is <c>null</c>.</exception>
        internal static TopLevelSubMechanismIllustrationPointEntity Create(
            this TopLevelSubMechanismIllustrationPoint topLevelSubMechanismIllustrationPoint,
            int order)
        {
            if (topLevelSubMechanismIllustrationPoint == null)
            {
                throw new ArgumentNullException(nameof(topLevelSubMechanismIllustrationPoint));
            }

            WindDirection windDirection = topLevelSubMechanismIllustrationPoint.WindDirection;
            return new TopLevelSubMechanismIllustrationPointEntity
            {
                ClosingSituation = topLevelSubMechanismIllustrationPoint.ClosingSituation.DeepClone(),
                WindDirectionName = windDirection.Name.DeepClone(),
                WindDirectionAngle = windDirection.Angle,
                SubMechanismIllustrationPointEntity =
                    topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint
                                                         .Create(0),
                Order = order
            };
        }
    }
}