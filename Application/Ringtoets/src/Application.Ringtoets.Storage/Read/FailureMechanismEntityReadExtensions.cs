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
using Application.Ringtoets.Storage.Read.ClosingStructures;
using Application.Ringtoets.Storage.Read.GrassCoverErosionInwards;
using Application.Ringtoets.Storage.Read.GrassCoverErosionOutwards;
using Application.Ringtoets.Storage.Read.HeightStructures;
using Application.Ringtoets.Storage.Read.Piping;
using Application.Ringtoets.Storage.Read.StabilityPointStructures;
using Application.Ringtoets.Storage.Read.WaveImpactAsphaltCover;
using Core.Common.Base;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingFailureMechanism"/> based on the
    /// <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="IFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="IFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadCommonFailureMechanismProperties(this FailureMechanismEntity entity, IFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            failureMechanism.IsRelevant = Convert.ToBoolean(entity.IsRelevant);
            failureMechanism.Comments = entity.Comments;

            entity.ReadFailureMechanismSections(failureMechanism, collector);
        }

        private static void ReadFailureMechanismSections(this FailureMechanismEntity entity, IFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var failureMechanismSectionEntity in entity.FailureMechanismSectionEntities)
            {
                failureMechanism.AddSection(failureMechanismSectionEntity.Read(collector));
            }
        }

        #region Piping

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

            foreach (var stochasticSoilModelEntity in entity.StochasticSoilModelEntities.OrderBy(ssm => ssm.Order))
            {
                failureMechanism.StochasticSoilModels.Add(stochasticSoilModelEntity.Read(collector));
            }

            foreach (SurfaceLineEntity surfaceLineEntity in entity.SurfaceLineEntities.OrderBy(sl => sl.Order))
            {
                failureMechanism.SurfaceLines.Add(surfaceLineEntity.Read(collector));
            }

            entity.ReadPipingMechanismSectionResults(failureMechanism, collector);

            ReadPipingRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup,
                                           failureMechanism.GeneralInput, collector);
        }

        private static void ReadProbabilityAssessmentInput(ICollection<PipingFailureMechanismMetaEntity> pipingFailureMechanismMetaEntities, PipingProbabilityAssessmentInput pipingProbabilityAssessmentInput)
        {
            PipingProbabilityAssessmentInput probabilityAssessmentInput = pipingFailureMechanismMetaEntities.ElementAt(0).Read();

            pipingProbabilityAssessmentInput.A = probabilityAssessmentInput.A;
            pipingProbabilityAssessmentInput.UpliftCriticalSafetyFactor = probabilityAssessmentInput.UpliftCriticalSafetyFactor;
        }

        private static void ReadPipingMechanismSectionResults(this FailureMechanismEntity entity, PipingFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadPipingRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                           CalculationGroup targetRootCalculationGroup,
                                                           GeneralPipingInput generalPipingInput,
                                                           ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsPipingCalculationGroup(collector, generalPipingInput);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Grass Cover Erosion Inwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverErosionInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGeneralGrassCoverErosionInwardsCalculationInput(failureMechanism.GeneralInput);
            entity.ReadDikeProfiles(failureMechanism.DikeProfiles, collector);
            ReadGrassCoverErosionInwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
            entity.ReadGrassCoverErosionInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGeneralGrassCoverErosionInwardsCalculationInput(this FailureMechanismEntity entity, GeneralGrassCoverErosionInwardsInput input)
        {
            entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Single().Read(input);
        }

        private static void ReadDikeProfiles(this FailureMechanismEntity entity, ICollection<DikeProfile> dikeProfiles, ReadConversionCollector collector)
        {
            foreach (DikeProfileEntity dikeProfileEntity in entity.DikeProfileEntities)
            {
                dikeProfiles.Add(dikeProfileEntity.Read(collector));
            }
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

        private static void ReadGrassCoverErosionInwardsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                             CalculationGroup targetRootCalculationGroup,
                                                                             ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsGrassCoverErosionInwardsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Height Structures

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="HeightStructuresFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsHeightStructuresFailureMechanism(this FailureMechanismEntity entity, HeightStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadHeightStructuresMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);
            entity.ReadHeightStructures(failureMechanism.HeightStructures, collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
        }

        private static void ReadHeightStructuresMechanismSectionResults(this FailureMechanismEntity entity, HeightStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.HeightStructuresSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadHeightStructures(this FailureMechanismEntity entity, ObservableList<HeightStructure> heightStructures, ReadConversionCollector collector)
        {
            heightStructures.AddRange(entity.HeightStructureEntities.OrderBy(fpe => fpe.Order).Select(structureEntity => structureEntity.Read(collector)));
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralHeightStructuresInput generalInput)
        {
            GeneralHeightStructuresInput generalHeightStructuresInput = entity.HeightStructuresFailureMechanismMetaEntities.First().Read();
            generalInput.N = generalHeightStructuresInput.N;
        }

        #endregion

        #region Strength Stability Lengthwise Construction

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Technical Innovation

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="TechnicalInnovationFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="TechnicalInnovationFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Water Pressure Asphalt

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaterPressureAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaterPressureAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Closing Structures

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="ClosingStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="ClosingStructuresFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsClosingStructuresFailureMechanism(this FailureMechanismEntity entity, ClosingStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadClosingStructuresMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);
            entity.ReadHeightStructures(failureMechanism.ClosingStructures, collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
        }

        private static void ReadClosingStructuresMechanismSectionResults(this FailureMechanismEntity entity, ClosingStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.ClosingStructuresSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralClosingStructuresInput generalInput)
        {
            GeneralClosingStructuresInput generalClosingStructuresInput = entity.ClosingStructureFailureMechanismMetaEntities.First().Read();
            generalInput.N2A = generalClosingStructuresInput.N2A;
        }

        private static void ReadHeightStructures(this FailureMechanismEntity entity, ObservableList<ClosingStructure> closingStructures, ReadConversionCollector collector)
        {
            closingStructures.AddRange(entity.ClosingStructureEntities.OrderBy(fpe => fpe.Order).Select(structureEntity => structureEntity.Read(collector)));
        }

        #endregion

        #region Macrostability Inwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacrostabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacrostabilityInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Macrostability Outwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacrostabilityOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacrostabilityOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Wave Impact Asphalt Cover

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsWaveImpactAsphaltCoverFailureMechanism(this FailureMechanismEntity entity, WaveImpactAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaveImpactAsphaltCoverMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);

            ReadWaveImpactAsphaltCoverRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.WaveConditionsCalculationGroup, collector);
        }

        private static void ReadWaveImpactAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity, WaveImpactAsphaltCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaveImpactAsphaltCoverSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadWaveImpactAsphaltCoverRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                           CalculationGroup targetRootCalculationGroup,
                                                                           ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsWaveImpactAsphaltCoverWaveConditionsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Grass Cover Erosion Outwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverErosionOutwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverErosionOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGeneralGrassCoverErosionOutwardsCalculationInput(failureMechanism.GeneralInput);
            entity.ReadGrassCoverErosionOutwardsMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);
            entity.ReadHydraulicBoundaryLocations(failureMechanism.HydraulicBoundaryLocations, collector);

            ReadGrassCoverErosionOutwardsWaveConditionsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.WaveConditionsCalculationGroup, collector);
        }

        private static void ReadGeneralGrassCoverErosionOutwardsCalculationInput(this FailureMechanismEntity entity, GeneralGrassCoverErosionOutwardsInput input)
        {
            entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single().Read(input);
        }

        private static void ReadGrassCoverErosionOutwardsMechanismSectionResults(this FailureMechanismEntity entity, GrassCoverErosionOutwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadHydraulicBoundaryLocations(this FailureMechanismEntity entity, List<HydraulicBoundaryLocation> locations, ReadConversionCollector collector)
        {
            locations.AddRange(
                entity
                    .GrassCoverErosionOutwardsHydraulicLocationEntities
                    .OrderBy(location => location.Order)
                    .Select(location => location.Read(collector)));
        }

        private static void ReadGrassCoverErosionOutwardsWaveConditionsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                                            CalculationGroup targetRootCalculationGroup, ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsGrassCoverErosionOutwardsWaveConditionsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Grass Cover Slip Off Inwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverSlipOffInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverSlipOffInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Grass Cover Slip Off Outwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Microstability

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MicrostabilityFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MicrostabilityFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Piping Structure

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="PipingStructureFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="PipingStructureFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Dune Erosion

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="DuneErosionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
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

                sectionResultEntity.Read(result);
            }
        }

        #endregion

        #region Stability Stone Cover

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StabilityStoneCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStabilityStoneCoverFailureMechanism(this FailureMechanismEntity entity, StabilityStoneCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityStoneCoverMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);

            ReadStabilityStoneCoverWaveConditionsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.WaveConditionsCalculationGroup, collector);
        }

        private static void ReadForeshoreProfiles(this FailureMechanismEntity entity, ObservableList<ForeshoreProfile> foreshoreProfiles, ReadConversionCollector collector)
        {
            foreshoreProfiles.AddRange(entity.ForeshoreProfileEntities.OrderBy(fpe => fpe.Order).Select(foreshoreProfileEntity => foreshoreProfileEntity.Read(collector)));
        }

        private static void ReadStabilityStoneCoverMechanismSectionResults(this FailureMechanismEntity entity, StabilityStoneCoverFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityStoneCoverSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadStabilityStoneCoverWaveConditionsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                                      CalculationGroup targetRootCalculationGroup, ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Stability Point Structures

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StabilityPointStructuresFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStabilityPointStructuresFailureMechanism(this FailureMechanismEntity entity, StabilityPointStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityPointStructuresMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
        }

        private static void ReadStabilityPointStructuresMechanismSectionResults(this FailureMechanismEntity entity, StabilityPointStructuresFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityPointStructuresSectionResultEntities))
            {
                var failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralStabilityPointStructuresInput generalInput)
        {
            GeneralStabilityPointStructuresInput generalStabilityPointStructuresInput = entity.StabilityPointStructuresFailureMechanismMetaEntities.First().Read();
            generalInput.N = generalStabilityPointStructuresInput.N;
        }

        #endregion
    }
}