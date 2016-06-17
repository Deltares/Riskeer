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
using System.Collections.Generic;
using System.Linq;

using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// This class can be used to keep track of create and update operations on a database.
    /// This information can be used to reuse objects. When all operations have been performed,
    /// then the collected information can be used to transfer the ids assigned to the created
    /// database instances back to the data model or to clean up orphans.
    /// </summary>
    internal class PersistenceRegistry
    {
        private readonly Dictionary<ProjectEntity, Project> projects = new Dictionary<ProjectEntity, Project>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly Dictionary<AssessmentSectionEntity, AssessmentSection> assessmentSections = new Dictionary<AssessmentSectionEntity, AssessmentSection>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly Dictionary<FailureMechanismEntity, IFailureMechanism> failureMechanisms = new Dictionary<FailureMechanismEntity, IFailureMechanism>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections = new Dictionary<FailureMechanismSectionEntity, FailureMechanismSection>();
        private readonly Dictionary<PipingSectionResultEntity, PipingFailureMechanismSectionResult> pipingFailureMechanismSectionResults = new Dictionary<PipingSectionResultEntity, PipingFailureMechanismSectionResult>();
        private readonly Dictionary<GrassCoverErosionInwardsSectionResultEntity, GrassCoverErosionInwardsFailureMechanismSectionResult> grassCoverErosionInwardsFailureMechanismSectionResults = new Dictionary<GrassCoverErosionInwardsSectionResultEntity, GrassCoverErosionInwardsFailureMechanismSectionResult>();
        private readonly Dictionary<HeightStructuresSectionResultEntity, HeightStructuresFailureMechanismSectionResult> heightStructuresFailureMechanismSectionResults = new Dictionary<HeightStructuresSectionResultEntity, HeightStructuresFailureMechanismSectionResult>();
        private readonly Dictionary<StrengthStabilityLengthwiseConstructionSectionResultEntity, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult> strengthStabilityLengthwiseConstructionFailureMechanismSectionResults = new Dictionary<StrengthStabilityLengthwiseConstructionSectionResultEntity, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>();
        private readonly Dictionary<TechnicalInnovationSectionResultEntity, TechnicalInnovationFailureMechanismSectionResult> technicalInnovationFailureMechanismSectionResults = new Dictionary<TechnicalInnovationSectionResultEntity, TechnicalInnovationFailureMechanismSectionResult>();
        private readonly Dictionary<WaterPressureAsphaltCoverSectionResultEntity, WaterPressureAsphaltCoverFailureMechanismSectionResult> waterPressureAsphaltCoverFailureMechanismSectionResults = new Dictionary<WaterPressureAsphaltCoverSectionResultEntity, WaterPressureAsphaltCoverFailureMechanismSectionResult>();
        private readonly Dictionary<ClosingStructureSectionResultEntity, ClosingStructureFailureMechanismSectionResult> closingStructureFailureMechanismSectionResults = new Dictionary<ClosingStructureSectionResultEntity, ClosingStructureFailureMechanismSectionResult>();
        private readonly Dictionary<MacrostabilityInwardsSectionResultEntity, MacrostabilityInwardsFailureMechanismSectionResult> macrostabilityInwardsFailureMechanismSectionResults = new Dictionary<MacrostabilityInwardsSectionResultEntity, MacrostabilityInwardsFailureMechanismSectionResult>();
        private readonly Dictionary<MacrostabilityOutwardsSectionResultEntity, MacrostabilityOutwardsFailureMechanismSectionResult> macrostabilityOutwardsFailureMechanismSectionResults = new Dictionary<MacrostabilityOutwardsSectionResultEntity, MacrostabilityOutwardsFailureMechanismSectionResult>();
        private readonly Dictionary<WaveImpactAsphaltCoverSectionResultEntity, WaveImpactAsphaltCoverFailureMechanismSectionResult> waveImpactAsphaltCoverFailureMechanismSectionResults = new Dictionary<WaveImpactAsphaltCoverSectionResultEntity, WaveImpactAsphaltCoverFailureMechanismSectionResult>();
        private readonly Dictionary<GrassCoverErosionOutwardsSectionResultEntity, GrassCoverErosionOutwardsFailureMechanismSectionResult> grassCoverErosionOutwardsFailureMechanismSectionResults = new Dictionary<GrassCoverErosionOutwardsSectionResultEntity, GrassCoverErosionOutwardsFailureMechanismSectionResult>();
        private readonly Dictionary<GrassCoverSlipOffInwardsSectionResultEntity, GrassCoverSlipOffInwardsFailureMechanismSectionResult> grassCoverSlipOffInwardsFailureMechanismSectionResults = new Dictionary<GrassCoverSlipOffInwardsSectionResultEntity, GrassCoverSlipOffInwardsFailureMechanismSectionResult>();
        private readonly Dictionary<GrassCoverSlipOffOutwardsSectionResultEntity, GrassCoverSlipOffOutwardsFailureMechanismSectionResult> grassCoverSlipOffOutwardsFailureMechanismSectionResults = new Dictionary<GrassCoverSlipOffOutwardsSectionResultEntity, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>();
        private readonly Dictionary<MicrostabilitySectionResultEntity, MicrostabilityFailureMechanismSectionResult> microstabilityFailureMechanismSectionResults = new Dictionary<MicrostabilitySectionResultEntity, MicrostabilityFailureMechanismSectionResult>();
        private readonly Dictionary<PipingStructureSectionResultEntity, PipingStructureFailureMechanismSectionResult> pipingStructureFailureMechanismSectionResults = new Dictionary<PipingStructureSectionResultEntity, PipingStructureFailureMechanismSectionResult>();
        private readonly Dictionary<DuneErosionSectionResultEntity, DuneErosionFailureMechanismSectionResult> duneErosionFailureMechanismSectionResults = new Dictionary<DuneErosionSectionResultEntity, DuneErosionFailureMechanismSectionResult>();
        private readonly Dictionary<StabilityStoneCoverSectionResultEntity, StabilityStoneCoverFailureMechanismSectionResult> stabilityStoneCoverFailureMechanismSectionResults = new Dictionary<StabilityStoneCoverSectionResultEntity, StabilityStoneCoverFailureMechanismSectionResult>();
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicLocations = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly Dictionary<CalculationGroupEntity, CalculationGroup> calculationGroups = new Dictionary<CalculationGroupEntity, CalculationGroup>(new ReferenceEqualityComparer<CalculationGroupEntity>());
        private readonly Dictionary<PipingCalculationEntity, PipingCalculationScenario> pipingCalculations = new Dictionary<PipingCalculationEntity, PipingCalculationScenario>(new ReferenceEqualityComparer<PipingCalculationEntity>());
        private readonly Dictionary<PipingCalculationOutputEntity, PipingOutput> pipingOutputs = new Dictionary<PipingCalculationOutputEntity, PipingOutput>(new ReferenceEqualityComparer<PipingCalculationOutputEntity>());
        private readonly Dictionary<PipingSemiProbabilisticOutputEntity, PipingSemiProbabilisticOutput> pipingSemiProbabilisticOutputs = new Dictionary<PipingSemiProbabilisticOutputEntity, PipingSemiProbabilisticOutput>(new ReferenceEqualityComparer<PipingSemiProbabilisticOutputEntity>());
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SoilLayerEntity, PipingSoilLayer> soilLayers = new Dictionary<SoilLayerEntity, PipingSoilLayer>(new ReferenceEqualityComparer<SoilLayerEntity>());
        private readonly Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine> surfaceLines = new Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine>(new ReferenceEqualityComparer<SurfaceLineEntity>());
        private readonly Dictionary<SurfaceLinePointEntity, Point3D> surfaceLinePoints = new Dictionary<SurfaceLinePointEntity, Point3D>(new ReferenceEqualityComparer<SurfaceLinePointEntity>());
        private readonly Dictionary<CharacteristicPointEntity, Point3D> characteristicPoints = new Dictionary<CharacteristicPointEntity, Point3D>(new ReferenceEqualityComparer<CharacteristicPointEntity>());
        private readonly Dictionary<PipingFailureMechanismMetaEntity, PipingProbabilityAssessmentInput> pipingProbabilityAssessmentInputs = new Dictionary<PipingFailureMechanismMetaEntity, PipingProbabilityAssessmentInput>(new ReferenceEqualityComparer<PipingFailureMechanismMetaEntity>());

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(FailureMechanismSectionEntity entity, FailureMechanismSection model)
        {
            Register(failureMechanismSections, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(PipingSectionResultEntity entity, PipingFailureMechanismSectionResult model)
        {
            Register(pipingFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(GrassCoverErosionInwardsSectionResultEntity entity, GrassCoverErosionInwardsFailureMechanismSectionResult model)
        {
            Register(grassCoverErosionInwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HeightStructuresFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(HeightStructuresSectionResultEntity entity, HeightStructuresFailureMechanismSectionResult model)
        {
            Register(heightStructuresFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StrengthStabilityLengthwiseConstructionSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(StrengthStabilityLengthwiseConstructionSectionResultEntity entity, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult model)
        {
            Register(strengthStabilityLengthwiseConstructionFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="TechnicalInnovationSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="TechnicalInnovationFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(TechnicalInnovationSectionResultEntity entity, TechnicalInnovationFailureMechanismSectionResult model)
        {
            Register(technicalInnovationFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="WaterPressureAsphaltCoverSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="WaterPressureAsphaltCoverFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(WaterPressureAsphaltCoverSectionResultEntity entity, WaterPressureAsphaltCoverFailureMechanismSectionResult model)
        {
            Register(waterPressureAsphaltCoverFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="ClosingStructureFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(ClosingStructureSectionResultEntity entity, ClosingStructureFailureMechanismSectionResult model)
        {
            Register(closingStructureFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacrostabilityInwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacrostabilityInwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(MacrostabilityInwardsSectionResultEntity entity, MacrostabilityInwardsFailureMechanismSectionResult model)
        {
            Register(macrostabilityInwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacrostabilityOutwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(MacrostabilityOutwardsSectionResultEntity entity, MacrostabilityOutwardsFailureMechanismSectionResult model)
        {
            Register(macrostabilityOutwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="WaveImpactAsphaltCoverSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="WaveImpactAsphaltCoverFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(WaveImpactAsphaltCoverSectionResultEntity entity, WaveImpactAsphaltCoverFailureMechanismSectionResult model)
        {
            Register(waveImpactAsphaltCoverFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(GrassCoverErosionOutwardsSectionResultEntity entity, GrassCoverErosionOutwardsFailureMechanismSectionResult model)
        {
            Register(grassCoverErosionOutwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverSlipOffInwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="GrassCoverSlipOffInwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(GrassCoverSlipOffInwardsSectionResultEntity entity, GrassCoverSlipOffInwardsFailureMechanismSectionResult model)
        {
            Register(grassCoverSlipOffInwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="GrassCoverSlipOffOutwardsFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(GrassCoverSlipOffOutwardsSectionResultEntity entity, GrassCoverSlipOffOutwardsFailureMechanismSectionResult model)
        {
            Register(grassCoverSlipOffOutwardsFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MicrostabilitySectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MicrostabilityFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(MicrostabilitySectionResultEntity entity, MicrostabilityFailureMechanismSectionResult model)
        {
            Register(microstabilityFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStructureSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingStructureFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(PipingStructureSectionResultEntity entity, PipingStructureFailureMechanismSectionResult model)
        {
            Register(pipingStructureFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="DuneErosionSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="DuneErosionFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(DuneErosionSectionResultEntity entity, DuneErosionFailureMechanismSectionResult model)
        {
            Register(duneErosionFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityStoneCoverSectionResultEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StabilityStoneCoverFailureMechanismSectionResult"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(StabilityStoneCoverSectionResultEntity entity, StabilityStoneCoverFailureMechanismSectionResult model)
        {
            Register(stabilityStoneCoverFailureMechanismSectionResults, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="CalculationGroup"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(CalculationGroupEntity entity, CalculationGroup model)
        {
            Register(calculationGroups, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingCalculationEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingCalculationScenario"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(PipingCalculationEntity entity, PipingCalculationScenario model)
        {
            Register(pipingCalculations, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingCalculationOutputEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingOutput"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(PipingCalculationOutputEntity entity, PipingOutput model)
        {
            Register(pipingOutputs, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSemiProbabilisticOutputEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSemiProbabilisticOutput"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(PipingSemiProbabilisticOutputEntity entity, PipingSemiProbabilisticOutput model)
        {
            Register(pipingSemiProbabilisticOutputs, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="Project"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(ProjectEntity entity, Project model)
        {
            Register(projects, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="AssessmentSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(AssessmentSectionEntity entity, AssessmentSection model)
        {
            Register(assessmentSections, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Register(hydraulicLocations, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="IFailureMechanism"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(FailureMechanismEntity entity, IFailureMechanism model)
        {
            Register(failureMechanisms, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            Register(stochasticSoilModels, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(StochasticSoilProfileEntity entity, StochasticSoilProfile model)
        {
            Register(stochasticSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SoilProfileEntity entity, PipingSoilProfile model)
        {
            Register(soilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSoilLayer"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SoilLayerEntity entity, PipingSoilLayer model)
        {
            Register(soilLayers, entity, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilModel model)
        {
            return ContainsValue(stochasticSoilModels, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilProfile model)
        {
            return ContainsValue(stochasticSoilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(soilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(HydraulicBoundaryLocation model)
        {
            return ContainsValue(hydraulicLocations, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(RingtoetsPipingSurfaceLine model)
        {
            return ContainsValue(surfaceLines, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(FailureMechanismSection model)
        {
            return ContainsValue(failureMechanismSections, model);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilModelEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> for which a create/update
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilModel)"/> to find out whether
        /// a create/update operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilModelEntity Get(StochasticSoilModel model)
        {
            return Get(stochasticSoilModels, model);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilProfileEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> for which a create/update
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilProfile)"/> to find out whether
        /// a create/create operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilProfileEntity Get(StochasticSoilProfile model)
        {
            return Get(stochasticSoilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SoilProfileEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> for which a create/update
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingSoilProfile)"/> to find out whether a
        /// create/update operation has been registered for <paramref name="model"/>.</remarks>
        internal SoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(soilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLineEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> for which a
        /// read/update operation has been registered.</param>
        /// <returns>The constructed <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(RingtoetsPipingSurfaceLine)"/> to find out
        /// whether a create/update operation has been registered for <paramref name="model"/>.</remarks>
        internal SurfaceLineEntity Get(RingtoetsPipingSurfaceLine model)
        {
            return Get(surfaceLines, model);
        }

        /// <summary>
        /// Obtains the <see cref="HydraulicLocationEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> for which a
        /// read/update operation has been registered.</param>
        /// <returns>The constructed <see cref="HydraulicLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(HydraulicBoundaryLocation)"/> to find out
        /// whether a create/update operation has been registered for <paramref name="model"/>.</remarks>
        internal HydraulicLocationEntity Get(HydraulicBoundaryLocation model)
        {
            return Get(hydraulicLocations, model);
        }

        /// <summary>
        /// Obtains the <see cref="FailureMechanismSection"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> for which a
        /// read/update operation has been registered.</param>
        /// <returns>The constructed <see cref="FailureMechanismSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(FailureMechanismSection)"/> to find out
        /// whether a create/update operation has been registered for <paramref name="model"/>.</remarks>
        internal FailureMechanismSectionEntity Get(FailureMechanismSection model)
        {
            return Get(failureMechanismSections, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SurfaceLineEntity entity, RingtoetsPipingSurfaceLine model)
        {
            Register(surfaceLines, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> to be registered.</param>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SurfaceLinePointEntity entity, Point3D model)
        {
            Register(surfaceLinePoints, entity, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLinePointEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> for which
        /// a create or update operation has been registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create/update operation
        /// has been registered for <paramref name="model"/>.</exception>
        internal SurfaceLinePointEntity GetSurfaceLinePoint(Point3D model)
        {
            return Get(surfaceLinePoints, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="CharacteristicPointEntity"/> to be registered.</param>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> corresponding
        /// to the characteristic point data to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(CharacteristicPointEntity entity, Point3D model)
        {
            Register(characteristicPoints, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingProbabilityAssessmentInput"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(PipingFailureMechanismMetaEntity entity, PipingProbabilityAssessmentInput model)
        {
            Register(pipingProbabilityAssessmentInputs, entity, model);
        }

        /// <summary>
        /// Transfer ids from the created entities to the domain model objects' property.
        /// </summary>
        internal void TransferIds()
        {
            foreach (var entity in projects.Keys)
            {
                projects[entity].StorageId = entity.ProjectEntityId;
            }

            foreach (var entity in failureMechanisms.Keys)
            {
                failureMechanisms[entity].StorageId = entity.FailureMechanismEntityId;
            }

            foreach (var entity in failureMechanismSections.Keys)
            {
                failureMechanismSections[entity].StorageId = entity.FailureMechanismSectionEntityId;
            }

            foreach (var entity in assessmentSections.Keys)
            {
                assessmentSections[entity].StorageId = entity.AssessmentSectionEntityId;
            }

            foreach (var entity in pipingFailureMechanismSectionResults.Keys)
            {
                pipingFailureMechanismSectionResults[entity].StorageId = entity.PipingSectionResultEntityId;
            }

            foreach (var entity in grassCoverErosionInwardsFailureMechanismSectionResults.Keys)
            {
                grassCoverErosionInwardsFailureMechanismSectionResults[entity].StorageId = entity.GrassCoverErosionInwardsSectionResultEntityId;
            }

            foreach (var entity in heightStructuresFailureMechanismSectionResults.Keys)
            {
                heightStructuresFailureMechanismSectionResults[entity].StorageId = entity.HeightStructuresSectionResultEntityId;
            }

            foreach (var entity in strengthStabilityLengthwiseConstructionFailureMechanismSectionResults.Keys)
            {
                strengthStabilityLengthwiseConstructionFailureMechanismSectionResults[entity].StorageId = entity.StrengthStabilityLengthwiseConstructionSectionResultEntityId;
            }

            foreach (var entity in technicalInnovationFailureMechanismSectionResults.Keys)
            {
                technicalInnovationFailureMechanismSectionResults[entity].StorageId = entity.TechnicalInnovationSectionResultEntityId;
            }

            foreach (var entity in waterPressureAsphaltCoverFailureMechanismSectionResults.Keys)
            {
                waterPressureAsphaltCoverFailureMechanismSectionResults[entity].StorageId = entity.WaterPressureAsphaltCoverSectionResultEntityId;
            }

            foreach (var entity in closingStructureFailureMechanismSectionResults.Keys)
            {
                closingStructureFailureMechanismSectionResults[entity].StorageId = entity.ClosingStructureSectionResultEntityId;
            }

            foreach (var entity in macrostabilityInwardsFailureMechanismSectionResults.Keys)
            {
                macrostabilityInwardsFailureMechanismSectionResults[entity].StorageId = entity.MacrostabilityInwardsSectionResultEntityId;
            }

            foreach (var entity in macrostabilityOutwardsFailureMechanismSectionResults.Keys)
            {
                macrostabilityOutwardsFailureMechanismSectionResults[entity].StorageId = entity.MacrostabilityOutwardsSectionResultEntityId;
            }

            foreach (var entity in waveImpactAsphaltCoverFailureMechanismSectionResults.Keys)
            {
                waveImpactAsphaltCoverFailureMechanismSectionResults[entity].StorageId = entity.WaveImpactAsphaltCoverSectionResultEntityId;
            }

            foreach (var entity in grassCoverErosionOutwardsFailureMechanismSectionResults.Keys)
            {
                grassCoverErosionOutwardsFailureMechanismSectionResults[entity].StorageId = entity.GrassCoverErosionOutwardsSectionResultEntityId;
            }

            foreach (var entity in grassCoverSlipOffInwardsFailureMechanismSectionResults.Keys)
            {
                grassCoverSlipOffInwardsFailureMechanismSectionResults[entity].StorageId = entity.GrassCoverSlipOffInwardsSectionResultEntityId;
            }

            foreach (var entity in grassCoverSlipOffOutwardsFailureMechanismSectionResults.Keys)
            {
                grassCoverSlipOffOutwardsFailureMechanismSectionResults[entity].StorageId = entity.GrassCoverSlipOffOutwardsSectionResultEntityId;
            }

            foreach (var entity in microstabilityFailureMechanismSectionResults.Keys)
            {
                microstabilityFailureMechanismSectionResults[entity].StorageId = entity.MicrostabilitySectionResultEntityId;
            }

            foreach (var entity in pipingStructureFailureMechanismSectionResults.Keys)
            {
                pipingStructureFailureMechanismSectionResults[entity].StorageId = entity.PipingStructureSectionResultEntityId;
            }

            foreach (var entity in duneErosionFailureMechanismSectionResults.Keys)
            {
                duneErosionFailureMechanismSectionResults[entity].StorageId = entity.DuneErosionSectionResultEntityId;
            }

            foreach (var entity in stabilityStoneCoverFailureMechanismSectionResults.Keys)
            {
                stabilityStoneCoverFailureMechanismSectionResults[entity].StorageId = entity.StabilityStoneCoverSectionResultEntityId;
            }

            foreach (var entity in hydraulicLocations.Keys)
            {
                hydraulicLocations[entity].StorageId = entity.HydraulicLocationEntityId;
            }

            foreach (var entity in calculationGroups.Keys)
            {
                calculationGroups[entity].StorageId = entity.CalculationGroupEntityId;
            }

            foreach (var entity in pipingCalculations.Keys)
            {
                pipingCalculations[entity].StorageId = entity.PipingCalculationEntityId;
            }

            foreach (var entity in pipingOutputs.Keys)
            {
                pipingOutputs[entity].StorageId = entity.PipingCalculationOutputEntityId;
            }

            foreach (var entity in pipingSemiProbabilisticOutputs.Keys)
            {
                pipingSemiProbabilisticOutputs[entity].StorageId = entity.PipingSemiProbabilisticOutputEntityId;
            }

            foreach (var entity in stochasticSoilModels.Keys)
            {
                stochasticSoilModels[entity].StorageId = entity.StochasticSoilModelEntityId;
            }

            foreach (var entity in stochasticSoilProfiles.Keys)
            {
                stochasticSoilProfiles[entity].StorageId = entity.StochasticSoilProfileEntityId;
            }

            foreach (var entity in soilProfiles.Keys)
            {
                soilProfiles[entity].StorageId = entity.SoilProfileEntityId;
            }

            foreach (var entity in soilLayers.Keys)
            {
                soilLayers[entity].StorageId = entity.SoilLayerEntityId;
            }

            foreach (var entity in surfaceLines.Keys)
            {
                surfaceLines[entity].StorageId = entity.SurfaceLineEntityId;
            }

            foreach (var entity in surfaceLinePoints.Keys)
            {
                surfaceLinePoints[entity].StorageId = entity.SurfaceLinePointEntityId;
            }

            foreach (var entity in pipingProbabilityAssessmentInputs.Keys)
            {
                pipingProbabilityAssessmentInputs[entity].StorageId = entity.PipingFailureMechanismMetaEntityId;
            }

            // CharacteristicPoints do not really have a 'identity' within the object-model.
            // As such, no need to copy StorageId. This is already covered by surfaceLinePoints.
        }

        /// <summary>
        /// Removes all the entities for which no update operation was registered from the <paramref name="dbContext"/>.
        /// </summary>
        /// <param name="dbContext">The <see cref="IRingtoetsEntities"/> from which to remove the entities.</param>
        internal void RemoveUntouched(IRingtoetsEntities dbContext)
        {
            var orphanedProjectEntities = new List<ProjectEntity>();
            foreach (ProjectEntity projectEntity in dbContext.ProjectEntities
                                                             .Where(e => e.ProjectEntityId > 0))
            {
                if (!projects.ContainsKey(projectEntity))
                {
                    orphanedProjectEntities.Add(projectEntity);
                }
            }
            dbContext.ProjectEntities.RemoveRange(orphanedProjectEntities);

            var orphanedAssessmentSectionEntities = new List<AssessmentSectionEntity>();
            foreach (AssessmentSectionEntity assessmentSectionEntity in dbContext.AssessmentSectionEntities
                                                                                 .Where(e => e.AssessmentSectionEntityId > 0))
            {
                if (!assessmentSections.ContainsKey(assessmentSectionEntity))
                {
                    orphanedAssessmentSectionEntities.Add(assessmentSectionEntity);
                }
            }
            dbContext.AssessmentSectionEntities.RemoveRange(orphanedAssessmentSectionEntities);

            var orphanedFailureMechanismEntities = new List<FailureMechanismEntity>();
            foreach (FailureMechanismEntity failureMechanismEntity in dbContext.FailureMechanismEntities
                                                                               .Where(e => e.FailureMechanismEntityId > 0))
            {
                if (!failureMechanisms.ContainsKey(failureMechanismEntity))
                {
                    orphanedFailureMechanismEntities.Add(failureMechanismEntity);
                }
            }
            dbContext.FailureMechanismEntities.RemoveRange(orphanedFailureMechanismEntities);

            var orphanedFailureMechanismSectionEntities = new List<FailureMechanismSectionEntity>();
            foreach (FailureMechanismSectionEntity failureMechanismSectionEntity in dbContext.FailureMechanismSectionEntities
                                                                                             .Where(e => e.FailureMechanismSectionEntityId > 0))
            {
                if (!failureMechanismSections.ContainsKey(failureMechanismSectionEntity))
                {
                    orphanedFailureMechanismSectionEntities.Add(failureMechanismSectionEntity);
                }
            }
            dbContext.FailureMechanismSectionEntities.RemoveRange(orphanedFailureMechanismSectionEntities);

            var orphanedPipingSectionResultEntities = new List<PipingSectionResultEntity>();
            foreach (PipingSectionResultEntity pipingSectionResultEntity in dbContext.PipingSectionResultEntities
                                                                                             .Where(e => e.PipingSectionResultEntityId > 0))
            {
                if (!pipingFailureMechanismSectionResults.ContainsKey(pipingSectionResultEntity))
                {
                    orphanedPipingSectionResultEntities.Add(pipingSectionResultEntity);
                }
            }
            dbContext.PipingSectionResultEntities.RemoveRange(orphanedPipingSectionResultEntities);

            var orphanedGrassCoverErosionInwardsSectionResultEntities = new List<GrassCoverErosionInwardsSectionResultEntity>();
            foreach (GrassCoverErosionInwardsSectionResultEntity sectionResultEntity in dbContext.GrassCoverErosionInwardsSectionResultEntities
                                                                                             .Where(e => e.GrassCoverErosionInwardsSectionResultEntityId > 0))
            {
                if (!grassCoverErosionInwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedGrassCoverErosionInwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.GrassCoverErosionInwardsSectionResultEntities.RemoveRange(orphanedGrassCoverErosionInwardsSectionResultEntities);

            var orphanedHeightStructuresSectionResultEntities = new List<HeightStructuresSectionResultEntity>();
            foreach (HeightStructuresSectionResultEntity sectionResultEntity in dbContext.HeightStructuresSectionResultEntities
                                                                                             .Where(e => e.HeightStructuresSectionResultEntityId > 0))
            {
                if (!heightStructuresFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedHeightStructuresSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.HeightStructuresSectionResultEntities.RemoveRange(orphanedHeightStructuresSectionResultEntities);

            var orphanedStrengthStabilityLengthwiseConstructionSectionResultEntities = new List<StrengthStabilityLengthwiseConstructionSectionResultEntity>();
            foreach (StrengthStabilityLengthwiseConstructionSectionResultEntity sectionResultEntity in dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities
                                                                                             .Where(e => e.StrengthStabilityLengthwiseConstructionSectionResultEntityId > 0))
            {
                if (!strengthStabilityLengthwiseConstructionFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedStrengthStabilityLengthwiseConstructionSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.StrengthStabilityLengthwiseConstructionSectionResultEntities.RemoveRange(orphanedStrengthStabilityLengthwiseConstructionSectionResultEntities);

            var orphanedTechnicalInnovationSectionResultEntities = new List<TechnicalInnovationSectionResultEntity>();
            foreach (TechnicalInnovationSectionResultEntity sectionResultEntity in dbContext.TechnicalInnovationSectionResultEntities
                                                                                             .Where(e => e.TechnicalInnovationSectionResultEntityId > 0))
            {
                if (!technicalInnovationFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedTechnicalInnovationSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.TechnicalInnovationSectionResultEntities.RemoveRange(orphanedTechnicalInnovationSectionResultEntities);

            var orphanedWaterPressureAsphaltCoverSectionResultEntities = new List<WaterPressureAsphaltCoverSectionResultEntity>();
            foreach (WaterPressureAsphaltCoverSectionResultEntity sectionResultEntity in dbContext.WaterPressureAsphaltCoverSectionResultEntities
                                                                                             .Where(e => e.WaterPressureAsphaltCoverSectionResultEntityId > 0))
            {
                if (!waterPressureAsphaltCoverFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedWaterPressureAsphaltCoverSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.WaterPressureAsphaltCoverSectionResultEntities.RemoveRange(orphanedWaterPressureAsphaltCoverSectionResultEntities);

            var orphanedClosingStructureSectionResultEntities = new List<ClosingStructureSectionResultEntity>();
            foreach (ClosingStructureSectionResultEntity sectionResultEntity in dbContext.ClosingStructureSectionResultEntities
                                                                                             .Where(e => e.ClosingStructureSectionResultEntityId > 0))
            {
                if (!closingStructureFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedClosingStructureSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.ClosingStructureSectionResultEntities.RemoveRange(orphanedClosingStructureSectionResultEntities);

            var orphanedMacrostabilityInwardsSectionResultEntities = new List<MacrostabilityInwardsSectionResultEntity>();
            foreach (MacrostabilityInwardsSectionResultEntity sectionResultEntity in dbContext.MacrostabilityInwardsSectionResultEntities
                                                                                             .Where(e => e.MacrostabilityInwardsSectionResultEntityId > 0))
            {
                if (!macrostabilityInwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedMacrostabilityInwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.MacrostabilityInwardsSectionResultEntities.RemoveRange(orphanedMacrostabilityInwardsSectionResultEntities);

            var orphanedMacrostabilityOutwardsSectionResultEntities = new List<MacrostabilityOutwardsSectionResultEntity>();
            foreach (MacrostabilityOutwardsSectionResultEntity sectionResultEntity in dbContext.MacrostabilityOutwardsSectionResultEntities
                                                                                             .Where(e => e.MacrostabilityOutwardsSectionResultEntityId > 0))
            {
                if (!macrostabilityOutwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedMacrostabilityOutwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.MacrostabilityOutwardsSectionResultEntities.RemoveRange(orphanedMacrostabilityOutwardsSectionResultEntities);

            var orphanedWaveImpactAsphaltCoverSectionResultEntities = new List<WaveImpactAsphaltCoverSectionResultEntity>();
            foreach (WaveImpactAsphaltCoverSectionResultEntity sectionResultEntity in dbContext.WaveImpactAsphaltCoverSectionResultEntities
                                                                                             .Where(e => e.WaveImpactAsphaltCoverSectionResultEntityId > 0))
            {
                if (!waveImpactAsphaltCoverFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedWaveImpactAsphaltCoverSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.WaveImpactAsphaltCoverSectionResultEntities.RemoveRange(orphanedWaveImpactAsphaltCoverSectionResultEntities);

            var orphanedGrassCoverErosionOutwardsSectionResultEntities = new List<GrassCoverErosionOutwardsSectionResultEntity>();
            foreach (GrassCoverErosionOutwardsSectionResultEntity sectionResultEntity in dbContext.GrassCoverErosionOutwardsSectionResultEntities
                                                                                             .Where(e => e.GrassCoverErosionOutwardsSectionResultEntityId > 0))
            {
                if (!grassCoverErosionOutwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedGrassCoverErosionOutwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.GrassCoverErosionOutwardsSectionResultEntities.RemoveRange(orphanedGrassCoverErosionOutwardsSectionResultEntities);

            var orphanedGrassCoverSlipOffInwardsSectionResultEntities = new List<GrassCoverSlipOffInwardsSectionResultEntity>();
            foreach (GrassCoverSlipOffInwardsSectionResultEntity sectionResultEntity in dbContext.GrassCoverSlipOffInwardsSectionResultEntities
                                                                                             .Where(e => e.GrassCoverSlipOffInwardsSectionResultEntityId > 0))
            {
                if (!grassCoverSlipOffInwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedGrassCoverSlipOffInwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.GrassCoverSlipOffInwardsSectionResultEntities.RemoveRange(orphanedGrassCoverSlipOffInwardsSectionResultEntities);

            var orphanedGrassCoverSlipOffOutwardsSectionResultEntities = new List<GrassCoverSlipOffOutwardsSectionResultEntity>();
            foreach (GrassCoverSlipOffOutwardsSectionResultEntity sectionResultEntity in dbContext.GrassCoverSlipOffOutwardsSectionResultEntities
                                                                                             .Where(e => e.GrassCoverSlipOffOutwardsSectionResultEntityId > 0))
            {
                if (!grassCoverSlipOffOutwardsFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedGrassCoverSlipOffOutwardsSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.GrassCoverSlipOffOutwardsSectionResultEntities.RemoveRange(orphanedGrassCoverSlipOffOutwardsSectionResultEntities);

            var orphanedMicrostabilitySectionResultEntities = new List<MicrostabilitySectionResultEntity>();
            foreach (MicrostabilitySectionResultEntity sectionResultEntity in dbContext.MicrostabilitySectionResultEntities
                                                                                             .Where(e => e.MicrostabilitySectionResultEntityId > 0))
            {
                if (!microstabilityFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedMicrostabilitySectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.MicrostabilitySectionResultEntities.RemoveRange(orphanedMicrostabilitySectionResultEntities);

            var orphanedPipingStructureSectionResultEntities = new List<PipingStructureSectionResultEntity>();
            foreach (PipingStructureSectionResultEntity sectionResultEntity in dbContext.PipingStructureSectionResultEntities
                                                                                             .Where(e => e.PipingStructureSectionResultEntityId > 0))
            {
                if (!pipingStructureFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedPipingStructureSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.PipingStructureSectionResultEntities.RemoveRange(orphanedPipingStructureSectionResultEntities);

            var orphanedDuneErosionSectionResultEntities = new List<DuneErosionSectionResultEntity>();
            foreach (DuneErosionSectionResultEntity sectionResultEntity in dbContext.DuneErosionSectionResultEntities
                                                                                             .Where(e => e.DuneErosionSectionResultEntityId > 0))
            {
                if (!duneErosionFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedDuneErosionSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.DuneErosionSectionResultEntities.RemoveRange(orphanedDuneErosionSectionResultEntities);

            var orphanedStabilityStoneCoverSectionResultEntities = new List<StabilityStoneCoverSectionResultEntity>();
            foreach (StabilityStoneCoverSectionResultEntity sectionResultEntity in dbContext.StabilityStoneCoverSectionResultEntities
                                                                                             .Where(e => e.StabilityStoneCoverSectionResultEntityId > 0))
            {
                if (!stabilityStoneCoverFailureMechanismSectionResults.ContainsKey(sectionResultEntity))
                {
                    orphanedStabilityStoneCoverSectionResultEntities.Add(sectionResultEntity);
                }
            }
            dbContext.StabilityStoneCoverSectionResultEntities.RemoveRange(orphanedStabilityStoneCoverSectionResultEntities);

            var orphanedHydraulicLocationEntities = new List<HydraulicLocationEntity>();
            foreach (HydraulicLocationEntity hydraulicLocationEntity in dbContext.HydraulicLocationEntities
                                                                                 .Where(e => e.HydraulicLocationEntityId > 0))
            {
                if (!hydraulicLocations.ContainsKey(hydraulicLocationEntity))
                {
                    orphanedHydraulicLocationEntities.Add(hydraulicLocationEntity);
                }
            }
            dbContext.HydraulicLocationEntities.RemoveRange(orphanedHydraulicLocationEntities);

            var orphanedCalculationGroupEntities = new List<CalculationGroupEntity>();
            foreach (CalculationGroupEntity calculationGroupEntity in dbContext.CalculationGroupEntities
                                                                               .Where(e => e.CalculationGroupEntityId > 0))
            {
                if (!calculationGroups.ContainsKey(calculationGroupEntity))
                {
                    orphanedCalculationGroupEntities.Add(calculationGroupEntity);
                }
            }
            dbContext.CalculationGroupEntities.RemoveRange(orphanedCalculationGroupEntities);

            var orphanedPipingCalculationEntities = new List<PipingCalculationEntity>();
            foreach (PipingCalculationEntity pipingCalculationEntity in dbContext.PipingCalculationEntities
                                                                                 .Where(e => e.PipingCalculationEntityId > 0))
            {
                if (!pipingCalculations.ContainsKey(pipingCalculationEntity))
                {
                    orphanedPipingCalculationEntities.Add(pipingCalculationEntity);
                }
            }
            dbContext.PipingCalculationEntities.RemoveRange(orphanedPipingCalculationEntities);

            var orphanedPipingCalculationOutputEntities = new List<PipingCalculationOutputEntity>();
            foreach (PipingCalculationOutputEntity pipingCalculationOutputEntity in dbContext.PipingCalculationOutputEntities
                                                                                             .Where(e => e.PipingCalculationOutputEntityId > 0))
            {
                if (!pipingOutputs.ContainsKey(pipingCalculationOutputEntity))
                {
                    orphanedPipingCalculationOutputEntities.Add(pipingCalculationOutputEntity);
                }
            }
            dbContext.PipingCalculationOutputEntities.RemoveRange(orphanedPipingCalculationOutputEntities);

            var orphanedPipingSemiProbabilisticOutputEntities = new List<PipingSemiProbabilisticOutputEntity>();
            foreach (PipingSemiProbabilisticOutputEntity pipingSemiProbabilisticOutputEntity in dbContext.PipingSemiProbabilisticOutputEntities
                                                                                                         .Where(e => e.PipingSemiProbabilisticOutputEntityId > 0))
            {
                if (!pipingSemiProbabilisticOutputs.ContainsKey(pipingSemiProbabilisticOutputEntity))
                {
                    orphanedPipingSemiProbabilisticOutputEntities.Add(pipingSemiProbabilisticOutputEntity);
                }
            }
            dbContext.PipingSemiProbabilisticOutputEntities.RemoveRange(orphanedPipingSemiProbabilisticOutputEntities);

            var orphanedStochasticSoilModelEntities = new List<StochasticSoilModelEntity>();
            foreach (StochasticSoilModelEntity stochasticSoilModelEntity in dbContext.StochasticSoilModelEntities
                                                                                     .Where(e => e.StochasticSoilModelEntityId > 0))
            {
                if (!stochasticSoilModels.ContainsKey(stochasticSoilModelEntity))
                {
                    orphanedStochasticSoilModelEntities.Add(stochasticSoilModelEntity);
                }
            }
            dbContext.StochasticSoilModelEntities.RemoveRange(orphanedStochasticSoilModelEntities);

            var orphanedStochasticSoilProfileEntities = new List<StochasticSoilProfileEntity>();
            foreach (StochasticSoilProfileEntity stochasticSoilProfileEntity in dbContext.StochasticSoilProfileEntities
                                                                                         .Where(e => e.StochasticSoilProfileEntityId > 0))
            {
                if (!stochasticSoilProfiles.ContainsKey(stochasticSoilProfileEntity))
                {
                    orphanedStochasticSoilProfileEntities.Add(stochasticSoilProfileEntity);
                }
            }
            dbContext.StochasticSoilProfileEntities.RemoveRange(orphanedStochasticSoilProfileEntities);

            var orphanedSoilProfileEntities = new List<SoilProfileEntity>();
            foreach (SoilProfileEntity soilProfileEntity in dbContext.SoilProfileEntities
                                                                     .Where(e => e.SoilProfileEntityId > 0))
            {
                if (!soilProfiles.ContainsKey(soilProfileEntity))
                {
                    orphanedSoilProfileEntities.Add(soilProfileEntity);
                }
            }
            dbContext.SoilProfileEntities.RemoveRange(orphanedSoilProfileEntities);

            var orphanedSoilLayerEntities = new List<SoilLayerEntity>();
            foreach (SoilLayerEntity soilLayerEntity in dbContext.SoilLayerEntities
                                                                 .Where(e => e.SoilLayerEntityId > 0))
            {
                if (!soilLayers.ContainsKey(soilLayerEntity))
                {
                    orphanedSoilLayerEntities.Add(soilLayerEntity);
                }
            }
            dbContext.SoilLayerEntities.RemoveRange(orphanedSoilLayerEntities);

            var orphanedSurfaceLineEntities = new List<SurfaceLineEntity>();
            foreach (SurfaceLineEntity surfaceLineEntity in dbContext.SurfaceLineEntities
                                                                     .Where(e => e.SurfaceLineEntityId > 0))
            {
                if (!surfaceLines.ContainsKey(surfaceLineEntity))
                {
                    orphanedSurfaceLineEntities.Add(surfaceLineEntity);
                }
            }
            dbContext.SurfaceLineEntities.RemoveRange(orphanedSurfaceLineEntities);

            var orphanedSurfaceLinePointEntities = new List<SurfaceLinePointEntity>();
            foreach (SurfaceLinePointEntity surfaceLinePointEntity in dbContext.SurfaceLinePointEntities
                                                                               .Where(e => e.SurfaceLinePointEntityId > 0))
            {
                if (!surfaceLinePoints.ContainsKey(surfaceLinePointEntity))
                {
                    orphanedSurfaceLinePointEntities.Add(surfaceLinePointEntity);
                }
            }
            dbContext.SurfaceLinePointEntities.RemoveRange(orphanedSurfaceLinePointEntities);

            var orphanedCharacteristicPointEntities = new List<CharacteristicPointEntity>();
            foreach (CharacteristicPointEntity characteristicPointEntity in dbContext.CharacteristicPointEntities
                                                                                     .Where(e => e.CharacteristicPointEntityId > 0))
            {
                if (!characteristicPoints.ContainsKey(characteristicPointEntity))
                {
                    orphanedCharacteristicPointEntities.Add(characteristicPointEntity);
                }
            }
            dbContext.CharacteristicPointEntities.RemoveRange(orphanedCharacteristicPointEntities);

            var orphanedPipingFailureMechanismMetaEntities = new List<PipingFailureMechanismMetaEntity>();
            foreach (PipingFailureMechanismMetaEntity pipingFailureMechanismMetaEntity in dbContext.PipingFailureMechanismMetaEntities
                                                                                                   .Where(e => e.PipingFailureMechanismMetaEntityId > 0))
            {
                if (!pipingProbabilityAssessmentInputs.ContainsKey(pipingFailureMechanismMetaEntity))
                {
                    orphanedPipingFailureMechanismMetaEntities.Add(pipingFailureMechanismMetaEntity);
                }
            }
            dbContext.PipingFailureMechanismMetaEntities.RemoveRange(orphanedPipingFailureMechanismMetaEntities);
        }

        private bool ContainsValue<T, U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.ContainsValue(model);
        }

        private void Register<T, U>(Dictionary<T, U> collection, T entity, U model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            collection[entity] = model;
        }

        private T Get<T, U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.Keys.Single(k => ReferenceEquals(collection[k], model));
        }
    }
}