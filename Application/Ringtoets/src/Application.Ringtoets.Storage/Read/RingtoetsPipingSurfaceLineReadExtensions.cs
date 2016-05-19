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

using Ringtoets.Integration.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="RingtoetsPipingSurfaceLine"/>
    /// based on the <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLineReadExtensions
    {
        /// <summary>
        /// Read the <see cref="SurfaceLineEntity"/> and use the information to construct
        /// a <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to create
        /// <see cref="RingtoetsPipingSurfaceLine"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static RingtoetsPipingSurfaceLine Read(this SurfaceLineEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                StorageId = entity.SurfaceLineEntityId,
                Name = entity.Name,
                ReferenceLineIntersectionWorldPoint = new Point2D(
                    Convert.ToDouble(entity.ReferenceLineIntersectionX),
                    Convert.ToDouble(entity.ReferenceLineIntersectionY))
            };

            entity.ReadSurfaceLineGeometry(surfaceLine, collector);
            entity.ReadCharacteristicPoints(surfaceLine, collector);

            return surfaceLine;
        }

        private static void ReadCharacteristicPoints(this SurfaceLineEntity entity, RingtoetsPipingSurfaceLine surfaceLine, ReadConversionCollector collector)
        {
            if (entity.BottomDitchDikeSidePointEntity != null)
            {
                surfaceLine.SetBottomDitchDikeSideAt(collector.Get(entity.BottomDitchDikeSidePointEntity));
            }
            if (entity.BottomDitchPolderSidePointEntity != null)
            {
                surfaceLine.SetBottomDitchPolderSideAt(collector.Get(entity.BottomDitchPolderSidePointEntity));
            }
            if (entity.DikeToeAtPolderPointEntity != null)
            {
                surfaceLine.SetDikeToeAtPolderAt(collector.Get(entity.DikeToeAtPolderPointEntity));
            }
            if (entity.DikeToeAtRiverPointEntity != null)
            {
                surfaceLine.SetDikeToeAtRiverAt(collector.Get(entity.DikeToeAtRiverPointEntity));
            }
            if (entity.DitchDikeSidePointEntity != null)
            {
                surfaceLine.SetDitchDikeSideAt(collector.Get(entity.DitchDikeSidePointEntity));
            }
            if (entity.DitchPolderSidePointEntity != null)
            {
                surfaceLine.SetDitchPolderSideAt(collector.Get(entity.DitchPolderSidePointEntity));
            }
        }

        private static void ReadSurfaceLineGeometry(this SurfaceLineEntity entity, RingtoetsPipingSurfaceLine surfaceLine, ReadConversionCollector collector)
        {
            var geometryPoints = new Point3D[entity.SurfaceLinePointEntities.Count];
            foreach (SurfaceLinePointEntity pointEntity in entity.SurfaceLinePointEntities)
            {
                var geometryPoint = pointEntity.Read(collector);
                geometryPoints[pointEntity.Order] = geometryPoint;
            }
            surfaceLine.SetGeometry(geometryPoints);
        }
    }
}