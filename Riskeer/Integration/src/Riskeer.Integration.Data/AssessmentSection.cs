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
using System.ComponentModel;
using Core.Common.Base;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.FailurePath;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.Piping.Data;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class AssessmentSection : Observable, IAssessmentSection
    {
        private const double defaultNorm = 1.0 / 30000;
        private const RiskeerWellKnownTileSource defaultWellKnownTileSource = RiskeerWellKnownTileSource.BingAerial;

        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSection"/> class.
        /// </summary>
        /// <param name="composition">The composition of the assessment section, e.g. what
        /// type of elements can be found within the assessment section.</param>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm of the assessment section.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="composition"/> 
        /// is not a valid enum value of <see cref="AssessmentSectionComposition"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="composition"/>
        /// is not supported.</exception>
        public AssessmentSection(AssessmentSectionComposition composition,
                                 double lowerLimitNorm = defaultNorm,
                                 double signalingNorm = defaultNorm)
        {
            Name = Resources.AssessmentSection_DisplayName;
            Comments = new Comment();

            BackgroundData = new BackgroundData(new WellKnownBackgroundDataConfiguration(defaultWellKnownTileSource))
            {
                Name = defaultWellKnownTileSource.GetDisplayName()
            };

            ReferenceLine = new ReferenceLine();

            HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            SpecificFailurePaths = new ObservableList<SpecificFailurePath>();
            WaterLevelCalculationsForUserDefinedTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>();
            WaveHeightCalculationsForUserDefinedTargetProbabilities = new ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability>();

            Piping = new PipingFailureMechanism();
            GrassCoverErosionInwards = new GrassCoverErosionInwardsFailureMechanism();
            MacroStabilityInwards = new MacroStabilityInwardsFailureMechanism();
            Microstability = new MicrostabilityFailureMechanism();
            StabilityStoneCover = new StabilityStoneCoverFailureMechanism();
            WaveImpactAsphaltCover = new WaveImpactAsphaltCoverFailureMechanism();
            WaterPressureAsphaltCover = new WaterPressureAsphaltCoverFailureMechanism();
            GrassCoverErosionOutwards = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverSlipOffOutwards = new GrassCoverSlipOffOutwardsFailureMechanism();
            GrassCoverSlipOffInwards = new GrassCoverSlipOffInwardsFailureMechanism();
            HeightStructures = new HeightStructuresFailureMechanism();
            ClosingStructures = new ClosingStructuresFailureMechanism();
            StabilityPointStructures = new StabilityPointStructuresFailureMechanism();
            PipingStructure = new PipingStructureFailureMechanism();
            DuneErosion = new DuneErosionFailureMechanism();

            FailureMechanismContribution = new FailureMechanismContribution(lowerLimitNorm, signalingNorm);
            ChangeComposition(composition);
        }

        /// <summary>
        /// Gets or sets the "Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism Piping { get; set; }

        /// <summary>
        /// Gets or sets the "Grasbekleding erosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwards { get; set; }

        /// <summary>
        /// Gets or sets the "Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism MacroStabilityInwards { get; set; }

        /// <summary>
        /// Gets or sets the "Microstabiliteit" failure mechanism.
        /// </summary>
        public MicrostabilityFailureMechanism Microstability { get; set; }

        /// <summary>
        /// Gets or sets the "Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        public StabilityStoneCoverFailureMechanism StabilityStoneCover { get; set; }

        /// <summary>
        /// Gets or sets the "Golfklappen op asfaltbekleding" failure mechanism.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanism WaveImpactAsphaltCover { get; set; }

        /// <summary>
        /// Gets or sets the "Wateroverdruk bij asfaltbekleding" failure mechanism.
        /// </summary>
        public WaterPressureAsphaltCoverFailureMechanism WaterPressureAsphaltCover { get; set; }

        /// <summary>
        /// Gets or sets the "Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism GrassCoverErosionOutwards { get; set; }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards { get; set; }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding afschuiven binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffInwardsFailureMechanism GrassCoverSlipOffInwards { get; set; }

        /// <summary>
        /// Gets or sets the "Hoogte kunstwerk" failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism HeightStructures { get; set; }

        /// <summary>
        /// Gets or sets the "Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        public ClosingStructuresFailureMechanism ClosingStructures { get; set; }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        public PipingStructureFailureMechanism PipingStructure { get; set; }

        /// <summary>
        /// Gets or sets the "Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        public StabilityPointStructuresFailureMechanism StabilityPointStructures { get; set; }

        /// <summary>
        /// Gets or sets the "Duinafslag" failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism DuneErosion { get; set; }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalingNorm => waterLevelCalculationsForSignalingNorm;

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForLowerLimitNorm => waterLevelCalculationsForLowerLimitNorm;

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaterLevelCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaveHeightCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<SpecificFailurePath> SpecificFailurePaths { get; }

        public string Id { get; set; }

        public string Name { get; set; }

        public Comment Comments { get; }

        public AssessmentSectionComposition Composition { get; private set; }

        public ReferenceLine ReferenceLine { get; }

        public FailureMechanismContribution FailureMechanismContribution { get; }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }

        public BackgroundData BackgroundData { get; }

        /// <summary>
        /// Sets hydraulic boundary location calculations for <paramref name="hydraulicBoundaryLocations"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocations">The hydraulic boundary locations to add calculations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocations"/> is <c>null</c>.</exception>
        public void SetHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            if (hydraulicBoundaryLocations == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocations));
            }

            ClearHydraulicBoundaryLocationCalculations();

            foreach (HydraulicBoundaryLocation hydraulicBoundaryLocation in hydraulicBoundaryLocations)
            {
                AddHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocation);
            }
        }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return Piping;
            yield return GrassCoverErosionInwards;
            yield return MacroStabilityInwards;
            yield return Microstability;
            yield return StabilityStoneCover;
            yield return WaveImpactAsphaltCover;
            yield return WaterPressureAsphaltCover;
            yield return GrassCoverErosionOutwards;
            yield return GrassCoverSlipOffOutwards;
            yield return GrassCoverSlipOffInwards;
            yield return HeightStructures;
            yield return ClosingStructures;
            yield return PipingStructure;
            yield return StabilityPointStructures;
            yield return DuneErosion;
        }

        public IEnumerable<IFailureMechanism> GetContributingFailureMechanisms()
        {
            yield return Piping;
            yield return GrassCoverErosionInwards;
            yield return MacroStabilityInwards;
            yield return StabilityStoneCover;
            yield return WaveImpactAsphaltCover;
            yield return GrassCoverErosionOutwards;
            yield return HeightStructures;
            yield return ClosingStructures;
            yield return PipingStructure;
            yield return StabilityPointStructures;
            yield return DuneErosion;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="newComposition"/> 
        /// is not a valid enum value of <see cref="AssessmentSectionComposition"/>.</exception>
        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionComposition), newComposition))
            {
                throw new InvalidEnumArgumentException(nameof(newComposition),
                                                       (int) newComposition,
                                                       typeof(AssessmentSectionComposition));
            }

            Composition = newComposition;
            SetFailureMechanismsToBeInAssembly();
        }

        private void ClearHydraulicBoundaryLocationCalculations()
        {
            waterLevelCalculationsForSignalingNorm.Clear();
            waterLevelCalculationsForLowerLimitNorm.Clear();

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Clear();
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Clear();
            }
        }

        private void AddHydraulicBoundaryLocationCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            waterLevelCalculationsForSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                element.HydraulicBoundaryLocationCalculations.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            }
        }

        private void SetFailureMechanismsToBeInAssembly()
        {
            Piping.InAssembly = Composition != AssessmentSectionComposition.Dune;
            GrassCoverErosionInwards.InAssembly = Composition != AssessmentSectionComposition.Dune;
            MacroStabilityInwards.InAssembly = Composition != AssessmentSectionComposition.Dune;
            StabilityStoneCover.InAssembly = Composition != AssessmentSectionComposition.Dune;
            WaveImpactAsphaltCover.InAssembly = Composition != AssessmentSectionComposition.Dune;
            GrassCoverErosionOutwards.InAssembly = Composition != AssessmentSectionComposition.Dune;
            HeightStructures.InAssembly = Composition != AssessmentSectionComposition.Dune;
            ClosingStructures.InAssembly = Composition != AssessmentSectionComposition.Dune;
            StabilityPointStructures.InAssembly = Composition != AssessmentSectionComposition.Dune;
            PipingStructure.InAssembly = Composition != AssessmentSectionComposition.Dune;
            DuneErosion.InAssembly = Composition != AssessmentSectionComposition.Dike;
        }
    }
}