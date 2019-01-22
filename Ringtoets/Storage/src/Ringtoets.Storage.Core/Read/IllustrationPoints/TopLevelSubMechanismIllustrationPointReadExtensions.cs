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
using Ringtoets.Common.Data.IllustrationPoints;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.IllustrationPoints
{
    /// <summary>
    /// Extension methods for <see cref="TopLevelSubMechanismIllustrationPointEntity"/>
    /// related to creating a <see cref="TopLevelSubMechanismIllustrationPoint"/>.
    /// </summary>
    internal static class TopLevelSubMechanismIllustrationPointReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="TopLevelSubMechanismIllustrationPointEntity"/> and 
        /// uses the information to construct a <see cref="TopLevelSubMechanismIllustrationPoint"/>.
        /// </summary>
        /// <param name="entity">The <see cref="TopLevelSubMechanismIllustrationPointEntity"/>
        /// to create a <see cref="TopLevelSubMechanismIllustrationPoint"/> for.</param>
        /// <returns>A new <see cref="TopLevelSubMechanismIllustrationPoint"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/>
        /// is <c>null</c>.</exception>
        internal static TopLevelSubMechanismIllustrationPoint Read(this TopLevelSubMechanismIllustrationPointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            WindDirection windDirection = GetWindDirection(entity);

            SubMechanismIllustrationPoint subMechanismIllustrationPoint =
                entity.SubMechanismIllustrationPointEntity.Read();

            return new TopLevelSubMechanismIllustrationPoint(windDirection,
                                                             entity.ClosingSituation,
                                                             subMechanismIllustrationPoint);
        }

        private static WindDirection GetWindDirection(TopLevelSubMechanismIllustrationPointEntity entity)
        {
            return new WindDirection(entity.WindDirectionName,
                                     entity.WindDirectionAngle);
        }
    }
}