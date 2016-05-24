﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
    /// Extensions methods for <see cref="RingtoetsPipingSurfaceLine"/> related to creating
    /// an <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLineCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLineEntity"/> based on the information of the <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>a new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static SurfaceLineEntity Create(this RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new SurfaceLineEntity
            {
                Name = surfaceLine.Name,
                ReferenceLineIntersectionX = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.X),
                ReferenceLineIntersectionY = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.Y)
            };
            CreateSurfaceLinePointEntities(surfaceLine, collector, entity);
            CreateCharacteristicPointEntities(surfaceLine, collector, entity);

            collector.Register(entity, surfaceLine);

            return entity;
        }

        private static void CreateSurfaceLinePointEntities(RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry collector, SurfaceLineEntity entity)
        {
            int order = 0;
            foreach (Point3D point3D in surfaceLine.Points)
            {
                entity.SurfaceLinePointEntities.Add(point3D.CreateSurfaceLinePoint(collector, order++));
            }
        }

        private static void CreateCharacteristicPointEntities(RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry collector, SurfaceLineEntity entity)
        {
            if (surfaceLine.BottomDitchPolderSide != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.BottomDitchPolderSide);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.BottomDitchPolderSide
                });
            }
            if (surfaceLine.BottomDitchDikeSide != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.BottomDitchDikeSide);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.BottomDitchDikeSide
                });
            }
            if (surfaceLine.DikeToeAtPolder != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.DikeToeAtPolder);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtPolder
                });
            }
            if (surfaceLine.DikeToeAtRiver != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.DikeToeAtRiver);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.DikeToeAtRiver
                });
            }
            if (surfaceLine.DitchDikeSide != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.DitchDikeSide);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.DitchDikeSide
                });
            }
            if (surfaceLine.DitchPolderSide != null)
            {
                SurfaceLinePointEntity characteristicPointEntity = collector.GetSurfaceLinePoint(surfaceLine.DitchPolderSide);
                characteristicPointEntity.CharacteristicPointEntities.Add(new CharacteristicPointEntity
                {
                    CharacteristicPointType = (short)CharacteristicPointType.DitchPolderSide
                });
            }
        }
    }
}