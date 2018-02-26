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
using System.ComponentModel;
using Core.Common.Base;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class AssessmentSection : Observable, IAssessmentSection
    {
        private const double defaultNorm = 1.0 / 30000;
        private const RingtoetsWellKnownTileSource defaultWellKnownTileSource = RingtoetsWellKnownTileSource.BingAerial;

        private readonly IList<HydraulicBoundaryLocationCalculation> designWaterLevelLocationCalculations1 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> designWaterLevelLocationCalculations2 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> designWaterLevelLocationCalculations3 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> designWaterLevelLocationCalculations4 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> waveHeightLocationCalculations1 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> waveHeightLocationCalculations2 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> waveHeightLocationCalculations3 = new List<HydraulicBoundaryLocationCalculation>();
        private readonly IList<HydraulicBoundaryLocationCalculation> waveHeightLocationCalculations4 = new List<HydraulicBoundaryLocationCalculation>();
        private ReferenceLine referenceLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSection"/> class.
        /// </summary>
        /// <param name="composition">The composition of the assessment section, e.g. what
        /// type of elements can be found within the assessment section.</param>
        /// <param name="lowerLimitNorm">The lower limit norm of the assessment section.</param>
        /// <param name="signalingNorm">The signaling norm which of the assessment section.</param>
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

            HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            Piping = new PipingFailureMechanism();
            GrassCoverErosionInwards = new GrassCoverErosionInwardsFailureMechanism();
            MacroStabilityInwards = new MacroStabilityInwardsFailureMechanism();
            MacroStabilityOutwards = new MacroStabilityOutwardsFailureMechanism();
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
            StrengthStabilityLengthwiseConstruction = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            PipingStructure = new PipingStructureFailureMechanism();
            DuneErosion = new DuneErosionFailureMechanism();
            TechnicalInnovation = new TechnicalInnovationFailureMechanism();

            const int otherContribution = 30;
            FailureMechanismContribution = new FailureMechanismContribution(GetContributingFailureMechanisms(),
                                                                            otherContribution,
                                                                            lowerLimitNorm,
                                                                            signalingNorm);
            ChangeComposition(composition);
        }

        /// <summary>
        /// Gets the "Dijken en dammen - Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism Piping { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwards { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public MacroStabilityInwardsFailureMechanism MacroStabilityInwards { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Macrostabiliteit buitenwaarts" failure mechanism.
        /// </summary>
        public MacroStabilityOutwardsFailureMechanism MacroStabilityOutwards { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Microstabiliteit" failure mechanism.
        /// </summary>
        public MicrostabilityFailureMechanism Microstability { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        public StabilityStoneCoverFailureMechanism StabilityStoneCover { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Golfklappen op asfaltbekledingen" failure mechanism.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanism WaveImpactAsphaltCover { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Wateroverdruk bij asfaltbekleding" failure mechanism.
        /// </summary>
        public WaterPressureAsphaltCoverFailureMechanism WaterPressureAsphaltCover { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism GrassCoverErosionOutwards { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards { get; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffInwardsFailureMechanism GrassCoverSlipOffInwards { get; }

        /// <summary>
        /// Gets the "Kunstwerken - Hoogte kunstwerk" failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism HeightStructures { get; }

        /// <summary>
        /// Gets the "Kunstwerken - Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        public ClosingStructuresFailureMechanism ClosingStructures { get; }

        /// <summary>
        /// Gets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        public PipingStructureFailureMechanism PipingStructure { get; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        public StabilityPointStructuresFailureMechanism StabilityPointStructures { get; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit langsconstructies" failure mechanism.
        /// </summary>
        public StrengthStabilityLengthwiseConstructionFailureMechanism StrengthStabilityLengthwiseConstruction { get; }

        /// <summary>
        /// Gets the "Duinwaterkering - Duinafslag" failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism DuneErosion { get; }

        /// <summary>
        /// Gets the "Technische innovaties - Technische innovaties" failure mechanism.
        /// </summary>
        public TechnicalInnovationFailureMechanism TechnicalInnovation { get; }

        /// <summary>
        /// Gets the design water level calculations corresponding to the first norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations1
        {
            get
            {
                return designWaterLevelLocationCalculations1;
            }
        }

        /// <summary>
        /// Gets the design water level calculations corresponding to the second norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations2
        {
            get
            {
                return designWaterLevelLocationCalculations2;
            }
        }

        /// <summary>
        /// Gets the design water level calculations corresponding to the third norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations3
        {
            get
            {
                return designWaterLevelLocationCalculations3;
            }
        }

        /// <summary>
        /// Gets the design water level calculations corresponding to the fourth norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> DesignWaterLevelLocationCalculations4
        {
            get
            {
                return designWaterLevelLocationCalculations4;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the first norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations1
        {
            get
            {
                return waveHeightLocationCalculations1;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the second norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations2
        {
            get
            {
                return waveHeightLocationCalculations2;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the third norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations3
        {
            get
            {
                return waveHeightLocationCalculations3;
            }
        }

        /// <summary>
        /// Gets the wave height calculations corresponding to the fourth norm category boundary.
        /// </summary>
        public IEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightLocationCalculations4
        {
            get
            {
                return waveHeightLocationCalculations4;
            }
        }

        public string Id { get; set; }

        public string Name { get; set; }

        public Comment Comments { get; }

        public AssessmentSectionComposition Composition { get; private set; }

        public ReferenceLine ReferenceLine
        {
            get
            {
                return referenceLine;
            }
            set
            {
                referenceLine = value;
                double sectionLength = value?.Length ?? double.NaN;
                Piping.PipingProbabilityAssessmentInput.SectionLength = sectionLength;
                MacroStabilityInwards.MacroStabilityInwardsProbabilityAssessmentInput.SectionLength = sectionLength;
                MacroStabilityOutwards.MacroStabilityOutwardsProbabilityAssessmentInput.SectionLength = sectionLength;
                WaveImpactAsphaltCover.GeneralWaveImpactAsphaltCoverInput.SectionLength = sectionLength;
            }
        }

        public FailureMechanismContribution FailureMechanismContribution { get; }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; }

        public BackgroundData BackgroundData { get; }

        /// <summary>
        /// Adds hydraulic boundary location calculations for <paramref name="hydraulicBoundaryLocation"/>.
        /// </summary>
        /// <param name="hydraulicBoundaryLocation">The hydraulic boundary location to add calculations for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="hydraulicBoundaryLocation"/> is <c>null</c>.</exception>
        public void AddHydraulicBoundaryLocationCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            if (hydraulicBoundaryLocation == null)
            {
                throw new ArgumentNullException(nameof(hydraulicBoundaryLocation));
            }

            designWaterLevelLocationCalculations1.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            designWaterLevelLocationCalculations2.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            designWaterLevelLocationCalculations3.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            designWaterLevelLocationCalculations4.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightLocationCalculations1.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightLocationCalculations2.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightLocationCalculations3.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightLocationCalculations4.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
        }

        /// <summary>
        /// Clears all currently added hydraulic boundary location calculations.
        /// </summary>
        public void ClearHydraulicBoundaryLocationCalculations()
        {
            designWaterLevelLocationCalculations1.Clear();
            designWaterLevelLocationCalculations2.Clear();
            designWaterLevelLocationCalculations3.Clear();
            designWaterLevelLocationCalculations4.Clear();
            waveHeightLocationCalculations1.Clear();
            waveHeightLocationCalculations2.Clear();
            waveHeightLocationCalculations3.Clear();
            waveHeightLocationCalculations4.Clear();
        }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return Piping;
            yield return GrassCoverErosionInwards;
            yield return MacroStabilityInwards;
            yield return MacroStabilityOutwards;
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
            yield return StrengthStabilityLengthwiseConstruction;
            yield return DuneErosion;
            yield return TechnicalInnovation;
        }

        /// <inheritdoc />
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="newComposition"/> 
        /// is not a valid enum value of <see cref="AssessmentSectionComposition"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="newComposition"/>
        /// is not supported.</exception>
        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            if (!Enum.IsDefined(typeof(AssessmentSectionComposition), newComposition))
            {
                throw new InvalidEnumArgumentException(nameof(newComposition),
                                                       (int) newComposition,
                                                       typeof(AssessmentSectionComposition));
            }

            switch (newComposition)
            {
                case AssessmentSectionComposition.Dike:
                    Piping.Contribution = 24;
                    GrassCoverErosionInwards.Contribution = 24;
                    MacroStabilityInwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 5;
                    WaveImpactAsphaltCover.Contribution = 5;
                    GrassCoverErosionOutwards.Contribution = 5;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StabilityPointStructures.Contribution = 2;
                    DuneErosion.Contribution = 0;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.Dune:
                    Piping.Contribution = 0;
                    GrassCoverErosionInwards.Contribution = 0;
                    MacroStabilityInwards.Contribution = 0;
                    StabilityStoneCover.Contribution = 0;
                    WaveImpactAsphaltCover.Contribution = 0;
                    GrassCoverErosionOutwards.Contribution = 0;
                    HeightStructures.Contribution = 0;
                    ClosingStructures.Contribution = 0;
                    PipingStructure.Contribution = 0;
                    StabilityPointStructures.Contribution = 0;
                    DuneErosion.Contribution = 70;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    Piping.Contribution = 24;
                    GrassCoverErosionInwards.Contribution = 24;
                    MacroStabilityInwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 5;
                    WaveImpactAsphaltCover.Contribution = 5;
                    GrassCoverErosionOutwards.Contribution = 5;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StabilityPointStructures.Contribution = 2;
                    DuneErosion.Contribution = 10;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 20);
                    break;
                default:
                    throw new NotSupportedException($"The enum value {nameof(AssessmentSectionComposition)}.{newComposition} is not supported.");
            }

            Composition = newComposition;
            SetFailureMechanismRelevancy();
        }

        private void SetFailureMechanismRelevancy()
        {
            Piping.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            GrassCoverErosionInwards.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            MacroStabilityInwards.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            StabilityStoneCover.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            WaveImpactAsphaltCover.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            GrassCoverErosionOutwards.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            HeightStructures.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            ClosingStructures.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            StabilityPointStructures.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            PipingStructure.IsRelevant = Composition != AssessmentSectionComposition.Dune;
            DuneErosion.IsRelevant = Composition != AssessmentSectionComposition.Dike;
        }

        private IEnumerable<IFailureMechanism> GetContributingFailureMechanisms()
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
    }
}