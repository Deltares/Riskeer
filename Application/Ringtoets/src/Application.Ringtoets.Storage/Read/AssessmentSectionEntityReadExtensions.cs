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
using System.Linq;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
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
                throw new ArgumentNullException(nameof(collector));
            }

            var assessmentSection = new AssessmentSection((AssessmentSectionComposition) entity.Composition)
            {
                Id = entity.Id,
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                },
                FailureMechanismContribution =
                {
                    Norm = entity.Norm
                }
            };

            entity.ReadBackgroundMapData(assessmentSection);

            entity.ReadHydraulicDatabase(assessmentSection, collector);
            entity.ReadReferenceLine(assessmentSection);

            FixMemoryLoadProblemForReadingFailureMechanisms();

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

        /// <summary>
        /// This method should be called after reading the <see cref="ReferenceLine"/> but
        /// before reading any of the <see cref="IFailureMechanism"/> instances.
        /// </summary>
        private static void FixMemoryLoadProblemForReadingFailureMechanisms()
        {
            // See WTI-1049
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        private static void ReadBackgroundMapData(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            BackgroundMapDataContainer container = entity.BackgroundMapDataEntities.Single().Read();

            assessmentSection.BackgroundMapData.IsVisible = container.IsVisible;
            assessmentSection.BackgroundMapData.Transparency = container.Transparency;
            assessmentSection.BackgroundMapData.MapData = container.MapData;
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
            var pipingFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.Piping);
            pipingFailureMechanismEntity?.ReadAsPipingFailureMechanism(assessmentSection.PipingFailureMechanism, collector);
        }

        private static void ReadGrassCoverErosionInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverErosionInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentTopErosionAndInwards);
            grassCoverErosionInwardsFailureMechanismEntity?.ReadAsGrassCoverErosionInwardsFailureMechanism(assessmentSection.GrassCoverErosionInwards, collector);
        }

        private static void ReadHeightStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var heightStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StructureHeight);
            heightStructuresFailureMechanismEntity?.ReadAsHeightStructuresFailureMechanism(assessmentSection.HeightStructures, collector);
        }

        private static void ReadStrengthStabilityLengthwiseConstructionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var strengthStabilityLengthwiseConstructionFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StrengthAndStabilityParallelConstruction);
            strengthStabilityLengthwiseConstructionFailureMechanismEntity?.ReadAsStrengthStabilityLengthwiseConstructionFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction, collector);
        }

        private static void ReadTechnicalInnovationFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var technicalInnovationFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.TechnicalInnovations);
            technicalInnovationFailureMechanismEntity?.ReadAsTechnicalInnovationFailureMechanism(assessmentSection.TechnicalInnovation, collector);
        }

        private static void ReadWaterPressureAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var waterPressureAsphaltCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.WaterOverpressureAsphaltRevetment);
            waterPressureAsphaltCoverFailureMechanismEntity?.ReadAsWaterPressureAsphaltCoverFailureMechanism(assessmentSection.WaterPressureAsphaltCover, collector);
        }

        private static void ReadClosingStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var closingStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.ReliabilityClosingOfStructure);
            closingStructuresFailureMechanismEntity?.ReadAsClosingStructuresFailureMechanism(assessmentSection.ClosingStructures, collector);
        }

        private static void ReadMacrostabilityInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var macrostabilityInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.MacrostabilityInwards);
            macrostabilityInwardsFailureMechanismEntity?.ReadAsMacrostabilityInwardsFailureMechanism(assessmentSection.MacrostabilityInwards, collector);
        }

        private static void ReadMacrostabilityOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var macrostabilityOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.MacrostabilityOutwards);
            macrostabilityOutwardsFailureMechanismEntity?.ReadAsMacrostabilityOutwardsFailureMechanism(assessmentSection.MacrostabilityOutwards, collector);
        }

        private static void ReadWaveImpactAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var waveImpactAsphaltCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.WaveImpactOnAsphaltRevetment);
            waveImpactAsphaltCoverFailureMechanismEntity?.ReadAsWaveImpactAsphaltCoverFailureMechanism(assessmentSection.WaveImpactAsphaltCover, collector);
        }

        private static void ReadGrassCoverErosionOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverErosionOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentErosionOutwards);
            grassCoverErosionOutwardsFailureMechanismEntity?.ReadAsGrassCoverErosionOutwardsFailureMechanism(assessmentSection.GrassCoverErosionOutwards, collector);
        }

        private static void ReadGrassCoverSlipOffInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverSlipOffInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentSlidingInwards);
            grassCoverSlipOffInwardsFailureMechanismEntity?.ReadAsGrassCoverSlipOffInwardsFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, collector);
        }

        private static void ReadGrassCoverSlipOffOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var grassCoverSlipOffOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentSlidingOutwards);
            grassCoverSlipOffOutwardsFailureMechanismEntity?.ReadAsGrassCoverSlipOffOutwardsFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, collector);
        }

        private static void ReadMicrostabilityFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var microstabilityFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.Microstability);
            microstabilityFailureMechanismEntity?.ReadAsMicrostabilityFailureMechanism(assessmentSection.Microstability, collector);
        }

        private static void ReadPipingStructureFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var pipingStructureFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.PipingAtStructure);
            pipingStructureFailureMechanismEntity?.ReadAsPipingStructureFailureMechanism(assessmentSection.PipingStructure, collector);
        }

        private static void ReadDuneErosionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var duneErosionFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.DuneErosion);
            duneErosionFailureMechanismEntity?.ReadAsDuneErosionFailureMechanism(assessmentSection.DuneErosion, collector);
        }

        private static void ReadStabilityStoneCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var stabilityStoneCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StabilityStoneRevetment);
            stabilityStoneCoverFailureMechanismEntity?.ReadAsStabilityStoneCoverFailureMechanism(assessmentSection.StabilityStoneCover, collector);
        }

        private static void ReadStabilityPointStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            var stabilityPointStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StabilityPointStructures);
            stabilityPointStructuresFailureMechanismEntity?.ReadAsStabilityPointStructuresFailureMechanism(assessmentSection.StabilityPointStructures, collector);
        }

        private static FailureMechanismEntity GetFailureMechanismEntityOfType(AssessmentSectionEntity entity, FailureMechanismType type)
        {
            return entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) type);
        }
    }
}