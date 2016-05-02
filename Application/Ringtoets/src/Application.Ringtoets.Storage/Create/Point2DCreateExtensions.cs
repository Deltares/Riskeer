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
using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="Point2D"/> related to creating database entities.
    /// </summary>
    public static class Point2DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ReferenceLinePointEntity"/> based on the information of the <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The point to create a database entity for.</param>
        /// <param name="order">A value representing the position of the point in an ordered collection.</param>
        /// <returns>A new <see cref="ReferenceLinePointEntity"/>.</returns>
        public static ReferenceLinePointEntity CreateReferenceLinePoint(this Point2D point, int order)
        {
            var entity = new ReferenceLinePointEntity
            {
                X = Convert.ToDecimal(point.X),
                Y = Convert.ToDecimal(point.Y),
                Order = order
            };

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="FailureMechanismSectionPointEntity"/> based on the information of the <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The point to create a database entity for.</param>
        /// <param name="order">A value representing the position of the point in an ordered collection.</param>
        /// <returns>A new <see cref="FailureMechanismSectionPointEntity"/>.</returns>
        public static FailureMechanismSectionPointEntity CreateFailureMechanismSectionPoint(this Point2D point, int order)
        {
            var entity = new FailureMechanismSectionPointEntity
            {
                X = Convert.ToDecimal(point.X),
                Y = Convert.ToDecimal(point.Y),
                Order = order
            };

            return entity;
        }


    }
}