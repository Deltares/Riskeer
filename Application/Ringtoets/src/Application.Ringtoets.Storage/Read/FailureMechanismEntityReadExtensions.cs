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
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="PipingFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="PipingFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingFailureMechanism"/>.</returns>
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

            failureMechanism.StorageId = entity.FailureMechanismEntityId;
            failureMechanism.IsRelevant = entity.IsRelevant == 1;
            failureMechanism.Comments = entity.Comments;

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

            entity.ReadFailureMechanismSections(failureMechanism, collector);
            entity.ReadPipingMechanismSectionResults(failureMechanism, collector);

            ReadRootCalculationGroup(entity.CalculationGroupEntity, failureMechanism.CalculationsGroup,
                                     failureMechanism.GeneralInput, collector);
        }

        private static void ReadPipingMechanismSectionResults(this FailureMechanismEntity entity, PipingFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            foreach (var pipingSectionResultEntity in entity.FailureMechanismSectionEntities.SelectMany(fms => fms.PipingSectionResultEntities))
            {
                var readSectionResult = pipingSectionResultEntity.Read(collector);
                var failureMechanismSection = collector.Get(pipingSectionResultEntity.FailureMechanismSectionEntity);
                var result = failureMechanism.SectionResults.Single(sr => ReferenceEquals(sr.Section, failureMechanismSection));
                result.StorageId = readSectionResult.StorageId;
                result.AssessmentLayerOne = readSectionResult.AssessmentLayerOne;
                result.AssessmentLayerThree = readSectionResult.AssessmentLayerThree;
            }
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverErosionInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism"></param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsFailureMechanism"/>.</returns>
        internal static void ReadAsGrassCoverErosionInwardsFailureMechanism(this FailureMechanismEntity entity, GrassCoverErosionInwardsFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            failureMechanism.StorageId = entity.FailureMechanismEntityId;
            failureMechanism.IsRelevant = entity.IsRelevant == 1;
            failureMechanism.Comments = entity.Comments;

            entity.ReadFailureMechanismSections(failureMechanism, collector);
        }

        /// <summary>
        /// Read the <see cref="FailureMechanismEntity"/> and use the information to construct a <see cref="MacrostabilityInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to create <see cref="GrassCoverErosionInwardsFailureMechanism"/> for.</param>
        /// <param name="failureMechanism">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacrostabilityInwardsFailureMechanism"/>.</returns>
        internal static void ReadAsStandAloneFailureMechanism(this FailureMechanismEntity entity, IFailureMechanism failureMechanism, ReadConversionCollector collector)
        {
            failureMechanism.StorageId = entity.FailureMechanismEntityId;
            failureMechanism.IsRelevant = entity.IsRelevant == 1;
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