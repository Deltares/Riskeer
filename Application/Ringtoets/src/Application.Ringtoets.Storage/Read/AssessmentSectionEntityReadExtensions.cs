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
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

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

            var assessmentSection = new AssessmentSection((AssessmentSectionComposition) entity.Composition)
            {
                Id = entity.Id,
                Name = entity.Name,
                Comments = entity.Comments,
                FailureMechanismContribution =
                {
                    Norm = entity.Norm
                }
            };

            entity.ReadHydraulicDatabase(assessmentSection, collector);
            entity.ReadReferenceLine(assessmentSection);

            entity.ReadPipingFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverErosionInwardsFailureMechanism(assessmentSection, collector);
            entity.ReadHeightStructuresFailureMechanism(assessmentSection, collector);
            entity.ReadStrengthStabilityLengthwiseConstructionFailureMechanism(assessmentSection, collector);
            entity.ReadTechnicalInnovationFailureMechanism(assessmentSection, collector);
            entity.ReadWaterPressureAsphaltCoverFailureMechanism(assessmentSection, collector);
            entity.ReadClosingStructuresFailureMechanism(assessmentSection, collector);
            entity.ReadMacrostabilityInwardsFailureMechanism(assessmentSection, collector);
            entity.ReadMacrostabilityOutwardsFailureMechanism(assessmentSection, collector);
            entity.ReadWaveImpactAsphaltCoverFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverErosionOutwardsFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverSlipOffInwardsFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverSlipOffOutwardsFailureMechanism(assessmentSection, collector);
            entity.ReadMicrostabilityFailureMechanism(assessmentSection, collector);
            entity.ReadPipingStructureFailureMechanism(assessmentSection, collector);
            entity.ReadDuneErosionFailureMechanism(assessmentSection, collector);
            entity.ReadStabilityStoneCoverFailureMechanism(assessmentSection, collector);
            entity.ReadStabilityPointStructuresFailureMechanism(assessmentSection, collector);

            return assessmentSection;
        }

        private static void ReadReferenceLine(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            if (entity.ReferenceLinePointXml != null)
            {
                Point2D[] points = new Point2DXmlSerializer().FromXml(entity.ReferenceLinePointXml);

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(points);

                assessmentSection.ReferenceLine = referenceLine;
            }
        }

        private static void ReadHydraulicDatabase(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            if (entity.HydraulicDatabaseLocation != null)
            {
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = entity.HydraulicDatabaseLocation,
                    Version = entity.HydraulicDatabaseVersion
                };

                foreach (var hydraulicLocationEntity in entity.HydraulicLocationEntities.OrderBy(hl => hl.Order))
                {
                    assessmentSection.HydraulicBoundaryDatabase.Locations.Add(hydraulicLocationEntity.Read(collector));
                }
            }
        }

        private static void ReadPipingFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var pipingFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.Piping);
            if (pipingFailureMechanismEntity != null)
            {
                pipingFailureMechanismEntity.ReadAsPipingFailureMechanism(assessmentSection.PipingFailureMechanism, collector);
            }
        }

        private static void ReadGrassCoverErosionInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverErosionInwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.GrassRevetmentTopErosionAndInwards);
            if (grassCoverErosionInwardsFailureMechanismEntity != null)
            {
                grassCoverErosionInwardsFailureMechanismEntity.ReadAsGrassCoverErosionInwardsFailureMechanism(assessmentSection.GrassCoverErosionInwards, collector);
            }
        }

        private static void ReadHeightStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var heightStructuresFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.StructureHeight);
            if (heightStructuresFailureMechanismEntity != null)
            {
                heightStructuresFailureMechanismEntity.ReadAsHeightStructuresFailureMechanism(assessmentSection.HeightStructures, collector);
            }
        }

        private static void ReadStrengthStabilityLengthwiseConstructionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var strengthStabilityLengthwiseConstructionFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.StrengthAndStabilityParallelConstruction);
            if (strengthStabilityLengthwiseConstructionFailureMechanismEntity != null)
            {
                strengthStabilityLengthwiseConstructionFailureMechanismEntity.ReadAsStrengthStabilityLengthwiseConstructionFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction, collector);
            }
        }

        private static void ReadTechnicalInnovationFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var technicalInnovationFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.TechnicalInnovations);
            if (technicalInnovationFailureMechanismEntity != null)
            {
                technicalInnovationFailureMechanismEntity.ReadAsTechnicalInnovationFailureMechanism(assessmentSection.TechnicalInnovation, collector);
            }
        }

        private static void ReadWaterPressureAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var waterPressureAsphaltCoverFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.WaterOverpressureAsphaltRevetment);
            if (waterPressureAsphaltCoverFailureMechanismEntity != null)
            {
                waterPressureAsphaltCoverFailureMechanismEntity.ReadAsWaterPressureAsphaltCoverFailureMechanism(assessmentSection.WaterPressureAsphaltCover, collector);
            }
        }

        private static void ReadClosingStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var closingStructuresFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.ReliabilityClosingOfStructure);
            if (closingStructuresFailureMechanismEntity != null)
            {
                closingStructuresFailureMechanismEntity.ReadAsClosingStructuresFailureMechanism(assessmentSection.ClosingStructures, collector);
            }
        }

        private static void ReadMacrostabilityInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var macrostabilityInwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.MacrostabilityInwards);
            if (macrostabilityInwardsFailureMechanismEntity != null)
            {
                macrostabilityInwardsFailureMechanismEntity.ReadAsMacrostabilityInwardsFailureMechanism(assessmentSection.MacrostabilityInwards, collector);
            }
        }

        private static void ReadMacrostabilityOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var macrostabilityOutwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.MacrostabilityOutwards);
            if (macrostabilityOutwardsFailureMechanismEntity != null)
            {
                macrostabilityOutwardsFailureMechanismEntity.ReadAsMacrostabilityOutwardsFailureMechanism(assessmentSection.MacrostabilityOutwards, collector);
            }
        }

        private static void ReadWaveImpactAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var waveImpactAsphaltCoverFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.WaveImpactOnAsphaltRevetment);
            if (waveImpactAsphaltCoverFailureMechanismEntity != null)
            {
                waveImpactAsphaltCoverFailureMechanismEntity.ReadAsWaveImpactAsphaltCoverFailureMechanism(assessmentSection.WaveImpactAsphaltCover, collector);
            }
        }

        private static void ReadGrassCoverErosionOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverErosionOutwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.GrassRevetmentErosionOutwards);
            if (grassCoverErosionOutwardsFailureMechanismEntity != null)
            {
                grassCoverErosionOutwardsFailureMechanismEntity.ReadAsGrassCoverErosionOutwardsFailureMechanism(assessmentSection.GrassCoverErosionOutwards, collector);
            }
        }

        private static void ReadGrassCoverSlipOffInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverSlipOffInwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.GrassRevetmentSlidingInwards);
            if (grassCoverSlipOffInwardsFailureMechanismEntity != null)
            {
                grassCoverSlipOffInwardsFailureMechanismEntity.ReadAsGrassCoverSlipOffInwardsFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, collector);
            }
        }

        private static void ReadGrassCoverSlipOffOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverSlipOffOutwardsFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.GrassRevetmentSlidingOutwards);
            if (grassCoverSlipOffOutwardsFailureMechanismEntity != null)
            {
                grassCoverSlipOffOutwardsFailureMechanismEntity.ReadAsGrassCoverSlipOffOutwardsFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, collector);
            }
        }

        private static void ReadMicrostabilityFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var microstabilityFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.Microstability);
            if (microstabilityFailureMechanismEntity != null)
            {
                microstabilityFailureMechanismEntity.ReadAsMicrostabilityFailureMechanism(assessmentSection.Microstability, collector);
            }
        }

        private static void ReadPipingStructureFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var pipingStructureFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.PipingAtStructure);
            if (pipingStructureFailureMechanismEntity != null)
            {
                pipingStructureFailureMechanismEntity.ReadAsPipingStructureFailureMechanism(assessmentSection.PipingStructure, collector);
            }
        }

        private static void ReadDuneErosionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var duneErosionFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.DuneErosion);
            if (duneErosionFailureMechanismEntity != null)
            {
                duneErosionFailureMechanismEntity.ReadAsDuneErosionFailureMechanism(assessmentSection.DuneErosion, collector);
            }
        }

        private static void ReadStabilityStoneCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var stabilityStoneCoverFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.StabilityStoneRevetment);
            if (stabilityStoneCoverFailureMechanismEntity != null)
            {
                stabilityStoneCoverFailureMechanismEntity.ReadAsStabilityStoneCoverFailureMechanism(assessmentSection.StabilityStoneCover, collector);
            }
        }

        private static void ReadStabilityPointStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var stabilityPointStructuresFailureMechanismEntity = entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) FailureMechanismType.StabilityPointStructures);
            if (stabilityPointStructuresFailureMechanismEntity != null)
            {
                stabilityPointStructuresFailureMechanismEntity.ReadAsStabilityPointStructuresFailureMechanism(assessmentSection.StabilityPointStructures, collector);
            }
        }
    }
}