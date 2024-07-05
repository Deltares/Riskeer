// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.ClosingStructures;
using Riskeer.Storage.Core.Read.DuneErosion;
using Riskeer.Storage.Core.Read.FailureMechanismSectionResults;
using Riskeer.Storage.Core.Read.GrassCoverErosionInwards;
using Riskeer.Storage.Core.Read.HeightStructures;
using Riskeer.Storage.Core.Read.MacroStabilityInwards;
using Riskeer.Storage.Core.Read.Piping;
using Riskeer.Storage.Core.Read.StabilityPointStructures;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Read
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
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void ReadCommonFailureMechanismProperties<T>(this T entity,
                                                                     IFailureMechanism failureMechanism,
                                                                     ReadConversionCollector collector)
            where T : IFailureMechanismEntity
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

            failureMechanism.InAssembly = Convert.ToBoolean(entity.InAssembly);
            failureMechanism.InAssemblyInputComments.Body = entity.InAssemblyInputComments;
            failureMechanism.InAssemblyOutputComments.Body = entity.InAssemblyOutputComments;
            failureMechanism.NotInAssemblyComments.Body = entity.NotInAssemblyComments;

            entity.ReadFailureMechanismSections(failureMechanism, collector);
            ReadAssemblyResult(entity, failureMechanism);
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

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="ICalculatableFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to read into a <see cref="ICalculatableFailureMechanism"/>.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        private static void ReadCommonCalculatableFailureMechanismProperties(this FailureMechanismEntity entity,
                                                                             ICalculatableFailureMechanism failureMechanism,
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

            ReadCommonFailureMechanismProperties(entity, failureMechanism, collector);
            failureMechanism.CalculationsInputComments.Body = entity.CalculationsInputComments;
        }

        private static void ReadAssemblyResult(IFailureMechanismEntity entity, IFailureMechanism failureMechanism)
        {
            FailureMechanismAssemblyResult assemblyResult = failureMechanism.AssemblyResult;
            assemblyResult.ProbabilityResultType = (FailureMechanismAssemblyProbabilityResultType) entity.FailureMechanismAssemblyResultProbabilityResultType;
            if (entity.FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability != null)
            {
                assemblyResult.ManualFailureMechanismAssemblyProbability = entity.FailureMechanismAssemblyResultManualFailureMechanismAssemblyProbability.ToNullAsNaN();
            }
        }

        private static void ReadFailureMechanismSections(this IFailureMechanismEntity entity,
                                                         IFailureMechanism failureMechanism,
                                                         ReadConversionCollector collector)
        {
            FailureMechanismSection[] readFailureMechanismSections = entity.FailureMechanismSectionEntities
                                                                           .Select(failureMechanismSectionEntity =>
                                                                                       failureMechanismSectionEntity.Read(collector))
                                                                           .ToArray();
            if (readFailureMechanismSections.Any())
            {
                failureMechanism.SetSections(readFailureMechanismSections, entity.FailureMechanismSectionCollectionSourcePath);
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

            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);

            PipingFailureMechanismMetaEntity metaEntity = entity.PipingFailureMechanismMetaEntities.Single();
            metaEntity.ReadGeneralPipingInput(failureMechanism.GeneralInput);
            metaEntity.ReadFailureMechanismValues(failureMechanism);

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
            entity.ReadPipingFailureMechanismSectionConfigurations(failureMechanism, collector);

            ReadPipingRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadPipingMechanismSectionResults(this FailureMechanismEntity entity,
                                                              PipingFailureMechanism failureMechanism,
                                                              ReadConversionCollector collector)
        {
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(
                    sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadPipingFailureMechanismSectionConfigurations(this FailureMechanismEntity entity,
                                                                            PipingFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            IEnumerable<PipingFailureMechanismSectionConfigurationEntity> failureMechanismSectionConfigurationEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingFailureMechanismSectionConfigurationEntities);
            foreach (PipingFailureMechanismSectionConfigurationEntity sectionConfigurationEntity in failureMechanismSectionConfigurationEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionConfigurationEntity.FailureMechanismSectionEntity);
                PipingFailureMechanismSectionConfiguration configuration = failureMechanism.SectionConfigurations.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionConfigurationEntity.Read(configuration);
            }
        }

        private static void ReadPipingRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                           CalculationGroup targetRootCalculationGroup,
                                                           ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsPipingCalculationGroup(collector);
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                            GrassCoverErosionInwardsFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadDikeProfiles(failureMechanism.DikeProfiles, collector);
            ReadGrassCoverErosionInwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
            entity.ReadGrassCoverErosionInwardsMechanismSectionResults(failureMechanism, collector);
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
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsHeightStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                    HeightStructuresFailureMechanism failureMechanism,
                                                                    ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadHeightStructuresMechanismSectionResults(failureMechanism, collector);

            HeightStructuresFailureMechanismMetaEntity metaEntity = entity.HeightStructuresFailureMechanismMetaEntities.Single();
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, metaEntity.ForeshoreProfileCollectionSourcePath, collector);
            entity.ReadHeightStructures(failureMechanism.HeightStructures, metaEntity.HeightStructureCollectionSourcePath, collector);
            ReadHeightStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadHeightStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                        HeightStructuresFailureMechanism failureMechanism,
                                                                        ReadConversionCollector collector)
        {
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
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

        #region Water Pressure Asphalt

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaterPressureAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaterPressureAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
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
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsClosingStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                     ClosingStructuresFailureMechanism failureMechanism,
                                                                     ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadClosingStructuresMechanismSectionResults(failureMechanism, collector);

            ClosingStructuresFailureMechanismMetaEntity metaEntity = entity.ClosingStructuresFailureMechanismMetaEntities.Single();

            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles,
                                         metaEntity.ForeshoreProfileCollectionSourcePath,
                                         collector);
            entity.ReadClosingStructures(failureMechanism.ClosingStructures,
                                         metaEntity.ClosingStructureCollectionSourcePath, collector);

            ReadClosingStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadClosingStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                         ClosingStructuresFailureMechanism failureMechanism,
                                                                         ReadConversionCollector collector)
        {
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
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

            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);

            MacroStabilityInwardsFailureMechanismMetaEntity metaEntity = entity.MacroStabilityInwardsFailureMechanismMetaEntities.Single();

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
            entity.ReadMacroStabilityInwardsFailureMechanismSectionConfigurations(failureMechanism, collector);
            ReadMacroStabilityInwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadMacroStabilityInwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                             MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadMacroStabilityInwardsFailureMechanismSectionConfigurations(this FailureMechanismEntity entity,
                                                                                           MacroStabilityInwardsFailureMechanism failureMechanism,
                                                                                           ReadConversionCollector collector)
        {
            IEnumerable<MacroStabilityInwardsFailureMechanismSectionConfigurationEntity> failureMechanismSectionConfigurationEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.MacroStabilityInwardsFailureMechanismSectionConfigurationEntities);
            foreach (MacroStabilityInwardsFailureMechanismSectionConfigurationEntity sectionConfigurationEntity in failureMechanismSectionConfigurationEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionConfigurationEntity.FailureMechanismSectionEntity);
                FailureMechanismSectionConfiguration configuration = failureMechanism.SectionConfigurations.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionConfigurationEntity.Read(configuration);
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

        #region Wave Impact Asphalt Cover

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to update a <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="WaveImpactAsphaltCoverFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsWaveImpactAsphaltCoverFailureMechanism(this FailureMechanismEntity entity,
                                                                          WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                          ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadWaveImpactAsphaltCoverMechanismSectionResults(failureMechanism, collector);

            WaveImpactAsphaltCoverFailureMechanismMetaEntity metaEntity = entity.WaveImpactAsphaltCoverFailureMechanismMetaEntities.Single();
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles, metaEntity.ForeshoreProfileCollectionSourcePath, collector);

            ReadWaveImpactAsphaltCoverRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadWaveImpactAsphaltCoverMechanismSectionResults(this FailureMechanismEntity entity,
                                                                              WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                                              ReadConversionCollector collector)
        {
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadWaveImpactAsphaltCoverRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                           CalculationGroup targetRootCalculationGroup,
                                                                           ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsWaveImpactAsphaltCoverCalculationGroup(collector);
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsGrassCoverErosionOutwardsFailureMechanism(this FailureMechanismEntity entity,
                                                                             GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                             ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadGrassCoverErosionOutwardsMechanismSectionResults(failureMechanism, collector);

            GrassCoverErosionOutwardsFailureMechanismMetaEntity metaEntity =
                entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Single();
            ReadForeshoreProfiles(entity,
                                  failureMechanism.ForeshoreProfiles,
                                  metaEntity.ForeshoreProfileCollectionSourcePath,
                                  collector);

            ReadGrassCoverErosionOutwardsRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadGrassCoverErosionOutwardsMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                                 ReadConversionCollector collector)
        {
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadGrassCoverErosionOutwardsRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                              CalculationGroup targetRootCalculationGroup, ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsGrassCoverErosionOutwardsCalculationGroup(collector);
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
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
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
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
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
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
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
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
        }

        private static void ReadPipingStructureMechanismSectionResults(this FailureMechanismEntity entity,
                                                                       PipingStructureFailureMechanism failureMechanism,
                                                                       ReadConversionCollector collector)
        {
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities = entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsDuneErosionFailureMechanism(this FailureMechanismEntity entity,
                                                               DuneErosionFailureMechanism failureMechanism,
                                                               ReadConversionCollector collector)
        {
            entity.ReadCommonFailureMechanismProperties(failureMechanism, collector);
            failureMechanism.CalculationsInputComments.Body = entity.CalculationsInputComments;

            entity.ReadDuneErosionMechanismSectionResults(failureMechanism, collector);
            entity.ReadDuneLocations(failureMechanism, collector);
            entity.ReadDuneLocationCalculations(failureMechanism, collector);
        }

        private static void ReadDuneErosionMechanismSectionResults(this FailureMechanismEntity entity,
                                                                   DuneErosionFailureMechanism failureMechanism,
                                                                   ReadConversionCollector collector)
        {
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

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

            failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities.AddRange(metaEntity.DuneLocationCalculationForTargetProbabilityCollectionEntities
                                                                                                          .OrderBy(e => e.Order)
                                                                                                          .Select(e => e.Read(collector))
                                                                                                          .ToArray());
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsStabilityStoneCoverFailureMechanism(this FailureMechanismEntity entity,
                                                                       StabilityStoneCoverFailureMechanism failureMechanism,
                                                                       ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityStoneCoverMechanismSectionResults(failureMechanism, collector);

            StabilityStoneCoverFailureMechanismMetaEntity metaEntity =
                entity.StabilityStoneCoverFailureMechanismMetaEntities.Single();
            ReadForeshoreProfiles(entity,
                                  failureMechanism.ForeshoreProfiles,
                                  metaEntity.ForeshoreProfileCollectionSourcePath,
                                  collector);

            ReadStabilityStoneCoverRootCalculationGroup(entity.CalculationGroupEntity,
                                                        failureMechanism.CalculationsGroup,
                                                        collector);
        }

        private static void ReadStabilityStoneCoverMechanismSectionResults(this FailureMechanismEntity entity,
                                                                           StabilityStoneCoverFailureMechanism failureMechanism,
                                                                           ReadConversionCollector collector)
        {
            IEnumerable<NonAdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.NonAdoptableFailureMechanismSectionResultEntities);
            foreach (NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                NonAdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
            }
        }

        private static void ReadStabilityStoneCoverRootCalculationGroup(CalculationGroupEntity rootCalculationGroupEntity,
                                                                        CalculationGroup targetRootCalculationGroup,
                                                                        ReadConversionCollector collector)
        {
            CalculationGroup rootCalculationGroup = rootCalculationGroupEntity.ReadAsStabilityStoneCoverCalculationGroup(collector);
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
        /// <exception cref="ArgumentNullException">Thrown when any argument is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when expected table entries could not be found.</exception>
        internal static void ReadAsStabilityPointStructuresFailureMechanism(this FailureMechanismEntity entity,
                                                                            StabilityPointStructuresFailureMechanism failureMechanism,
                                                                            ReadConversionCollector collector)
        {
            entity.ReadCommonCalculatableFailureMechanismProperties(failureMechanism, collector);
            entity.ReadStabilityPointStructuresMechanismSectionResults(failureMechanism, collector);

            StabilityPointStructuresFailureMechanismMetaEntity metaEntity =
                entity.StabilityPointStructuresFailureMechanismMetaEntities.Single();
            entity.ReadForeshoreProfiles(failureMechanism.ForeshoreProfiles,
                                         metaEntity.ForeshoreProfileCollectionSourcePath,
                                         collector);

            entity.ReadStabilityPointStructures(failureMechanism.StabilityPointStructures,
                                                metaEntity.StabilityPointStructureCollectionSourcePath,
                                                collector);
            ReadStabilityPointStructuresRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup, collector);
        }

        private static void ReadStabilityPointStructuresMechanismSectionResults(this FailureMechanismEntity entity,
                                                                                StabilityPointStructuresFailureMechanism failureMechanism,
                                                                                ReadConversionCollector collector)
        {
            IEnumerable<AdoptableFailureMechanismSectionResultEntity> sectionResultEntities =
                entity.FailureMechanismSectionEntities.SelectMany(fms => fms.AdoptableFailureMechanismSectionResultEntities);
            foreach (AdoptableFailureMechanismSectionResultEntity sectionResultEntity in sectionResultEntities)
            {
                FailureMechanismSection failureMechanismSection = collector.Get(sectionResultEntity.FailureMechanismSectionEntity);
                AdoptableFailureMechanismSectionResult result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));

                sectionResultEntity.Read(result);
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