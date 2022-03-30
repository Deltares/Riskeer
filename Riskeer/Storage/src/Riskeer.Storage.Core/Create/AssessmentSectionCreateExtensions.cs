﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.Create.ClosingStructures;
using Riskeer.Storage.Core.Create.DuneErosion;
using Riskeer.Storage.Core.Create.GrassCoverErosionInwards;
using Riskeer.Storage.Core.Create.GrassCoverErosionOutwards;
using Riskeer.Storage.Core.Create.GrassCoverSlipOffInwards;
using Riskeer.Storage.Core.Create.GrassCoverSlipOffOutwards;
using Riskeer.Storage.Core.Create.HeightStructures;
using Riskeer.Storage.Core.Create.MacroStabilityInwards;
using Riskeer.Storage.Core.Create.Microstability;
using Riskeer.Storage.Core.Create.Piping;
using Riskeer.Storage.Core.Create.PipingStructure;
using Riskeer.Storage.Core.Create.StabilityPointStructures;
using Riskeer.Storage.Core.Create.StabilityStoneCover;
using Riskeer.Storage.Core.Create.WaterPressureAsphaltCover;
using Riskeer.Storage.Core.Create.WaveImpactAsphaltCover;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="AssessmentSection"/> related to creating an <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    internal static class AssessmentSectionCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="AssessmentSectionEntity"/> based on the information of the <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="section">The section to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static AssessmentSectionEntity Create(this AssessmentSection section, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            FailureMechanismContribution contribution = section.FailureMechanismContribution;
            var entity = new AssessmentSectionEntity
            {
                Id = section.Id.DeepClone(),
                Name = section.Name.DeepClone(),
                Composition = Convert.ToByte(section.Composition),
                Comments = section.Comments.Body.DeepClone(),
                LowerLimitNorm = contribution.LowerLimitNorm,
                SignalingNorm = contribution.SignalingNorm,
                NormativeNormType = Convert.ToByte(contribution.NormativeNorm)
            };

            AddEntityForHydraulicDatabase(section.HydraulicBoundaryDatabase, entity, registry);
            AddHydraulicLocationCalculationEntities(section, entity, registry);
            AddHydraulicLocationCalculationForTargetProbabilityCollectionEntities(section, entity, registry);
            AddEntityForReferenceLine(section, entity);

            entity.BackgroundDataEntities.Add(section.BackgroundData.Create());

            entity.FailureMechanismEntities.Add(section.Piping.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.HeightStructures.Create(registry));
            entity.FailureMechanismEntities.Add(section.WaterPressureAsphaltCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.ClosingStructures.Create(registry));
            entity.FailureMechanismEntities.Add(section.MacroStabilityInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.WaveImpactAsphaltCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverErosionOutwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverSlipOffInwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.GrassCoverSlipOffOutwards.Create(registry));
            entity.FailureMechanismEntities.Add(section.Microstability.Create(registry));
            entity.FailureMechanismEntities.Add(section.PipingStructure.Create(registry));
            entity.FailureMechanismEntities.Add(section.StabilityStoneCover.Create(registry));
            entity.FailureMechanismEntities.Add(section.DuneErosion.Create(registry));
            entity.FailureMechanismEntities.Add(section.StabilityPointStructures.Create(registry));

            AddSpecificFailurePathEntities(section, entity, registry);

            return entity;
        }

        private static void AddSpecificFailurePathEntities(AssessmentSection section, AssessmentSectionEntity entity, PersistenceRegistry registry)
        {
            var i = 0;
            foreach (SpecificFailureMechanism specificFailureMechanism in section.SpecificFailureMechanisms)
            {
                entity.SpecificFailureMechanismEntities.Add(specificFailureMechanism.Create(registry, i++));
            }
        }

        private static void AddEntityForReferenceLine(AssessmentSection section, AssessmentSectionEntity entity)
        {
            if (section.ReferenceLine.Points.Any())
            {
                entity.ReferenceLinePointXml = new Point2DCollectionXmlSerializer().ToXml(section.ReferenceLine.Points);
            }
        }

        private static void AddEntityForHydraulicDatabase(HydraulicBoundaryDatabase hydraulicBoundaryDatabase, AssessmentSectionEntity entity, PersistenceRegistry registry)
        {
            if (hydraulicBoundaryDatabase.IsLinked())
            {
                entity.HydraulicBoundaryDatabaseEntities.Add(hydraulicBoundaryDatabase.Create());

                for (var i = 0; i < hydraulicBoundaryDatabase.Locations.Count; i++)
                {
                    HydraulicBoundaryLocation hydraulicBoundaryLocation = hydraulicBoundaryDatabase.Locations[i];
                    entity.HydraulicLocationEntities.Add(hydraulicBoundaryLocation.Create(registry, i));
                }
            }
        }

        private static void AddHydraulicLocationCalculationEntities(AssessmentSection assessmentSection,
                                                                    AssessmentSectionEntity entity,
                                                                    PersistenceRegistry registry)
        {
            entity.HydraulicLocationCalculationCollectionEntity1 = assessmentSection.WaterLevelCalculationsForSignalingNorm.Create(registry);
            entity.HydraulicLocationCalculationCollectionEntity = assessmentSection.WaterLevelCalculationsForLowerLimitNorm.Create(registry);
        }

        private static void AddHydraulicLocationCalculationForTargetProbabilityCollectionEntities(AssessmentSection assessmentSection,
                                                                                                  AssessmentSectionEntity entity,
                                                                                                  PersistenceRegistry registry)
        {
            AddHydraulicLocationCalculationForTargetProbabilityCollectionEntities(assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities,
                                                                                  HydraulicBoundaryLocationCalculationType.WaterLevel,
                                                                                  entity,
                                                                                  registry);

            AddHydraulicLocationCalculationForTargetProbabilityCollectionEntities(assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities,
                                                                                  HydraulicBoundaryLocationCalculationType.WaveHeight,
                                                                                  entity,
                                                                                  registry);
        }

        private static void AddHydraulicLocationCalculationForTargetProbabilityCollectionEntities(
            List<HydraulicBoundaryLocationCalculationsForTargetProbability> hydraulicBoundaryLocationCalculationsForTargetProbabilities,
            HydraulicBoundaryLocationCalculationType calculationType,
            AssessmentSectionEntity entity,
            PersistenceRegistry registry)
        {
            for (int i = 0; i < hydraulicBoundaryLocationCalculationsForTargetProbabilities.Count; i++)
            {
                entity.HydraulicLocationCalculationForTargetProbabilityCollectionEntities.Add(
                    hydraulicBoundaryLocationCalculationsForTargetProbabilities[i].Create(calculationType, i, registry));
            }
        }
    }
}