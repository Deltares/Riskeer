// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Read
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

            var assessmentSection = new AssessmentSection((AssessmentSectionComposition) entity.Composition,
                                                          entity.LowerLimitNorm,
                                                          entity.SignalingNorm)
            {
                Id = entity.Id,
                Name = entity.Name,
                Comments =
                {
                    Body = entity.Comments
                },
                FailureMechanismContribution =
                {
                    NormativeNorm = (NormType) entity.NormativeNormType
                }
            };

            entity.ReadBackgroundData(assessmentSection);

            entity.ReadHydraulicDatabase(assessmentSection, collector);
            entity.ReadReferenceLine(assessmentSection);

            entity.ReadPipingFailureMechanism(assessmentSection, collector);
            entity.ReadGrassCoverErosionInwardsFailureMechanism(assessmentSection, collector);
            entity.ReadHeightStructuresFailureMechanism(assessmentSection, collector);
            entity.ReadStrengthStabilityLengthwiseConstructionFailureMechanism(assessmentSection, collector);
            entity.ReadTechnicalInnovationFailureMechanism(assessmentSection, collector);
            entity.ReadWaterPressureAsphaltCoverFailureMechanism(assessmentSection, collector);
            entity.ReadClosingStructuresFailureMechanism(assessmentSection, collector);
            entity.ReadMacroStabilityInwardsFailureMechanism(assessmentSection, collector);
            entity.ReadMacroStabilityOutwardsFailureMechanism(assessmentSection, collector);
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

        private static void ReadBackgroundData(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            BackgroundData readBackgroundData = entity.BackgroundDataEntities.Single().Read();

            BackgroundData backgroundData = assessmentSection.BackgroundData;
            backgroundData.IsVisible = readBackgroundData.IsVisible;
            backgroundData.Transparency = readBackgroundData.Transparency;
            backgroundData.Name = readBackgroundData.Name;
            backgroundData.Configuration = readBackgroundData.Configuration;
        }

        private static void ReadReferenceLine(this AssessmentSectionEntity entity, IAssessmentSection assessmentSection)
        {
            if (entity.ReferenceLinePointXml != null)
            {
                Point2D[] points = new Point2DCollectionXmlSerializer().FromXml(entity.ReferenceLinePointXml);

                var referenceLine = new ReferenceLine();
                referenceLine.SetGeometry(points);

                assessmentSection.ReferenceLine = referenceLine;
            }
        }

        private static void ReadHydraulicDatabase(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            if (entity.HydraulicDatabaseLocation != null)
            {
                HydraulicBoundaryDatabase hydraulicBoundaryDatabase = assessmentSection.HydraulicBoundaryDatabase;
                hydraulicBoundaryDatabase.FilePath = entity.HydraulicDatabaseLocation;
                hydraulicBoundaryDatabase.Version = entity.HydraulicDatabaseVersion;

                HydraulicBoundaryLocation[] readHydraulicBoundaryLocations = entity.HydraulicLocationEntities
                                                                                   .OrderBy(hl => hl.Order)
                                                                                   .Select(hle => hle.Read(collector))
                                                                                   .ToArray();
                hydraulicBoundaryDatabase.Locations.AddRange(readHydraulicBoundaryLocations);
                assessmentSection.SetHydraulicBoundaryLocationCalculations(readHydraulicBoundaryLocations);
                assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(readHydraulicBoundaryLocations);

                entity.ReadHydraulicBoundaryLocationCalculations(assessmentSection, collector);
            }
        }

        private static void ReadHydraulicBoundaryLocationCalculations(this AssessmentSectionEntity entity,
                                                                      IAssessmentSection assessmentSection,
                                                                      ReadConversionCollector collector)
        {
            entity.HydraulicLocationCalculationCollectionEntity7.Read(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity6.Read(assessmentSection.WaterLevelCalculationsForSignalingNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity5.Read(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity4.Read(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, collector);

            entity.HydraulicLocationCalculationCollectionEntity3.Read(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity2.Read(assessmentSection.WaveHeightCalculationsForSignalingNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity1.Read(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, collector);
            entity.HydraulicLocationCalculationCollectionEntity.Read(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, collector);
        }

        private static void ReadPipingFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity pipingFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.Piping);
            pipingFailureMechanismEntity?.ReadAsPipingFailureMechanism(assessmentSection.Piping, collector);
        }

        private static void ReadGrassCoverErosionInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity grassCoverErosionInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentTopErosionAndInwards);
            grassCoverErosionInwardsFailureMechanismEntity?.ReadAsGrassCoverErosionInwardsFailureMechanism(assessmentSection.GrassCoverErosionInwards, collector);
        }

        private static void ReadHeightStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity heightStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StructureHeight);
            heightStructuresFailureMechanismEntity?.ReadAsHeightStructuresFailureMechanism(assessmentSection.HeightStructures, collector);
        }

        private static void ReadStrengthStabilityLengthwiseConstructionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity strengthStabilityLengthwiseConstructionFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StrengthAndStabilityParallelConstruction);
            strengthStabilityLengthwiseConstructionFailureMechanismEntity?.ReadAsStrengthStabilityLengthwiseConstructionFailureMechanism(assessmentSection.StrengthStabilityLengthwiseConstruction, collector);
        }

        private static void ReadTechnicalInnovationFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity technicalInnovationFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.TechnicalInnovations);
            technicalInnovationFailureMechanismEntity?.ReadAsTechnicalInnovationFailureMechanism(assessmentSection.TechnicalInnovation, collector);
        }

        private static void ReadWaterPressureAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity waterPressureAsphaltCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.WaterOverpressureAsphaltRevetment);
            waterPressureAsphaltCoverFailureMechanismEntity?.ReadAsWaterPressureAsphaltCoverFailureMechanism(assessmentSection.WaterPressureAsphaltCover, collector);
        }

        private static void ReadClosingStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity closingStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.ReliabilityClosingOfStructure);
            closingStructuresFailureMechanismEntity?.ReadAsClosingStructuresFailureMechanism(assessmentSection.ClosingStructures, collector);
        }

        private static void ReadMacroStabilityInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity macroStabilityInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.MacroStabilityInwards);
            macroStabilityInwardsFailureMechanismEntity?.ReadAsMacroStabilityInwardsFailureMechanism(assessmentSection.MacroStabilityInwards, collector);
        }

        private static void ReadMacroStabilityOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity macroStabilityOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.MacroStabilityOutwards);
            macroStabilityOutwardsFailureMechanismEntity?.ReadAsMacroStabilityOutwardsFailureMechanism(assessmentSection.MacroStabilityOutwards, collector);
        }

        private static void ReadWaveImpactAsphaltCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity waveImpactAsphaltCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.WaveImpactOnAsphaltRevetment);
            waveImpactAsphaltCoverFailureMechanismEntity?.ReadAsWaveImpactAsphaltCoverFailureMechanism(assessmentSection.WaveImpactAsphaltCover, collector);
        }

        private static void ReadGrassCoverErosionOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity grassCoverErosionOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentErosionOutwards);
            grassCoverErosionOutwardsFailureMechanismEntity?.ReadAsGrassCoverErosionOutwardsFailureMechanism(assessmentSection.GrassCoverErosionOutwards, collector);
        }

        private static void ReadGrassCoverSlipOffInwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity grassCoverSlipOffInwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentSlidingInwards);
            grassCoverSlipOffInwardsFailureMechanismEntity?.ReadAsGrassCoverSlipOffInwardsFailureMechanism(assessmentSection.GrassCoverSlipOffInwards, collector);
        }

        private static void ReadGrassCoverSlipOffOutwardsFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity grassCoverSlipOffOutwardsFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.GrassRevetmentSlidingOutwards);
            grassCoverSlipOffOutwardsFailureMechanismEntity?.ReadAsGrassCoverSlipOffOutwardsFailureMechanism(assessmentSection.GrassCoverSlipOffOutwards, collector);
        }

        private static void ReadMicrostabilityFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity microstabilityFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.Microstability);
            microstabilityFailureMechanismEntity?.ReadAsMicrostabilityFailureMechanism(assessmentSection.Microstability, collector);
        }

        private static void ReadPipingStructureFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity pipingStructureFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.PipingAtStructure);
            pipingStructureFailureMechanismEntity?.ReadAsPipingStructureFailureMechanism(assessmentSection.PipingStructure, collector);
        }

        private static void ReadDuneErosionFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity duneErosionFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.DuneErosion);
            duneErosionFailureMechanismEntity?.ReadAsDuneErosionFailureMechanism(assessmentSection.DuneErosion, collector);
        }

        private static void ReadStabilityStoneCoverFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity stabilityStoneCoverFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StabilityStoneRevetment);
            stabilityStoneCoverFailureMechanismEntity?.ReadAsStabilityStoneCoverFailureMechanism(assessmentSection.StabilityStoneCover, collector);
        }

        private static void ReadStabilityPointStructuresFailureMechanism(this AssessmentSectionEntity entity, AssessmentSection assessmentSection, ReadConversionCollector collector)
        {
            FailureMechanismEntity stabilityPointStructuresFailureMechanismEntity = GetFailureMechanismEntityOfType(entity, FailureMechanismType.StabilityPointStructures);
            stabilityPointStructuresFailureMechanismEntity?.ReadAsStabilityPointStructuresFailureMechanism(assessmentSection.StabilityPointStructures, collector);
        }

        private static FailureMechanismEntity GetFailureMechanismEntityOfType(AssessmentSectionEntity entity, FailureMechanismType type)
        {
            return entity.FailureMechanismEntities.SingleOrDefault(fme => fme.FailureMechanismType == (int) type);
        }
    }
}