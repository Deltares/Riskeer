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
        /// <returns>A new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static AssessmentSectionEntity Create(this AssessmentSection section, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            var entity = new AssessmentSectionEntity
            {
                Id = section.Id,
                Name = section.Name,
                Composition = (short) section.Composition,
                Comments = section.Comments,
                Norm = section.FailureMechanismContribution.Norm
            };

            AddEntityForHydraulicDatabase(section, entity, registry);
            AddEntityForReferenceLine(section, entity);

            entity.FailureMechanismEntities.Add(section.PipingFailureMechanism.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.HeightStructures.Create(registry));
            entity.FailureMechanismEntities.Add(section.StrengthStabilityLengthwiseConstruction.Create(registry));
            entity.FailureMechanismEntities.Add(section.TechnicalInnovation.Create(registry));
            entity.FailureMechanismEntities.Add(section.WaterPressureAsphaltCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.ClosingStructure.Create(registry));
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
            entity.FailureMechanismEntities.Add(section.StrengthStabilityPointConstruction.Create(registry));

            registry.Register(entity, section);
            return entity;
        }

        private static void AddEntityForReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
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

        private static void AddEntityForHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, PersistenceRegistry registry)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                foreach (var hydraulicBoundaryLocation in section.HydraulicBoundaryDatabase.Locations)
                {
                    entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(registry));
                }
            }
        }
    }
}