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
using System.Collections.Generic;
using System.Linq;

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Update.Piping
{
    /// <summary>
    /// Extension methods for <see cref="RingtoetsPipingSurfaceLine"/> related to updating
    /// an <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLineUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="SurfaceLineEntity"/> in the database based on the information
        /// of the <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The piping surfaceline to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntryPointNotFoundException">When no <see cref="SurfaceLineEntity"/>
        /// can be found in <paramref name="context"/> that matches <paramref name="surfaceLine"/>.</exception>
        internal static void Update(this RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SurfaceLineEntity entity = surfaceLine.GetCorrespondingEntity(
                context.SurfaceLineEntities,
                context);

            entity.Name = surfaceLine.Name;
            entity.ReferenceLineIntersectionX = surfaceLine.ReferenceLineIntersectionWorldPoint.X.ToNaNAsNull();
            entity.ReferenceLineIntersectionY = surfaceLine.ReferenceLineIntersectionWorldPoint.Y.ToNaNAsNull();

            UpdateGeometry(surfaceLine, entity, registry);
            UpdateCharacteristicPoints(surfaceLine, entity, registry);

            registry.Register(entity, surfaceLine);
        }

        private static void UpdateGeometry(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, PersistenceRegistry registry)
        {
            var newBinaryData = new Point3DBinaryConverter().ToBytes(surfaceLine.Points);
            if (!BinaryDataEqualityHelper.AreEqual(entity.PointsData, newBinaryData))
            {
                entity.PointsData = newBinaryData;
            }
        }

        private static void UpdateCharacteristicPoints(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, PersistenceRegistry registry)
        {
            var characteristicPointAssociations = new[]
            {
                Tuple.Create(surfaceLine.BottomDitchPolderSide, CharacteristicPointType.BottomDitchPolderSide),
                Tuple.Create(surfaceLine.BottomDitchDikeSide, CharacteristicPointType.BottomDitchDikeSide),
                Tuple.Create(surfaceLine.DikeToeAtPolder, CharacteristicPointType.DikeToeAtPolder),
                Tuple.Create(surfaceLine.DikeToeAtRiver, CharacteristicPointType.DikeToeAtRiver),
                Tuple.Create(surfaceLine.DitchDikeSide, CharacteristicPointType.DitchDikeSide),
                Tuple.Create(surfaceLine.DitchPolderSide, CharacteristicPointType.DitchPolderSide)
            };

            // Add new items at the end to optimize performance during the lookup of existing points
            var characteristicPointEntitiesToAdd = new List<CharacteristicPointEntity>();

            foreach (Tuple<Point3D, CharacteristicPointType> characteristicPointAssociation in characteristicPointAssociations)
            {
                short typeValue = (short)characteristicPointAssociation.Item2;
                var characteristicPointEntity = entity.CharacteristicPointEntities.FirstOrDefault(cpe => cpe.Type == typeValue);
                if (characteristicPointAssociation.Item1 == null)
                {
                    if (characteristicPointEntity != null)
                    {
                        entity.CharacteristicPointEntities.Remove(characteristicPointEntity);
                    }
                }
                else
                {
                    double? xValue = characteristicPointAssociation.Item1.X.ToNaNAsNull();
                    double? yValue = characteristicPointAssociation.Item1.Y.ToNaNAsNull();
                    double? zValue = characteristicPointAssociation.Item1.Z.ToNaNAsNull();

                    if (characteristicPointEntity == null)
                    {
                        var newEntity = new CharacteristicPointEntity
                        {
                            Type = typeValue,
                            X = xValue,
                            Y = yValue,
                            Z = zValue
                        };
                        characteristicPointEntitiesToAdd.Add(newEntity);
                        registry.Register(newEntity, characteristicPointAssociation.Item1);
                    }
                    else if (characteristicPointEntity.X.Equals(xValue) &&
                             characteristicPointEntity.Y.Equals(yValue) &&
                             characteristicPointEntity.Z.Equals(zValue))
                    {
                        registry.Register(characteristicPointEntity, characteristicPointAssociation.Item1);
                    }
                    else
                    {
                        characteristicPointEntity.X = xValue;
                        characteristicPointEntity.Y = yValue;
                        characteristicPointEntity.Z = zValue;

                        registry.Register(characteristicPointEntity, characteristicPointAssociation.Item1);
                    }
                }

                foreach (CharacteristicPointEntity characteristicPointEntityToAdd in characteristicPointEntitiesToAdd)
                {
                    entity.CharacteristicPointEntities.Add(characteristicPointEntityToAdd);
                }
            }
        }
    }
}