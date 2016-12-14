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
using Application.Ringtoets.Storage.Create.ClosingStructures;
using Application.Ringtoets.Storage.Create.DuneErosion;
using Application.Ringtoets.Storage.Create.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.Create.GrassCoverErosionOutwards;
using Application.Ringtoets.Storage.Create.GrassCoverSlipOffInwards;
using Application.Ringtoets.Storage.Create.GrassCoverSlipOffOutwards;
using Application.Ringtoets.Storage.Create.HeightStructures;
using Application.Ringtoets.Storage.Create.MacrostabilityInwards;
using Application.Ringtoets.Storage.Create.MacrostabilityOutwards;
using Application.Ringtoets.Storage.Create.Microstability;
using Application.Ringtoets.Storage.Create.Piping;
using Application.Ringtoets.Storage.Create.PipingStructure;
using Application.Ringtoets.Storage.Create.StabilityPointStructures;
using Application.Ringtoets.Storage.Create.StabilityStoneCover;
using Application.Ringtoets.Storage.Create.StrengthStabilityLengthwise;
using Application.Ringtoets.Storage.Create.TechnicalInnovation;
using Application.Ringtoets.Storage.Create.WaterPressureAsphaltCover;
using Application.Ringtoets.Storage.Create.WaveImpactAsphaltCover;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Utils.Extensions;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="AssessmentSection"/> related to creating an <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    internal static class AssessmentSectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="AssessmentSectionEntity"/> based on the information of the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="section">The section to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="section"/> resides in its parent.</param>
        /// <returns>A new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static AssessmentSectionEntity Create(this AssessmentSection section, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new AssessmentSectionEntity
            {
                Id = section.Id.DeepClone(),
                Name = section.Name.DeepClone(),
                Composition = (byte) section.Composition,
                Comments = section.Comments.Body.DeepClone(),
                Norm = section.FailureMechanismContribution.Norm,
                Order = order
            };

            AddEntityForHydraulicDatabase(section, entity, registry);
            AddEntityForReferenceLine(section, entity);

            entity.FailureMechanismEntities.Add(section.PipingFailureMechanism.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.HeightStructures.Create(registry));
            entity.FailureMechanismEntities.Add(section.StrengthStabilityLengthwiseConstruction.Create(registry));
            entity.FailureMechanismEntities.Add(section.TechnicalInnovation.Create(registry));
            entity.FailureMechanismEntities.Add(section.WaterPressureAsphaltCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.ClosingStructures.Create(registry));
            entity.FailureMechanismEntities.Add(section.MacrostabilityInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.MacrostabilityOutwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.WaveImpactAsphaltCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionOutwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverSlipOffInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverSlipOffOutwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.Microstability.Create(registry));
            entity.FailureMechanismEntities.Add(section.PipingStructure.Create(registry));
            entity.FailureMechanismEntities.Add(section.StabilityStoneCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.DuneErosion.Create(registry));
            entity.FailureMechanismEntities.Add(section.StabilityPointStructures.Create(registry));

            return entity;
        }

        private static void AddEntityForReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
        {
            if (section.ReferenceLine != null)
            {
                entity.ReferenceLinePointXml = new Point2DXmlSerializer().ToXml(section.ReferenceLine.Points);
            }
        }

        private static void AddEntityForHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, PersistenceRegistry registry)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath.DeepClone();
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version.DeepClone();

                for (var i = 0; i < section.HydraulicBoundaryDatabase.Locations.Count; i++)
                {
                    var hydraulicBoundaryLocation = section.HydraulicBoundaryDatabase.Locations[i];
                    entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(registry, i));
                }
            }
        }
    }
}