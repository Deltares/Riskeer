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
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Application.Ringtoets.Storage.Update;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="AssessmentSection"/> related to updating an <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    public static class AssessmentSectionUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="AssessmentSectionEntity"/> in the database based on the information of the 
        /// <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="section">The section to update the database entity for.</param>
        /// <param name="collector">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        public static void Update(this AssessmentSection section, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = GetSingleAssessmentSection(section, context);
            entity.Name = section.Name;
            entity.Composition = (short) section.Composition;

            UpdatePipingFailureMechanism(section, collector, context);
            UpdateHydraulicDatabase(section, entity, collector, context);
            UpdateReferenceLine(section, entity, context);
            UpdateFailureMechanismPlaceHolders(section, collector, context);

            collector.Update(entity);
        }

        private static void UpdateFailureMechanismPlaceHolders(AssessmentSection section, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            section.MacrostabilityInwards.Update(collector, context);
            section.Overtopping.Update(collector, context);
            section.Closing.Update(collector, context);
            section.FailingOfConstruction.Update(collector, context);
            section.StoneRevetment.Update(collector, context);
            section.AsphaltRevetment.Update(collector, context);
            section.GrassRevetment.Update(collector, context);
            section.DuneErosion.Update(collector, context);
        }

        private static AssessmentSectionEntity GetSingleAssessmentSection(AssessmentSection section, IRingtoetsEntities context)
        {
            try {
                return context.AssessmentSectionEntities.Single(ase => ase.AssessmentSectionEntityId == section.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(AssessmentSectionEntity).Name, section.StorageId), exception);
            }
        }

        private static void UpdatePipingFailureMechanism(AssessmentSection section, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            section.PipingFailureMechanism.Update(collector, context);
        }

        private static void UpdateReferenceLine(AssessmentSection section, AssessmentSectionEntity entity, IRingtoetsEntities context)
        {
            if (HasChanges(entity.ReferenceLinePointEntities, section.ReferenceLine))
            {
                context.ReferenceLinePointEntities.RemoveRange(entity.ReferenceLinePointEntities);
                UpdateReferenceLinePoints(section, entity);
            }
        }

        private static void UpdateReferenceLinePoints(AssessmentSection section, AssessmentSectionEntity entity)
        {
            if (section.ReferenceLine != null)
            {
                var i = 0;
                foreach (var point2D in section.ReferenceLine.Points)
                {
                    entity.ReferenceLinePointEntities.Add(point2D.CreateReferenceLinePoint(i++));
                }
            }
        }

        private static void UpdateHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                foreach (var hydraulicBoundaryLocation in section.HydraulicBoundaryDatabase.Locations)
                {
                    if (hydraulicBoundaryLocation.IsNew())
                    {
                        entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(collector));
                    }
                    else
                    {
                        hydraulicBoundaryLocation.Update(collector, context);
                    }
                }
            }
        }

        private static bool HasChanges(ICollection<ReferenceLinePointEntity> existingLine, ReferenceLine otherLine)
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
            var otherPointsArray = otherLine.Points.ToArray();
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