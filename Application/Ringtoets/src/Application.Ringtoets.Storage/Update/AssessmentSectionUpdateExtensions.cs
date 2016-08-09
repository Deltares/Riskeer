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

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Serializers;
using Application.Ringtoets.Storage.Update.ClosingStructure;
using Application.Ringtoets.Storage.Update.DuneErosion;
using Application.Ringtoets.Storage.Update.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.Update.GrassCoverErosionOutwards;
using Application.Ringtoets.Storage.Update.GrassCoverSlipOffInwards;
using Application.Ringtoets.Storage.Update.GrassCoverSlipOffOutwards;
using Application.Ringtoets.Storage.Update.HeightStructures;
using Application.Ringtoets.Storage.Update.MacrostabilityInwards;
using Application.Ringtoets.Storage.Update.MacrostabilityOutwards;
using Application.Ringtoets.Storage.Update.Microstability;
using Application.Ringtoets.Storage.Update.Piping;
using Application.Ringtoets.Storage.Update.PipingStructure;
using Application.Ringtoets.Storage.Update.StabilityStoneCover;
using Application.Ringtoets.Storage.Update.StrengthStabilityLengthwiseConstruction;
using Application.Ringtoets.Storage.Update.StrengthStabilityPointConstruction;
using Application.Ringtoets.Storage.Update.TechnicalInnovation;
using Application.Ringtoets.Storage.Update.WaterPressureAsphaltCover;
using Application.Ringtoets.Storage.Update.WaveImpactAsphaltCover;

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
        /// does not have a corresponding entity in the database.</exception>
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

            AssessmentSectionEntity entity = section.GetCorrespondingEntity(context.AssessmentSectionEntities, context);
            entity.Id = section.Id;
            entity.Name = section.Name;
            entity.Composition = (short) section.Composition;
            entity.Comments = section.Comments;
            entity.Norm = section.FailureMechanismContribution.Norm;

            UpdateHydraulicDatabase(section, entity, registry, context);
            UpdateReferenceLine(section, entity);
            UpdateFailureMechanisms(section, registry, context);

            registry.Register(entity, section);
        }

        private static void UpdateFailureMechanisms(AssessmentSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            PipingFailureMechanismUpdateExtensions.Update(section.PipingFailureMechanism, registry, context);
            GrassCoverErosionInwardsFailureMechanismUpdateExtensions.Update(section.GrassCoverErosionInwards, registry, context);
            MacrostabilityInwardsFailureMechanismUpdateExtensions.Update(section.MacrostabilityInwards, registry, context);
            MacrostabilityOutwardsFailureMechanismUpdateExtensions.Update(section.MacrostabilityOutwards, registry, context);
            MicrostabilityFailureMechanismUpdateExtensions.Update(section.Microstability, registry, context);
            StabilityStoneCoverFailureMechanismUpdateExtensions.Update(section.StabilityStoneCover, registry, context);
            WaveImpactAsphaltCoverFailureMechanismUpdateExtensions.Update(section.WaveImpactAsphaltCover, registry, context);
            WaterPressureAsphaltCoverFailureMechanismUpdateExtensions.Update(section.WaterPressureAsphaltCover, registry, context);
            GrassCoverErosionOutwardsFailureMechanismUpdateExtensions.Update(section.GrassCoverErosionOutwards, registry, context);
            GrassCoverSlipOffOutwardsFailureMechanismUpdateExtensions.Update(section.GrassCoverSlipOffOutwards, registry, context);
            GrassCoverSlipOffInwardsFailureMechanismUpdateExtensions.Update(section.GrassCoverSlipOffInwards, registry, context);
            HeightStructuresFailureMechanismUpdateExtensions.Update(section.HeightStructures, registry, context);
            ClosingStructureFailureMechanismUpdateExtensions.Update(section.ClosingStructure, registry, context);
            PipingStructureFailureMechanismUpdateExtensions.Update(section.PipingStructure, registry, context);
            StrengthStabilityPointConstructionFailureMechanismUpdateExtensions.Update(section.StrengthStabilityPointConstruction, registry, context);
            StrengthStabilityLengthwiseConstructionFailureMechanismUpdateExtensions.Update(section.StrengthStabilityLengthwiseConstruction, registry, context);
            DuneErosionFailureMechanismUpdateExtensions.Update(section.DuneErosion, registry, context);
            TechnicalInnovationFailureMechanismUpdateExtensions.Update(section.TechnicalInnovation, registry, context);
        }

        private static void UpdateReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
        {
            if (section.ReferenceLine == null)
            {
                entity.ReferenceLinePointXml = null;
            }
            else
            {
                string newXml = new Point2DXmlSerializer().ToXml(section.ReferenceLine.Points);
                if (entity.ReferenceLinePointXml == null || !entity.ReferenceLinePointXml.Equals(newXml))
                {
                    entity.ReferenceLinePointXml = newXml;
                }
            }
        }

        private static void UpdateHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                for (int i = 0; i < section.HydraulicBoundaryDatabase.Locations.Count; i++)
                {
                    var hydraulicBoundaryLocation = section.HydraulicBoundaryDatabase.Locations[i];
                    if (hydraulicBoundaryLocation.IsNew())
                    {
                        entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(registry, i));
                    }
                    else
                    {
                        hydraulicBoundaryLocation.Update(registry, context);
                    }
                }
            }
        }
    }
}