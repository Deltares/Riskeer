// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Linq;
using Core.Common.Base;
using Core.Common.Util.Extensions;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
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
        private const double defaultFloodingProbability = 1.0 / 30000;
        private const RiskeerWellKnownTileSource defaultWellKnownTileSource = RiskeerWellKnownTileSource.BingAerial;

        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalFloodingProbability;
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForMaximumAllowableFloodingProbability;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSection"/> class.
        /// </summary>
        /// <param name="composition">The composition of the assessment section, e.g. what
        /// type of elements can be found within the assessment section.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability of the assessment section.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability of the assessment section.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="maximumAllowableFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalFloodingProbability"/> is larger than <paramref name="maximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="composition"/> 
        /// is not a valid enum value of <see cref="AssessmentSectionComposition"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="composition"/>
        /// is not supported.</exception>
        public AssessmentSection(AssessmentSectionComposition composition,
                                 double maximumAllowableFloodingProbability = defaultFloodingProbability,
                                 double signalFloodingProbability = defaultFloodingProbability)
        {
            Name = Resources.AssessmentSection_DisplayName;
            Comments = new Comment();

            BackgroundData = new BackgroundData(new WellKnownBackgroundDataConfiguration(defaultWellKnownTileSource))
            {
                Name = defaultWellKnownTileSource.GetDisplayName()
            };

            ReferenceLine = new ReferenceLine();

            HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            SpecificFailureMechanisms = new ObservableList<SpecificFailureMechanism>();
            waterLevelCalculationsForSignalFloodingProbability = new ObservableList<HydraulicBoundaryLocationCalculation>();
            waterLevelCalculationsForMaximumAllowableFloodingProbability = new ObservableList<HydraulicBoundaryLocationCalculation>();
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

            FailureMechanismContribution = new FailureMechanismContribution(maximumAllowableFloodingProbability, signalFloodingProbability);
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
        /// Gets or sets the "Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards { get; set; }

        /// <summary>
        /// Gets or sets the "Grasbekleding afschuiven binnentalud" failure mechanism.
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
        /// Gets or sets the "Piping bij kunstwerk" failure mechanism.
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

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalFloodingProbability => waterLevelCalculationsForSignalFloodingProbability;

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForMaximumAllowableFloodingProbability => waterLevelCalculationsForMaximumAllowableFloodingProbability;

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaterLevelCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<HydraulicBoundaryLocationCalculationsForTargetProbability> WaveHeightCalculationsForUserDefinedTargetProbabilities { get; }

        public ObservableList<SpecificFailureMechanism> SpecificFailureMechanisms { get; }

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

            RemoveOutdatedHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            AddMissingHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            SortHydraulicBoundaryLocationCalculations();
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

        private void RemoveOutdatedHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> allHydraulicBoundaryLocations)
        {
            HydraulicBoundaryLocation[] outdatedHydraulicBoundaryLocations = waterLevelCalculationsForSignalFloodingProbability
                                                                             .Select(hblc => hblc.HydraulicBoundaryLocation)
                                                                             .Except(allHydraulicBoundaryLocations)
                                                                             .ToArray();

            RemoveOutdatedHydraulicBoundaryLocationCalculations(waterLevelCalculationsForSignalFloodingProbability, outdatedHydraulicBoundaryLocations);
            RemoveOutdatedHydraulicBoundaryLocationCalculations(waterLevelCalculationsForMaximumAllowableFloodingProbability, outdatedHydraulicBoundaryLocations);

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                RemoveOutdatedHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations, outdatedHydraulicBoundaryLocations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                RemoveOutdatedHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations, outdatedHydraulicBoundaryLocations);
            }
        }

        private static void RemoveOutdatedHydraulicBoundaryLocationCalculations(ICollection<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations,
                                                                                HydraulicBoundaryLocation[] outdatedHydraulicBoundaryLocations)
        {
            hydraulicBoundaryLocationCalculations.RemoveAllWhere(hblc => outdatedHydraulicBoundaryLocations.Contains(hblc.HydraulicBoundaryLocation));
        }

        private void AddMissingHydraulicBoundaryLocationCalculations(IEnumerable<HydraulicBoundaryLocation> allHydraulicBoundaryLocations)
        {
            HydraulicBoundaryLocation[] missingHydraulicBoundaryLocations = allHydraulicBoundaryLocations
                                                                            .Except(waterLevelCalculationsForSignalFloodingProbability
                                                                                        .Select(hblc => hblc.HydraulicBoundaryLocation))
                                                                            .ToArray();

            AddMissingHydraulicBoundaryLocationCalculations(waterLevelCalculationsForSignalFloodingProbability, missingHydraulicBoundaryLocations);
            AddMissingHydraulicBoundaryLocationCalculations(waterLevelCalculationsForMaximumAllowableFloodingProbability, missingHydraulicBoundaryLocations);

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                AddMissingHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations, missingHydraulicBoundaryLocations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                AddMissingHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations, missingHydraulicBoundaryLocations);
            }
        }

        private static void AddMissingHydraulicBoundaryLocationCalculations(List<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations,
                                                                            IEnumerable<HydraulicBoundaryLocation> missingHydraulicBoundaryLocations)
        {
            hydraulicBoundaryLocationCalculations.AddRange(missingHydraulicBoundaryLocations.Select(mhbl => new HydraulicBoundaryLocationCalculation(mhbl)));
        }

        private void SortHydraulicBoundaryLocationCalculations()
        {
            SortHydraulicBoundaryLocationCalculations(waterLevelCalculationsForSignalFloodingProbability);
            SortHydraulicBoundaryLocationCalculations(waterLevelCalculationsForMaximumAllowableFloodingProbability);

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaterLevelCalculationsForUserDefinedTargetProbabilities)
            {
                SortHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations);
            }

            foreach (HydraulicBoundaryLocationCalculationsForTargetProbability element in WaveHeightCalculationsForUserDefinedTargetProbabilities)
            {
                SortHydraulicBoundaryLocationCalculations(element.HydraulicBoundaryLocationCalculations);
            }
        }

        private static void SortHydraulicBoundaryLocationCalculations(List<HydraulicBoundaryLocationCalculation> hydraulicBoundaryLocationCalculations)
        {
            hydraulicBoundaryLocationCalculations.Sort((x, y) => x.HydraulicBoundaryLocation.Id.CompareTo(y.HydraulicBoundaryLocation.Id));
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