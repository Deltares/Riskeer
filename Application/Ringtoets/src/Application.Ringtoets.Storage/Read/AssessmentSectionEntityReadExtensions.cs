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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="AssessmentSection"/> based on the
    /// <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    internal static class AssessmentSectionEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="AssessmentSectionEntity"/> and use the information to construct a <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> to create <see cref="AssessmentSection"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="AssessmentSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        internal static AssessmentSection Read(this AssessmentSectionEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var assessmentSection = new AssessmentSection((AssessmentSectionComposition)entity.Composition)
            {
                StorageId = entity.AssessmentSectionEntityId,
                Name = entity.Name
            };

            entity.ReadPipingFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverErosionInwardsFailureMechanism(assessmentSection);
            entity.ReadHydraulicDatabase(assessmentSection);
            entity.ReadReferenceLine(assessmentSection);
            entity.ReadStandAloneFailureMechanisms(assessmentSection);

            return assessmentSection;
        }

        private static void ReadReferenceLine(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            if (entity.ReferenceLinePointEntities.Any())
            {
                assessmentSection.ReferenceLine = new ReferenceLine();
                assessmentSection.ReferenceLine.SetGeometry(entity.ReferenceLinePointEntities.OrderBy(rlpe => rlpe.Order).Select(rlpe => rlpe.Read()));
            }
        }

        private static void ReadHydraulicDatabase(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            if (entity.HydraulicDatabaseLocation != null)
            {
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = entity.HydraulicDatabaseLocation,
                    Version = entity.HydraulicDatabaseVersion
                };

                foreach (var hydraulicLocationEntity in entity.HydraulicLocationEntities)
                {
                    assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicLocationEntity.Read());
                }
            }
        }

        private static void ReadPipingFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var pipingFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.Piping);
            if (pipingFailureMechanismEntity != null)
            {
                var failureMechanism = pipingFailureMechanismEntity.ReadAsPipingFailureMechanism(collector);

                var pipingFailureMechanism = assessmentSection.PipingFailureMechanism;
                pipingFailureMechanism.StochasticSoilModels.AddRange(failureMechanism.StochasticSoilModels);
                pipingFailureMechanism.IsRelevant = failureMechanism.IsRelevant;
                pipingFailureMechanism.StorageId = failureMechanism.StorageId;
                foreach (var failureMechanismSection in failureMechanism.Sections)
                {
                    pipingFailureMechanism.AddSection(failureMechanismSection);
                }
            }
        }

        private static void ReadGrassCoverErosionInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection)
        {
            var grassCoverErosionInwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int)FailureMechanismType.GrassRevetmentTopErosionAndInwards);
            if (grassCoverErosionInwardsFailureMechanismEntity != null)
            {
                var failureMechanism = grassCoverErosionInwardsFailureMechanismEntity.ReadAsGrassCoverErosionInwardsFailureMechanism();

                var grassCoverErosionInwards = assessmentSection.GrassCoverErosionInwards;
                grassCoverErosionInwards.IsRelevant = failureMechanism.IsRelevant;
                grassCoverErosionInwards.StorageId = failureMechanism.StorageId;
                foreach (var failureMechanismSection in failureMechanism.Sections)
                {
                    grassCoverErosionInwards.AddSection(failureMechanismSection);
                }
            }
        }

        private static void ReadStandAloneFailureMechanisms(this AssessmentSectionEntity entity, AssessmentSection assessmentSection)
        {
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.MacrostabilityInwards, assessmentSection.MacrostabilityInwards);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.StabilityStoneRevetment, assessmentSection.StabilityStoneCover);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.WaveImpactOnAsphaltRevetment, assessmentSection.WaveImpactAsphaltCover);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.GrassRevetmentErosionOutwards, assessmentSection.GrassCoverErosionOutside);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.GrassRevetmentSlidingOutwards, assessmentSection.GrassCoverSlipOffOutside);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.StructureHeight, assessmentSection.HeightStructure);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.ReliabilityClosingOfStructure, assessmentSection.ClosingStructure);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.PipingAtStructure, assessmentSection.PipingStructure);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.StrengthAndStabilityPointConstruction, assessmentSection.StrengthStabilityPointConstruction);
            entity.ReadStandAloneFailureMechanism(FailureMechanismType.DuneErosion, assessmentSection.DuneErosion);
        }

        private static void ReadStandAloneFailureMechanism(this AssessmentSectionEntity entity, FailureMechanismType failureMechanismType, StandAloneFailureMechanism standAloneFailureMechanism)
        {
            var failureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) failureMechanismType);
            if (failureMechanismEntity != null)
            {
                var failureMechanism = failureMechanismEntity.ReadAsStandAloneFailureMechanism();

                standAloneFailureMechanism.StorageId = failureMechanism.StorageId;
                standAloneFailureMechanism.IsRelevant = failureMechanism.IsRelevant;
                foreach (var failureMechanismSection in failureMechanism.Sections)
                {
                    standAloneFailureMechanism.AddSection(failureMechanismSection);
                }
            }
        }
    }
}