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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Update
{
    internal static class FailureMechanismSectionUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="AssessmentSectionEntity"/> in the database based on the information of the 
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The section to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        internal static void Update(this FailureMechanismSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            FailureMechanismSectionEntity entity = GetCorrespondingFailureMechanismSectionEntity(section, context);
            entity.Name = section.Name;

            UpdateGeometry(section, entity, context);

            registry.Register(entity, section);
        }

        private static FailureMechanismSectionEntity GetCorrespondingFailureMechanismSectionEntity(FailureMechanismSection section, IRingtoetsEntities context)
        {
            try
            {
                return context.FailureMechanismSectionEntities.Single(ase => ase.FailureMechanismSectionEntityId == section.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(FailureMechanismSectionEntity).Name, section.StorageId), exception);
            }
        }

        private static void UpdateGeometry(FailureMechanismSection section, FailureMechanismSectionEntity entity, IRingtoetsEntities context)
        {
            if (HasChanges(entity.FailureMechanismSectionPointEntities, section.Points))
            {
                context.FailureMechanismSectionPointEntities.RemoveRange(entity.FailureMechanismSectionPointEntities);
                UpdateGeometryPoints(section, entity);
            }
        }

        private static void UpdateGeometryPoints(FailureMechanismSection section, FailureMechanismSectionEntity entity)
        {
            var i = 0;
            foreach (var point2D in section.Points)
            {
                entity.FailureMechanismSectionPointEntities.Add(point2D.CreateFailureMechanismSectionPointEntity(i++));
            }
        }

        private static bool HasChanges(ICollection<FailureMechanismSectionPointEntity> existingLine, IEnumerable<Point2D> otherLine)
        {
            if (!existingLine.Any())
            {
                return otherLine != null;
            }
            if (otherLine == null)
            {
                return true;
            }

            var existingPointEntities = existingLine.OrderBy(rlpe => rlpe.Order).ToArray();
            var otherPointsArray = otherLine.ToArray();
            if (existingPointEntities.Length != otherPointsArray.Length)
            {
                return true;
            }
            for (int i = 0; i < existingPointEntities.Length; i++)
            {
                var isXAlmostEqual = Math.Abs(Convert.ToDouble(existingPointEntities[i].X) - otherPointsArray[i].X) < 1e-6;
                var isYAlmostEqual = Math.Abs(Convert.ToDouble(existingPointEntities[i].Y) - otherPointsArray[i].Y) < 1e-6;
                if (!isXAlmostEqual || !isYAlmostEqual)
                {
                    return true;
                }
            }
            return false;
        }
    }
}