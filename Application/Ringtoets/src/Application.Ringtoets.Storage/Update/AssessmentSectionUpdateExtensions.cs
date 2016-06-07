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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="AssessmentSection"/> related to updating an <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    internal static class AssessmentSectionUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="AssessmentSectionEntity"/> in the database based on the information of the 
        /// <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="section">The section to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="section"/>
        /// does not have a corresponding entity in <paramref name="context"/>.</exception>
        internal static void Update(this AssessmentSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            AssessmentSectionEntity entity = GetCorrespondingAssessmentSectionEntity(section, context);
            entity.Name = section.Name;
            entity.Composition = (short) section.Composition;
            entity.Comments = section.Comments;
            entity.Norm = section.FailureMechanismContribution.Norm;

            UpdateHydraulicDatabase(section, entity, registry, context);
            UpdateReferenceLine(section, entity, context);

            UpdatePipingFailureMechanism(section, registry, context);
            UpdateGrassCoverErosionInwardsFailureMechanism(section, registry, context);
            UpdateStandAloneFailureMechanisms(section, registry, context);

            registry.Register(entity, section);
        }

        private static void UpdateStandAloneFailureMechanisms(AssessmentSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            section.MacrostabilityInwards.Update(registry, context);
            section.MacrostabilityOutwards.Update(registry, context);
            section.Microstability.Update(registry, context);
            section.StabilityStoneCover.Update(registry, context);
            section.WaveImpactAsphaltCover.Update(registry, context);
            section.WaterPressureAsphaltCover.Update(registry, context);
            section.GrassCoverErosionOutwards.Update(registry, context);
            section.GrassCoverSlipOffOutwards.Update(registry, context);
            section.GrassCoverSlipOffInwards.Update(registry, context);
            section.HeightStructures.Update(registry, context);
            section.ClosingStructure.Update(registry, context);
            section.PipingStructure.Update(registry, context);
            section.StrengthStabilityPointConstruction.Update(registry, context);
            section.StrengthStabilityLengthwiseConstruction.Update(registry, context);
            section.DuneErosion.Update(registry, context);
            section.TechnicalInnovation.Update(registry, context);
        }

        private static AssessmentSectionEntity GetCorrespondingAssessmentSectionEntity(AssessmentSection section, IRingtoetsEntities context)
        {
            try
            {
                return context.AssessmentSectionEntities.Single(ase => ase.AssessmentSectionEntityId == section.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(AssessmentSectionEntity).Name, section.StorageId), exception);
            }
        }

        private static void UpdatePipingFailureMechanism(AssessmentSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            section.PipingFailureMechanism.Update(registry, context);
        }

        private static void UpdateGrassCoverErosionInwardsFailureMechanism(AssessmentSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            section.GrassCoverErosionInwards.Update(registry, context);
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
                    entity.ReferenceLinePointEntities.Add(point2D.CreateReferenceLinePointEntity(i++));
                }
            }
        }

        private static void UpdateHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                foreach (var hydraulicBoundaryLocation in section.HydraulicBoundaryDatabase.Locations)
                {
                    if (hydraulicBoundaryLocation.IsNew())
                    {
                        entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(registry));
                    }
                    else
                    {
                        hydraulicBoundaryLocation.Update(registry, context);
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
            for (var i = 0; i < existingPointEntities.Length; i++)
            {
                var existingPoint = new Point2D(Convert.ToDouble(existingPointEntities[i].X), Convert.ToDouble(existingPointEntities[i].Y));

                if (!Math2D.AreEqualPoints(existingPoint, otherPointsArray[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}