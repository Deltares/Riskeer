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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanism"/> based on the
    /// <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="PipingFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="failureMechanism"/> is <c>null</c></item>
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// </list></exception>
        internal static void ReadAsPipingFailureMechanism(this FailureMechanismEntity entity, PipingFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism");
            }
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            if (entity.PipingFailureMechanismMetaEntities.Count > 0)
            {
                ReadProbabilityAssessmentInput(entity.PipingFailureMechanismMetaEntities, failureMechanism.PipingProbabilityAssessmentInput);
            }

            foreach (var stochasticSoilModelEntity in entity.StochasticSoilModelEntities)
            {
                failureMechanism.StochasticSoilModels.Add(stochasticSoilModelEntity.Read(collector));
            }

            foreach (SurfaceLineEntity surfaceLineEntity in entity.SurfaceLineEntities)
            {
                failureMechanism.SurfaceLines.Add(surfaceLineEntity.Read(collector));
            }

            entity.ReadPipingMechanismSectionResults(failureMechanism, collector);

            ReadRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup,
                                     failureMechanism.GeneralInput, collector);
        }

        private static void ReadPipingMechanismSectionResults(this FailureMechanismEntity entity, PipingFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverErosionInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGeneralCalculationInput(failureMechanism.GeneralInput);
            entity.ReadGrassCoverErosionInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGeneralCalculationInput(this FailureMechanismEntity entity, GeneralGrassCoverErosionInwardsInput input)
        {
            GrassCoverErosionInwardsFailureMechanismMetaEntity t = entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Single();
            t.Read(input);
        }

        private static void ReadGrassCoverErosionInwardsMechanismSectionResults(this FailureMechanismEntity entity, GrassCoverErosionInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));
                
                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="HeightStructuresFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsHeightStructuresFailureMechanism(this FailureMechanismEntity entity, HeightStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadHeightStructuresMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadHeightStructuresMechanismSectionResults(this FailureMechanismEntity entity, HeightStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.HeightStructuresSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStrengthStabilityLengthwiseConstructionFailureMechanism(this FailureMechanismEntity entity, StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStrengthStabilityLengthwiseConstructionMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadStrengthStabilityLengthwiseConstructionMechanismSectionResults(this FailureMechanismEntity entity, StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StrengthStabilityLengthwiseConstructionSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="TechnicalInnovationFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="TechnicalInnovationFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsTechnicalInnovationFailureMechanism(this FailureMechanismEntity entity, TechnicalInnovationFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadTechnicalInnovationMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadTechnicalInnovationMechanismSectionResults(this FailureMechanismEntity entity, TechnicalInnovationFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.TechnicalInnovationSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaterPressureAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaterPressureAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsWaterPressureAsphaltCoverFailureMechanism(this FailureMechanismEntity entity, WaterPressureAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaterPressureAsphaltCoverMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadWaterPressureAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity, WaterPressureAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaterPressureAsphaltCoverSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="ClosingStructureFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="ClosingStructureFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsClosingStructureFailureMechanism(this FailureMechanismEntity entity, ClosingStructureFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadClosingStructureMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadClosingStructureMechanismSectionResults(this FailureMechanismEntity entity, ClosingStructureFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.ClosingStructureSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacrostabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacrostabilityInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsMacrostabilityInwardsFailureMechanism(this FailureMechanismEntity entity, MacrostabilityInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadMacrostabilityInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadMacrostabilityInwardsMechanismSectionResults(this FailureMechanismEntity entity, MacrostabilityInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacrostabilityInwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacrostabilityOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacrostabilityOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsMacrostabilityOutwardsFailureMechanism(this FailureMechanismEntity entity, MacrostabilityOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadMacrostabilityOutwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadMacrostabilityOutwardsMechanismSectionResults(this FailureMechanismEntity entity, MacrostabilityOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacrostabilityOutwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsWaveImpactAsphaltCoverFailureMechanism(this FailureMechanismEntity entity, WaveImpactAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaveImpactAsphaltCoverMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadWaveImpactAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity, WaveImpactAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaveImpactAsphaltCoverSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverErosionOutwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverErosionOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverErosionOutwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGrassCoverErosionOutwardsMechanismSectionResults(this FailureMechanismEntity entity, GrassCoverErosionOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverSlipOffInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverSlipOffInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverSlipOffInwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverSlipOffInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverSlipOffInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGrassCoverSlipOffInwardsMechanismSectionResults(this FailureMechanismEntity entity, GrassCoverSlipOffInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffInwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverSlipOffOutwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverSlipOffOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverSlipOffOutwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGrassCoverSlipOffOutwardsMechanismSectionResults(this FailureMechanismEntity entity, GrassCoverSlipOffOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffOutwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MicrostabilityFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MicrostabilityFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsMicrostabilityFailureMechanism(this FailureMechanismEntity entity, MicrostabilityFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadMicrostabilityMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadMicrostabilityMechanismSectionResults(this FailureMechanismEntity entity, MicrostabilityFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MicrostabilitySectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="PipingStructureFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="PipingStructureFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsPipingStructureFailureMechanism(this FailureMechanismEntity entity, PipingStructureFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadPipingStructureMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadPipingStructureMechanismSectionResults(this FailureMechanismEntity entity, PipingStructureFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingStructureSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="DuneErosionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsDuneErosionFailureMechanism(this FailureMechanismEntity entity, DuneErosionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadDuneErosionMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadDuneErosionMechanismSectionResults(this FailureMechanismEntity entity, DuneErosionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.DuneErosionSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StabilityStoneCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStabilityStoneCoverFailureMechanism(this FailureMechanismEntity entity, StabilityStoneCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityStoneCoverMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadStabilityStoneCoverMechanismSectionResults(this FailureMechanismEntity entity, StabilityStoneCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityStoneCoverSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StrengthStabilityPointConstructionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StrengthStabilityPointConstructionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStrengthStabilityPointConstructionFailureMechanism(this FailureMechanismEntity entity, StrengthStabilityPointConstructionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStrengthStabilityPointConstructionMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadStrengthStabilityPointConstructionMechanismSectionResults(this FailureMechanismEntity entity, StrengthStabilityPointConstructionFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StrengthStabilityPointConstructionSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="IFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadCommonFailureMechanismProperties(this FailureMechanismEntity entity, IFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            failureMechanism.StorageId = entity.FailureMechanismEntityId;
            failureMechanism.IsRelevant = Convert.ToBoolean(entity.IsRelevant);
            failureMechanism.Comments = entity.Comments;

            entity.ReadFailureMechanismSections(failureMechanism, collector);
        }

        private static void ReadProbabilityAssessmentInput(ICollection<PipingFailureMechanismMetaEntity> pipingFailureMechanismMetaEntities, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput)
        {
            PipingProbabilityAssessmentInput probabilityAssessmentInput = pipingFailureMechanismMetaEntities.ElementAt(0).Read();

            pipingProbabilityAssessmentInput.StorageId = probabilityAssessmentInput.StorageId;
            pipingProbabilityAssessmentInput.A = probabilityAssessmentInput.A;
        }

        private static void ReadRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                     CalculationGroup targetRootCalculationGroup, GeneralPipingInput generalPipingInput,
                                                     ReadConversionCollector collector)
        {
            var rootCalculationGroup = rootCalculationGroupEntity.ReadPipingCalculationGroup(collector, generalPipingInput);
            targetRootCalculationGroup.StorageId = rootCalculationGroup.StorageId;
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        private static void ReadFailureMechanismSections(this FailureMechanismEntity entity, IFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var failureMechanismSectionEntity in entity.FailureMechanismSectionEntities)
            {
                failureMechanism.AddSection(failureMechanismSectionEntity.Read(collector));
            }
        }
    }
}