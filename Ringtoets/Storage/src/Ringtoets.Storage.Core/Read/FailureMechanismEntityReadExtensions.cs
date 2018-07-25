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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Read.ClosingStructures;
using Ringtoets.Storage.Core.Read.DuneErosion;
using Ringtoets.Storage.Core.Read.GrassCoverErosionInwards;
using Ringtoets.Storage.Core.Read.GrassCoverErosionOutwards;
using Ringtoets.Storage.Core.Read.HeightStructures;
using Ringtoets.Storage.Core.Read.MacroStabilityInwards;
using Ringtoets.Storage.Core.Read.MacroStabilityOutwards;
using Ringtoets.Storage.Core.Read.Piping;
using Ringtoets.Storage.Core.Read.PipingStructure;
using Ringtoets.Storage.Core.Read.StabilityPointStructures;
using Ringtoets.Storage.Core.Read.StabilityStoneCover;
using Ringtoets.Storage.Core.Read.WaveImpactAsphaltCover;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Storage.Core.Read
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
        internal static void ReadCommonFailureMechanismProperties(this FailureMechanismEntity entity,
                                                                  IFailureMechanism failureMechanism,
                                                                  ReadConversionCollector collector)
        {
            failureMechanism.IsRelevant = Convert.ToBoolean(entity.IsRelevant);
            failureMechanism.InputComments.Body = entity.InputComments;
            failureMechanism.OutputComments.Body = entity.OutputComments;
            failureMechanism.NotRelevantComments.Body = entity.NotRelevantComments;

            entity.ReadFailureMechanismSections(failureMechanism, collector);
        }

        private static void ReadFailureMechanismSections(this FailureMechanismEntity entity,
                                                         IFailureMechanism failureMechanism,
                                                         ReadConversionCollector collector)
        {
            FailureMechanismSection[] readFailureMechanismSections = entity.FailureMechanismSectionEntities
                                                                               .Select(failureMechanismSectionEntity =>
                                                                                           failureMechanismSectionEntity.Read(collector))
                                                                               .ToArray();
            failureMechanism.SetSections(readFailureMechanismSections, string.Empty);
        }

        private static void ReadForeshoreProfiles(this FailureMechanismEntity entity,
                                                  ForeshoreProfileCollection foreshoreProfiles,
                                                  string foreshoreProfileSourcePath,
                                                  ReadConversionCollector collector)
        {
            if (foreshoreProfileSourcePath != null)
            {
                foreshoreProfiles.AddRange(entity.ForeshoreProfileEntities
                                                 .OrderBy(fpe => fpe.Order)
                                                 .Select(foreshoreProfileEntity => foreshoreProfileEntity.Read(collector))
                                                 .ToArray(),
                                           foreshoreProfileSourcePath);
            }
        }

        #region Piping

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="PipingFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries cannot be found.</exception>
        internal static void ReadAsPipingFailureMechanism(this FailureMechanismEntity entity,
                                                          PipingFailureMechanism failureMechanism,
                                                          ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            PipingFailureMechanismMetaEntity metaEntity = entity.PipingFailureMechanismMetaEntities.Single();
            metaEntity.ReadProbabilityAssessmentInput(failureMechanism.PipingProbabilityAssessmentInput);
            metaEntity.ReadGeneralPipingInput(failureMechanism.GeneralInput);

            string stochasticSoilModelCollectionSourcePath = metaEntity.StochasticSoilModelCollectionSourcePath;
            if (stochasticSoilModelCollectionSourcePath != null)
            {
                failureMechanism.StochasticSoilModels.AddRange(entity.StochasticSoilModelEntities
                                                                     .OrderBy(ssm => ssm.Order)
                                                                     .Select(e => e.ReadAsPipingStochasticSoilModel(collector))
                                                                     .ToArray(),
                                                               stochasticSoilModelCollectionSourcePath);
            }

            string surfaceLineCollectionSourcePath = metaEntity.SurfaceLineCollectionSourcePath;
            if (surfaceLineCollectionSourcePath != null)
            {
                failureMechanism.SurfaceLines.AddRange(entity.SurfaceLineEntities
                                                             .OrderBy(sl => sl.Order)
                                                             .Select(e => e.ReadAsPipingSurfaceLine(collector))
                                                             .ToArray(),
                                                       surfaceLineCollectionSourcePath);
            }

            entity.ReadPipingMechanismSectionResults(failureMechanism, collector);

            ReadPipingRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup,
                                           failureMechanism.GeneralInput, collector);
        }

        private static void ReadPipingMechanismSectionResults(this FailureMechanismEntity entity,
                                                              PipingFailureMechanism failureMechanism,
                                                              ReadConversionCollector collector)
        {
            foreach (PipingSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                PipingFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                            GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGeneralGrassCoverErosionInwardsCalculationInput(failureMechanism.GeneralInput);
            entity.ReadDikeProfiles(failureMechanism.DikeProfiles, collector);
            ReadGrassCoverErosionInwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
            entity.ReadGrassCoverErosionInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGeneralGrassCoverErosionInwardsCalculationInput(this FailureMechanismEntity entity,
                                                                                GeneralGrassCoverErosionInwardsInput input)
        {
            entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Single().Read(input);
        }

        private static void ReadDikeProfiles(this FailureMechanismEntity entity, DikeProfileCollection dikeProfiles, ReadConversionCollector collector)
        {
            if (entity.DikeProfileEntities.Any())
            {
                GrassCoverErosionInwardsFailureMechanismMetaEntity metaEntity =
                    entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Single();
                string sourcePath = metaEntity.DikeProfileCollectionSourcePath;

                dikeProfiles.AddRange(entity.DikeProfileEntities
                                            .OrderBy(dp => dp.Order)
                                            .Select(dp => dp.Read(collector))
                                            .ToArray(),
                                      sourcePath);
            }
        }

        private static void ReadGrassCoverErosionInwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                                ReadConversionCollector collector)
        {
            foreach (GrassCoverErosionInwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionInwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                GrassCoverErosionInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>>
        internal static void ReadAsHeightStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                    HeightStructuresFailureMechanism failureMechanism,
                                                                    ReadConversionCollector collector)
        {
            HeightStructuresFailureMechanismMetaEntity metaEntity = entity.HeightStructuresFailureMechanismMetaEntities.Single();
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadHeightStructuresMechanismSectionResults(failureMechanism, collector);
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, metaEntity.ForeshoreProfileCollectionSourcePath, collector);
            entity.ReadHeightStructures(failureMechanism.HeightStructures, metaEntity.HeightStructureCollectionSourcePath, collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
            ReadHeightStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadHeightStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                        HeightStructuresFailureMechanism failureMechanism,
                                                                        ReadConversionCollector collector)
        {
            foreach (HeightStructuresSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.HeightStructuresSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                HeightStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        private static void ReadHeightStructures(this FailureMechanismEntity entity,
                                                 StructureCollection<HeightStructure> heightStructures,
                                                 string sourcePath,
                                                 ReadConversionCollector collector)
        {
            if (sourcePath != null)
            {
                heightStructures.AddRange(entity.HeightStructureEntities
                                                .OrderBy(fpe => fpe.Order)
                                                .Select(structureEntity => structureEntity.Read(collector))
                                                .ToArray(),
                                          sourcePath);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralHeightStructuresInput generalInput)
        {
            GeneralHeightStructuresInput generalHeightStructuresInput = entity.HeightStructuresFailureMechanismMetaEntities.First().Read();
            generalInput.N = generalHeightStructuresInput.N;
        }

        private static void ReadHeightStructuresRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                     CalculationGroup targetRootCalculationGroup,
                                                                     ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsHeightStructuresCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region Strength Stability Lengthwise Construction

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StrengthStabilityLengthwiseConstructionFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsStrengthStabilityLengthwiseConstructionFailureMechanism(this FailureMechanismEntity entity,
                                                                                           StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism,
                                                                                           ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStrengthStabilityLengthwiseConstructionMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadStrengthStabilityLengthwiseConstructionMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                               StrengthStabilityLengthwiseConstructionFailureMechanism failureMechanism,
                                                                                               ReadConversionCollector collector)
        {
            foreach (StrengthStabilityLengthwiseConstructionSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StrengthStabilityLengthwiseConstructionSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsTechnicalInnovationFailureMechanism(this FailureMechanismEntity entity,
                                                                       TechnicalInnovationFailureMechanism failureMechanism,
                                                                       ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadTechnicalInnovationMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadTechnicalInnovationMechanismSectionResults(this FailureMechanismEntity entity,
                                                                           TechnicalInnovationFailureMechanism failureMechanism,
                                                                           ReadConversionCollector collector)
        {
            foreach (TechnicalInnovationSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.TechnicalInnovationSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                TechnicalInnovationFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsWaterPressureAsphaltCoverFailureMechanism(this FailureMechanismEntity entity,
                                                                             WaterPressureAsphaltCoverFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaterPressureAsphaltCoverMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadWaterPressureAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                 WaterPressureAsphaltCoverFailureMechanism failureMechanism,
                                                                                 ReadConversionCollector collector)
        {
            foreach (WaterPressureAsphaltCoverSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaterPressureAsphaltCoverSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                WaterPressureAsphaltCoverFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="InvalidOperationException">Thrown when expected table entries cannot be found.</exception>
        internal static void ReadAsClosingStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                     ClosingStructuresFailureMechanism failureMechanism,
                                                                     ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadClosingStructuresMechanismSectionResults(failureMechanism, collector);

            ClosingStructuresFailureMechanismMetaEntity metaEntity = entity.ClosingStructuresFailureMechanismMetaEntities.Single();

            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles,
                                         metaEntity.ForeshoreProfileCollectionSourcePath,
                                         collector);
            entity.ReadClosingStructures(failureMechanism.ClosingStructures,
                                         metaEntity.ClosingStructureCollectionSourcePath, collector);

            entity.ReadGeneralInput(failureMechanism.GeneralInput);
            ReadClosingStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadClosingStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                         ClosingStructuresFailureMechanism failureMechanism,
                                                                         ReadConversionCollector collector)
        {
            foreach (ClosingStructuresSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.ClosingStructuresSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                ClosingStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralClosingStructuresInput generalInput)
        {
            GeneralClosingStructuresInput generalClosingStructuresInput = entity.ClosingStructuresFailureMechanismMetaEntities.First().Read();
            generalInput.N2A = generalClosingStructuresInput.N2A;
        }

        private static void ReadClosingStructures(this FailureMechanismEntity entity,
                                                  StructureCollection<ClosingStructure> closingStructures,
                                                  string sourcePath,
                                                  ReadConversionCollector collector)
        {
            if (sourcePath != null)
            {
                closingStructures.AddRange(entity.ClosingStructureEntities
                                                 .OrderBy(fpe => fpe.Order)
                                                 .Select(structureEntity => structureEntity.Read(collector))
                                                 .ToArray(),
                                           sourcePath);
            }
        }

        private static void ReadClosingStructuresRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                      CalculationGroup targetRootCalculationGroup,
                                                                      ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsClosingStructuresCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region MacroStability Inwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacroStabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacroStabilityInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsMacroStabilityInwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                         MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                         ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);

            MacroStabilityInwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.Single();
            metaEntity.ReadProbabilityAssessmentInput(failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput);

            string stochasticSoilModelCollectionSourcePath = metaEntity.StochasticSoilModelCollectionSourcePath;
            if (stochasticSoilModelCollectionSourcePath != null)
            {
                failureMechanism.StochasticSoilModels.AddRange(entity.StochasticSoilModelEntities
                                                                     .OrderBy(ssm => ssm.Order)
                                                                     .Select(e => e.ReadAsMacroStabilityInwardsStochasticSoilModel(collector))
                                                                     .ToArray(),
                                                               stochasticSoilModelCollectionSourcePath);
            }

            string surfaceLineCollectionSourcePath = metaEntity.SurfaceLineCollectionSourcePath;
            if (surfaceLineCollectionSourcePath != null)
            {
                failureMechanism.SurfaceLines.AddRange(entity.SurfaceLineEntities
                                                             .OrderBy(sl => sl.Order)
                                                             .Select(e => e.ReadAsMacroStabilityInwardsSurfaceLine(collector))
                                                             .ToArray(),
                                                       surfaceLineCollectionSourcePath);
            }

            entity.ReadMacroStabilityInwardsMechanismSectionResults(failureMechanism, collector);
            ReadMacroStabilityInwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadMacroStabilityInwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                             MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            foreach (MacroStabilityInwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityInwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                MacroStabilityInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadMacroStabilityInwardsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                          CalculationGroup targetRootCalculationGroup,
                                                                          ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsMacroStabilityInwardsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion

        #region MacroStability Outwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="MacroStabilityOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="MacroStabilityOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadAsMacroStabilityOutwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                          MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                                          ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadMacroStabilityOutwardsMechanismSectionResults(failureMechanism, collector);
            MacroStabilityOutwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityOutwardsFailureMechanismMetaEntities.Single();
            metaEntity.ReadProbabilityAssessmentInput(failureMechanism.MacroStabilityOutwardsProbabilityAssessmentInput);
        }

        private static void ReadMacroStabilityOutwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                              MacroStabilityOutwardsFailureMechanism failureMechanism,
                                                                              ReadConversionCollector collector)
        {
            foreach (MacroStabilityOutwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityOutwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                MacroStabilityOutwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="InvalidOperationException">Thrown when the expected table entries could not be found.</exception>
        internal static void ReadAsWaveImpactAsphaltCoverFailureMechanism(this FailureMechanismEntity entity,
                                                                          WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                          ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaveImpactAsphaltCoverMechanismSectionResults(failureMechanism, collector);

            WaveImpactAsphaltCoverFailureMechanismMetaEntity metaEntity = entity.WaveImpactAsphaltCoverFailureMechanismMetaEntities.Single();
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, metaEntity.ForeshoreProfileCollectionSourcePath, collector);

            ReadWaveImpactAsphaltCoverRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.WaveConditionsCalculationGroup, collector);
            entity.ReadGeneralInput(failureMechanism.GeneralWaveImpactAsphaltCoverInput);
        }

        private static void ReadWaveImpactAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity,
                                                                              WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                              ReadConversionCollector collector)
        {
            foreach (WaveImpactAsphaltCoverSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.WaveImpactAsphaltCoverSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                WaveImpactAsphaltCoverFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralWaveImpactAsphaltCoverInput generalInput)
        {
            GeneralWaveImpactAsphaltCoverInput generalWaveImpactAsphaltCoverInput = entity.WaveImpactAsphaltCoverFailureMechanismMetaEntities.First().Read();
            generalInput.DeltaL = generalWaveImpactAsphaltCoverInput.DeltaL;
        }

        #endregion

        #region Grass Cover Erosion Outwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverErosionOutwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsGrassCoverErosionOutwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                             GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGeneralGrassCoverErosionOutwardsCalculationInput(failureMechanism.GeneralInput);
            entity.ReadGrassCoverErosionOutwardsMechanismSectionResults(failureMechanism, collector);

            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
            ReadForeshoreProfiles(entity,
                                  failureMechanism.ForeshoreProfiles,
                                  metaEntity.ForeshoreProfileCollectionSourcePath,
                                  collector);

            entity.ReadHydraulicBoundaryLocationCalculations(failureMechanism, collector);

            ReadGrassCoverErosionOutwardsWaveConditionsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.WaveConditionsCalculationGroup, collector);
        }

        private static void ReadGeneralGrassCoverErosionOutwardsCalculationInput(this FailureMechanismEntity entity,
                                                                                 GeneralGrassCoverErosionOutwardsInput input)
        {
            GetGrassCoverErosionOutwardsFailureMechanismMetaEntity(entity).Read(input);
        }

        private static void ReadGrassCoverErosionOutwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                 ReadConversionCollector collector)
        {
            foreach (GrassCoverErosionOutwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverErosionOutwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                GrassCoverErosionOutwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadHydraulicBoundaryLocationCalculations(this FailureMechanismEntity entity,
                                                                      GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                      ReadConversionCollector collector)
        {
            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity = GetGrassCoverErosionOutwardsFailureMechanismMetaEntity(entity);

            metaEntity.HydraulicLocationCalculationCollectionEntity5.Read(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                                                          collector);
            metaEntity.HydraulicLocationCalculationCollectionEntity4.Read(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                                                                          collector);
            metaEntity.HydraulicLocationCalculationCollectionEntity3.Read(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                                                                          collector);

            metaEntity.HydraulicLocationCalculationCollectionEntity2.Read(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                                                                          collector);
            metaEntity.HydraulicLocationCalculationCollectionEntity1.Read(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                                                                          collector);
            metaEntity.HydraulicLocationCalculationCollectionEntity.Read(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                                                                         collector);
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

        private static GrassCoverErosionOutwardsFailureMechanismMetaEntity GetGrassCoverErosionOutwardsFailureMechanismMetaEntity(FailureMechanismEntity entity)
        {
            return entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
        }

        #endregion

        #region Grass Cover Slip Off Inwards

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="GrassCoverSlipOffInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverSlipOffInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        internal static void ReadAsGrassCoverSlipOffInwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                            GrassCoverSlipOffInwardsFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverSlipOffInwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGrassCoverSlipOffInwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                GrassCoverSlipOffInwardsFailureMechanism failureMechanism,
                                                                                ReadConversionCollector collector)
        {
            foreach (GrassCoverSlipOffInwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffInwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                GrassCoverSlipOffInwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsGrassCoverSlipOffOutwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                             GrassCoverSlipOffOutwardsFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverSlipOffOutwardsMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadGrassCoverSlipOffOutwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                 GrassCoverSlipOffOutwardsFailureMechanism failureMechanism,
                                                                                 ReadConversionCollector collector)
        {
            foreach (GrassCoverSlipOffOutwardsSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.GrassCoverSlipOffOutwardsSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                GrassCoverSlipOffOutwardsFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsMicrostabilityFailureMechanism(this FailureMechanismEntity entity,
                                                                  MicrostabilityFailureMechanism failureMechanism,
                                                                  ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadMicrostabilityMechanismSectionResults(failureMechanism, collector);
        }

        private static void ReadMicrostabilityMechanismSectionResults(this FailureMechanismEntity entity,
                                                                      MicrostabilityFailureMechanism failureMechanism,
                                                                      ReadConversionCollector collector)
        {
            foreach (MicrostabilitySectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MicrostabilitySectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                MicrostabilityFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadAsPipingStructureFailureMechanism(this FailureMechanismEntity entity,
                                                                   PipingStructureFailureMechanism failureMechanism,
                                                                   ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadPipingStructureMechanismSectionResults(failureMechanism, collector);

            failureMechanism.N = (RoundedDouble) entity.PipingStructureFailureMechanismMetaEntities.Single().N;
        }

        private static void ReadPipingStructureMechanismSectionResults(this FailureMechanismEntity entity,
                                                                       PipingStructureFailureMechanism failureMechanism,
                                                                       ReadConversionCollector collector)
        {
            foreach (PipingStructureSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingStructureSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                PipingStructureFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        internal static void ReadAsDuneErosionFailureMechanism(this FailureMechanismEntity entity,
                                                               DuneErosionFailureMechanism failureMechanism,
                                                               ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadDuneErosionMechanismSectionResults(failureMechanism, collector);
            entity.ReadGeneralDuneErosionInput(failureMechanism.GeneralInput);
            entity.ReadDuneLocations(failureMechanism, collector);
            entity.ReadDuneLocationCalculations(failureMechanism, collector);
        }

        private static void ReadGeneralDuneErosionInput(this FailureMechanismEntity entity, GeneralDuneErosionInput input)
        {
            GetDuneErosionFailureMechanismMetaEntity(entity).Read(input);
        }

        private static void ReadDuneErosionMechanismSectionResults(this FailureMechanismEntity entity,
                                                                   DuneErosionFailureMechanism failureMechanism,
                                                                   ReadConversionCollector collector)
        {
            foreach (DuneErosionSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.DuneErosionSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                DuneErosionFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadDuneLocations(this FailureMechanismEntity entity,
                                              DuneErosionFailureMechanism failureMechanism,
                                              ReadConversionCollector collector)
        {
            failureMechanism.SetDuneLocations(entity.DuneLocationEntities
                                                    .OrderBy(location => location.Order)
                                                    .Select(location => location.Read(collector))
                                                    .ToArray());
        }

        private static void ReadDuneLocationCalculations(this FailureMechanismEntity entity,
                                                         DuneErosionFailureMechanism failureMechanism,
                                                         ReadConversionCollector collector)
        {
            DuneErosionFailureMechanismMetaEntity metaEntity = GetDuneErosionFailureMechanismMetaEntity(entity);

            metaEntity.DuneLocationCalculationCollectionEntity4.Read(failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm, collector);
            metaEntity.DuneLocationCalculationCollectionEntity3.Read(failureMechanism.CalculationsForMechanismSpecificSignalingNorm, collector);
            metaEntity.DuneLocationCalculationCollectionEntity2.Read(failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm, collector);
            metaEntity.DuneLocationCalculationCollectionEntity1.Read(failureMechanism.CalculationsForLowerLimitNorm, collector);
            metaEntity.DuneLocationCalculationCollectionEntity.Read(failureMechanism.CalculationsForFactorizedLowerLimitNorm, collector);
        }

        private static DuneErosionFailureMechanismMetaEntity GetDuneErosionFailureMechanismMetaEntity(FailureMechanismEntity entity)
        {
            return entity.DuneErosionFailureMechanismMetaEntities.Single();
        }

        #endregion

        #region Stability Stone Cover

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StabilityStoneCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries cannot be found.</exception>
        internal static void ReadAsStabilityStoneCoverFailureMechanism(this FailureMechanismEntity entity,
                                                                       StabilityStoneCoverFailureMechanism failureMechanism,
                                                                       ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityStoneCoverMechanismSectionResults(failureMechanism, collector);

            StabilityStoneCoverFailureMechanismMetaEntity metaEntity =
                entity.StabilityStoneCoverFailureMechanismMetaEntities.Single();
            ReadForeshoreProfiles(entity,
                                  failureMechanism.ForeshoreProfiles,
                                  metaEntity.ForeshoreProfileCollectionSourcePath,
                                  collector);

            ReadStabilityStoneCoverWaveConditionsRootCalculationGroup(entity.CalculationGroupEntity,
                                                                      failureMechanism.WaveConditionsCalculationGroup,
                                                                      collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
        }

        private static void ReadStabilityStoneCoverMechanismSectionResults(this FailureMechanismEntity entity,
                                                                           StabilityStoneCoverFailureMechanism failureMechanism,
                                                                           ReadConversionCollector collector)
        {
            foreach (StabilityStoneCoverSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityStoneCoverSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                StabilityStoneCoverFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadStabilityStoneCoverWaveConditionsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                                      CalculationGroup targetRootCalculationGroup,
                                                                                      ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsStabilityStoneCoverWaveConditionsCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralStabilityStoneCoverWaveConditionsInput generalInput)
        {
            GeneralStabilityStoneCoverWaveConditionsInput generalStabilityPointStructuresInput = entity.StabilityStoneCoverFailureMechanismMetaEntities.First().Read();
            generalInput.N = generalStabilityPointStructuresInput.N;
        }

        #endregion

        #region Stability Point Structures

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="StabilityPointStructuresFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries cannot be found.</exception>
        internal static void ReadAsStabilityPointStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                            StabilityPointStructuresFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityPointStructuresMechanismSectionResults(failureMechanism, collector);

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles,
                                         metaEntity.ForeshoreProfileCollectionSourcePath,
                                         collector);

            entity.ReadStabilityPointStructures(failureMechanism.StabilityPointStructures,
                                                metaEntity.StabilityPointStructureCollectionSourcePath,
                                                collector);
            entity.ReadGeneralInput(failureMechanism.GeneralInput);
            ReadStabilityPointStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadStabilityPointStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                StabilityPointStructuresFailureMechanism failureMechanism,
                                                                                ReadConversionCollector collector)
        {
            foreach (StabilityPointStructuresSectionResultEntity sectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.StabilityPointStructuresSectionResultEntities))
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                StabilityPointStructuresFailureMechanismSectionResult result = failureMechanism.SectionResults
                                                                                               .Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result, collector);
            }
        }

        private static void ReadStabilityPointStructures(this FailureMechanismEntity entity,
                                                         StructureCollection<StabilityPointStructure> stabilityPointStructures,
                                                         string sourcePath,
                                                         ReadConversionCollector collector)
        {
            if (sourcePath != null)
            {
                stabilityPointStructures.AddRange(entity.StabilityPointStructureEntities
                                                        .OrderBy(fpe => fpe.Order)
                                                        .Select(structureEntity => structureEntity.Read(collector))
                                                        .ToArray(),
                                                  sourcePath);
            }
        }

        private static void ReadGeneralInput(this FailureMechanismEntity entity, GeneralStabilityPointStructuresInput generalInput)
        {
            GeneralStabilityPointStructuresInput generalStabilityPointStructuresInput = entity.StabilityPointStructuresFailureMechanismMetaEntities.First().Read();
            generalInput.N = generalStabilityPointStructuresInput.N;
        }

        private static void ReadStabilityPointStructuresRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                             CalculationGroup targetRootCalculationGroup,
                                                                             ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsStabilityPointStructuresCalculationGroup(collector);
            foreach (ICalculationBase calculationBase in rootCalculationGroup.Children)
            {
                targetRootCalculationGroup.Children.Add(calculationBase);
            }
        }

        #endregion
    }
}