﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics.IllustrationPoints;

namespace Application.Ringtoets.Storage.Create.IllustrationPoints
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
        /// <param name="topLevelSubMechanismIllustrationPoint"></param>
        /// <param name="order">The index at which <paramref name="topLevelSubMechanismIllustrationPoint"/>
        /// resides within its parent.</param>
        /// <returns>A <see cref="TopLevelSubMechanismIllustrationPointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="topLevelSubMechanismIllustrationPoint"/> is <c>null</c>.</exception>
        public static TopLevelSubMechanismIllustrationPointEntity CreateTopLevelSubMechanismIllustrationPointEntity(
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
                ClosingSituation = topLevelSubMechanismIllustrationPoint.ClosingSituation,
                WindDirectionName = windDirection.Name,
                WindDirectionAngle = windDirection.Angle,
                SubMechanismIllustrationPointEntity =
                    topLevelSubMechanismIllustrationPoint.SubMechanismIllustrationPoint
                                                         .CreateSubMechanismIllustrationPointEntity(),
                Order = order
            };
        }
    }
}