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

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for <see cref="Point3D"/>
    /// for <see cref="RingtoetsPipingSurfaceLine.Points"/>based on the <see cref="SurfaceLinePointEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLinePointReadExtensions
    {
        /// <summary>
        /// Read the <see cref="SurfaceLinePointEntity"/> and use the information to construct
        /// a <see cref="Point3D"/> for a <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> to create
        /// <see cref="Point3D"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new surfaceline geometry point.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static Point3D Read(this SurfaceLinePointEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var surfaceLineGeometryPoint = new Point3D(
                Convert.ToDouble(entity.X),
                Convert.ToDouble(entity.Y),
                Convert.ToDouble(entity.Z))
            {
                StorageId = entity.SurfaceLinePointEntityId
            };

            collector.Read(entity, surfaceLineGeometryPoint);

            return surfaceLineGeometryPoint;
        }
    }
}