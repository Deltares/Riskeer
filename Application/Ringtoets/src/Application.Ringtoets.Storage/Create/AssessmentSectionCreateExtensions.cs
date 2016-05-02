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
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="AssessmentSection"/> related to creating an <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    public static class AssessmentSectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="AssessmentSectionEntity"/> based on the information of the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="section">The section to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static AssessmentSectionEntity Create(this AssessmentSection section, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new AssessmentSectionEntity
            {
                Name = section.Name,
                Composition = (short) section.Composition
            };

            CreatePipingFailureMechanism(section, entity, collector);
            CreateGrassCoverErosionInwardsFailureMechanism(section, entity, collector);
            CreateHydraulicDatabase(section, entity, collector);
            CreateReferenceLine(section, entity);
            CreateFailureMechanismPlaceHolders(section, entity, collector);

            collector.Create(entity, section);
            return entity;
        }

        private static void CreateGrassCoverErosionInwardsFailureMechanism(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionInwards.Create(collector));
        }

        private static void CreateFailureMechanismPlaceHolders(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            entity.FailureMechanismEntities.Add(section.MacrostabilityInwards.Create(FailureMechanismType.MacrostabilityInwards, collector));
            entity.FailureMechanismEntities.Add(section.Overtopping.Create(FailureMechanismType.StructureHeight, collector));
            entity.FailureMechanismEntities.Add(section.Closing.Create(FailureMechanismType.ReliabilityClosingOfStructure, collector));
            entity.FailureMechanismEntities.Add(section.FailingOfConstruction.Create(FailureMechanismType.StrengthAndStabilityPointConstruction, collector));
            entity.FailureMechanismEntities.Add(section.StoneRevetment.Create(FailureMechanismType.StabilityStoneRevetment, collector));
            entity.FailureMechanismEntities.Add(section.AsphaltRevetment.Create(FailureMechanismType.WaveImpactOnAsphaltRevetment, collector));
            entity.FailureMechanismEntities.Add(section.GrassRevetment.Create(FailureMechanismType.GrassRevetmentErosionOutwards, collector));
            entity.FailureMechanismEntities.Add(section.DuneErosion.Create(FailureMechanismType.DuneErosion, collector));
        }

        private static void CreatePipingFailureMechanism(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            entity.FailureMechanismEntities.Add(section.PipingFailureMechanism.Create(collector));
        }

        private static void CreateReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
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

        private static void CreateHydraulicDatabase(AssessmentSection section, AssessmentSectionEntity entity, CreateConversionCollector collector)
        {
            if (section.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = section.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = section.HydraulicBoundaryDatabase.Version;

                foreach (var hydraulicBoundaryLocation in section.HydraulicBoundaryDatabase.Locations)
                {
                    entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(collector));
                }
            }
        }
    }
}