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

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extensions methods for <see cref="RingtoetsPipingSurfaceLine.Points"/> related to
    /// creating an <see cref="SurfaceLinePointEntity"/>.
    /// </summary>
    internal static class Point3DCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLinePointEntity"/> based on the information of the <see cref="Point3D"/>.
        /// </summary>
        /// <param name="geometryPoint">The geometry point to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index in <see cref="RingtoetsPipingSurfaceLine.Points"/>.</param>
        /// <returns>A new <see cref="SurfaceLinePointEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static SurfaceLinePointEntity CreateSurfaceLinePointEntity(this Point3D geometryPoint,
                                                                            PersistenceRegistry registry,
                                                                            int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new SurfaceLinePointEntity
            {
                X = Convert.ToDecimal(geometryPoint.X),
                Y = Convert.ToDecimal(geometryPoint.Y),
                Z = Convert.ToDecimal(geometryPoint.Z),
                Order = order
            };

            registry.Register(entity, geometryPoint);

            return entity;
        }
    }
}