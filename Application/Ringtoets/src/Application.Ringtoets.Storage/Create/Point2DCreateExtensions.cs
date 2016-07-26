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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Geometry;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="Point2D"/> related to creating database entities.
    /// </summary>
    internal static class Point2DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ReferenceLinePointEntity"/> based on the information of the <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The point to create a database entity for.</param>
        /// <param name="order">A value representing the position of the point in an ordered collection.</param>
        /// <returns>A new <see cref="ReferenceLinePointEntity"/>.</returns>
        internal static ReferenceLinePointEntity CreateReferenceLinePointEntity(this Point2D point, int order)
        {
            var entity = new ReferenceLinePointEntity
            {
                X = point.X.ToNaNAsNull(),
                Y = point.Y.ToNaNAsNull(),
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
        internal static FailureMechanismSectionPointEntity CreateFailureMechanismSectionPointEntity(this Point2D point, int order)
        {
            var entity = new FailureMechanismSectionPointEntity
            {
                X = point.X.ToNaNAsNull(),
                Y = point.Y.ToNaNAsNull(),
                Order = order
            };

            return entity;
        }

        /// <summary>
        /// Creates a <see cref="StochasticSoilModelSegmentPointEntity"/> based on the
        /// information of the <see cref="Point2D"/>.
        /// </summary>
        /// <param name="point">The point to create a database entity for.</param>
        /// <param name="order">A value representing the position of the point in an
        /// ordered collection.</param>
        /// <returns>A new <see cref="StochasticSoilModelSegmentPointEntity"/>.</returns>
        internal static StochasticSoilModelSegmentPointEntity CreateStochasticSoilModelSegmentPointEntity(this Point2D point, int order)
        {
            var entity = new StochasticSoilModelSegmentPointEntity
            {
                X = point.X.ToNaNAsNull(),
                Y = point.Y.ToNaNAsNull(),
                Order = order
            };

            return entity;
        }
    }
}