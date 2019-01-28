// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone;

namespace Riskeer.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class AssessmentSection : Observable, IAssessmentSection
    {
        private const double defaultNorm = 1.0 / 30000;
        private const RingtoetsWellKnownTileSource defaultWellKnownTileSource = RingtoetsWellKnownTileSource.BingAerial;

        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waterLevelCalculationsForFactorizedLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForSignalingNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();
        private readonly ObservableList<HydraulicBoundaryLocationCalculation> waveHeightCalculationsForFactorizedLowerLimitNorm = new ObservableList<HydraulicBoundaryLocationCalculation>();

        private PipingFailureMechanism piping;
        private GrassCoverErosionInwardsFailureMechanism grassCoverErosionInwards;
        private MacroStabilityInwardsFailureMechanism macroStabilityInwards;
        private MacroStabilityOutwardsFailureMechanism macroStabilityOutwards;
        private MicrostabilityFailureMechanism microstability;
        private StabilityStoneCoverFailureMechanism stabilityStoneCover;
        private WaveImpactAsphaltCoverFailureMechanism waveImpactAsphaltCover;
        private WaterPressureAsphaltCoverFailureMechanism waterPressureAsphaltCover;
        private GrassCoverErosionOutwardsFailureMechanism grassCoverErosionOutwards;
        private GrassCoverSlipOffOutwardsFailureMechanism grassCoverSlipOffOutwards;
        private GrassCoverSlipOffInwardsFailureMechanism grassCoverSlipOffInwards;
        private HeightStructuresFailureMechanism heightStructures;
        private ClosingStructuresFailureMechanism closingStructures;
        private PipingStructureFailureMechanism pipingStructure;
        private StabilityPointStructuresFailureMechanism stabilityPointStructures;
        private StrengthStabilityLengthwiseConstructionFailureMechanism strengthStabilityLengthwiseConstruction;
        private DuneErosionFailureMechanism duneErosion;
        private TechnicalInnovationFailureMechanism technicalInnovation;
        private RoundedDouble failureProbabilityMarginFactor;

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

            ReferenceLine = new ReferenceLine();
            HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            piping = new PipingFailureMechanism();
            grassCoverErosionInwards = new GrassCoverErosionInwardsFailureMechanism();
            macroStabilityInwards = new MacroStabilityInwardsFailureMechanism();
            macroStabilityOutwards = new MacroStabilityOutwardsFailureMechanism();
            microstability = new MicrostabilityFailureMechanism();
            stabilityStoneCover = new StabilityStoneCoverFailureMechanism();
            waveImpactAsphaltCover = new WaveImpactAsphaltCoverFailureMechanism();
            waterPressureAsphaltCover = new WaterPressureAsphaltCoverFailureMechanism();
            grassCoverErosionOutwards = new GrassCoverErosionOutwardsFailureMechanism();
            grassCoverSlipOffOutwards = new GrassCoverSlipOffOutwardsFailureMechanism();
            grassCoverSlipOffInwards = new GrassCoverSlipOffInwardsFailureMechanism();
            heightStructures = new HeightStructuresFailureMechanism();
            closingStructures = new ClosingStructuresFailureMechanism();
            stabilityPointStructures = new StabilityPointStructuresFailureMechanism();
            strengthStabilityLengthwiseConstruction = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            pipingStructure = new PipingStructureFailureMechanism();
            duneErosion = new DuneErosionFailureMechanism();
            technicalInnovation = new TechnicalInnovationFailureMechanism();
            OtherFailureMechanism = new OtherFailureMechanism();

            failureProbabilityMarginFactor = new RoundedDouble(2);

            FailureMechanismContribution = new FailureMechanismContribution(lowerLimitNorm, signalingNorm);
            ChangeComposition(composition);
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Piping" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public PipingFailureMechanism Piping
        {
            get
            {
                return piping;
            }
            set
            {
                ValidateContribution(piping, value);
                piping = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding erosie kruin en binnentalud" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwards
        {
            get
            {
                return grassCoverErosionInwards;
            }
            set
            {
                ValidateContribution(grassCoverErosionInwards, value);
                grassCoverErosionInwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public MacroStabilityInwardsFailureMechanism MacroStabilityInwards
        {
            get
            {
                return macroStabilityInwards;
            }
            set
            {
                ValidateContribution(macroStabilityInwards, value);
                macroStabilityInwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Macrostabiliteit buitenwaarts" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public MacroStabilityOutwardsFailureMechanism MacroStabilityOutwards
        {
            get
            {
                return macroStabilityOutwards;
            }
            set
            {
                ValidateContribution(macroStabilityOutwards, value);
                macroStabilityOutwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Microstabiliteit" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public MicrostabilityFailureMechanism Microstability
        {
            get
            {
                return microstability;
            }
            set
            {
                ValidateContribution(microstability, value);
                microstability = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public StabilityStoneCoverFailureMechanism StabilityStoneCover
        {
            get
            {
                return stabilityStoneCover;
            }
            set
            {
                ValidateContribution(stabilityStoneCover, value);
                stabilityStoneCover = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Golfklappen op asfaltbekledingen" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public WaveImpactAsphaltCoverFailureMechanism WaveImpactAsphaltCover
        {
            get
            {
                return waveImpactAsphaltCover;
            }
            set
            {
                ValidateContribution(waveImpactAsphaltCover, value);
                waveImpactAsphaltCover = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Wateroverdruk bij asfaltbekleding" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public WaterPressureAsphaltCoverFailureMechanism WaterPressureAsphaltCover
        {
            get
            {
                return waterPressureAsphaltCover;
            }
            set
            {
                ValidateContribution(waterPressureAsphaltCover, value);
                waterPressureAsphaltCover = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public GrassCoverErosionOutwardsFailureMechanism GrassCoverErosionOutwards
        {
            get
            {
                return grassCoverErosionOutwards;
            }
            set
            {
                ValidateContribution(grassCoverErosionOutwards, value);
                grassCoverErosionOutwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards
        {
            get
            {
                return grassCoverSlipOffOutwards;
            }
            set
            {
                ValidateContribution(grassCoverSlipOffOutwards, value);
                grassCoverSlipOffOutwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Dijken en dammen - Grasbekleding afschuiven binnentalud" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public GrassCoverSlipOffInwardsFailureMechanism GrassCoverSlipOffInwards
        {
            get
            {
                return grassCoverSlipOffInwards;
            }
            set
            {
                ValidateContribution(grassCoverSlipOffInwards, value);
                grassCoverSlipOffInwards = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Hoogte kunstwerk" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public HeightStructuresFailureMechanism HeightStructures
        {
            get
            {
                return heightStructures;
            }
            set
            {
                ValidateContribution(heightStructures, value);
                heightStructures = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public ClosingStructuresFailureMechanism ClosingStructures
        {
            get
            {
                return closingStructures;
            }
            set
            {
                ValidateContribution(closingStructures, value);
                closingStructures = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public PipingStructureFailureMechanism PipingStructure
        {
            get
            {
                return pipingStructure;
            }
            set
            {
                ValidateContribution(pipingStructure, value);
                pipingStructure = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public StabilityPointStructuresFailureMechanism StabilityPointStructures
        {
            get
            {
                return stabilityPointStructures;
            }
            set
            {
                ValidateContribution(stabilityPointStructures, value);
                stabilityPointStructures = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Kunstwerken - Sterkte en stabiliteit langsconstructies" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public StrengthStabilityLengthwiseConstructionFailureMechanism StrengthStabilityLengthwiseConstruction
        {
            get
            {
                return strengthStabilityLengthwiseConstruction;
            }
            set
            {
                ValidateContribution(strengthStabilityLengthwiseConstruction, value);
                strengthStabilityLengthwiseConstruction = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Duinwaterkering - Duinafslag" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public DuneErosionFailureMechanism DuneErosion
        {
            get
            {
                return duneErosion;
            }
            set
            {
                ValidateContribution(duneErosion, value);
                duneErosion = value;
            }
        }

        /// <summary>
        /// Gets or sets the "Technische innovaties - Technische innovaties" failure mechanism.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="value"/>
        /// is not equal to the contribution of the current failure mechanism.</exception>
        public TechnicalInnovationFailureMechanism TechnicalInnovation
        {
            get
            {
                return technicalInnovation;
            }
            set
            {
                ValidateContribution(technicalInnovation, value);
                technicalInnovation = value;
            }
        }

        /// <summary>
        /// Gets the "Overige" category failure mechanism.
        /// </summary>
        public OtherFailureMechanism OtherFailureMechanism { get; }

        /// <summary>
        /// Gets the failure probability margin factor.
        /// </summary>
        public RoundedDouble FailureProbabilityMarginFactor
        {
            get
            {
                return failureProbabilityMarginFactor;
            }
            private set
            {
                failureProbabilityMarginFactor = value.ToPrecision(failureProbabilityMarginFactor.NumberOfDecimalPlaces);
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForFactorizedSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForSignalingNorm
        {
            get
            {
                return waterLevelCalculationsForSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForLowerLimitNorm
        {
            get
            {
                return waterLevelCalculationsForLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaterLevelCalculationsForFactorizedLowerLimitNorm
        {
            get
            {
                return waterLevelCalculationsForFactorizedLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForFactorizedSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForSignalingNorm
        {
            get
            {
                return waveHeightCalculationsForSignalingNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForLowerLimitNorm
        {
            get
            {
                return waveHeightCalculationsForLowerLimitNorm;
            }
        }

        public IObservableEnumerable<HydraulicBoundaryLocationCalculation> WaveHeightCalculationsForFactorizedLowerLimitNorm
        {
            get
            {
                return waveHeightCalculationsForFactorizedLowerLimitNorm;
            }
        }

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
            yield return OtherFailureMechanism;
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
                    MacroStabilityOutwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 5;
                    WaveImpactAsphaltCover.Contribution = 5;
                    GrassCoverErosionOutwards.Contribution = 5;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StabilityPointStructures.Contribution = 2;
                    DuneErosion.Contribution = 0;
                    OtherFailureMechanism.Contribution = 30;
                    FailureProbabilityMarginFactor = (RoundedDouble) 0.58;
                    break;
                case AssessmentSectionComposition.Dune:
                    Piping.Contribution = 0;
                    GrassCoverErosionInwards.Contribution = 0;
                    MacroStabilityInwards.Contribution = 0;
                    MacroStabilityOutwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 0;
                    WaveImpactAsphaltCover.Contribution = 0;
                    GrassCoverErosionOutwards.Contribution = 0;
                    HeightStructures.Contribution = 0;
                    ClosingStructures.Contribution = 0;
                    PipingStructure.Contribution = 0;
                    StabilityPointStructures.Contribution = 0;
                    DuneErosion.Contribution = 70;
                    OtherFailureMechanism.Contribution = 30;
                    FailureProbabilityMarginFactor = (RoundedDouble) 0;
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    Piping.Contribution = 24;
                    GrassCoverErosionInwards.Contribution = 24;
                    MacroStabilityInwards.Contribution = 4;
                    MacroStabilityOutwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 5;
                    WaveImpactAsphaltCover.Contribution = 5;
                    GrassCoverErosionOutwards.Contribution = 5;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StabilityPointStructures.Contribution = 2;
                    DuneErosion.Contribution = 10;
                    OtherFailureMechanism.Contribution = 20;
                    FailureProbabilityMarginFactor = (RoundedDouble) 0.58;
                    break;
                default:
                    throw new NotSupportedException();
            }

            Composition = newComposition;
            SetFailureMechanismRelevancy();
        }

        /// <summary>
        /// Validates whether the contribution of <paramref name="newFailureMechanism"/>
        /// is equal to the contribution of <paramref name="oldFailureMechanism"/>.
        /// </summary>
        /// <param name="oldFailureMechanism">The old failure mechanism value.</param>
        /// <param name="newFailureMechanism">The new failure mechanism value.</param>
        /// <exception cref="ArgumentException">Thrown when the contribution of <paramref name="newFailureMechanism"/>
        /// is not equal to the contribution of <paramref name="oldFailureMechanism"/>.</exception>
        private static void ValidateContribution(IFailureMechanism oldFailureMechanism, IFailureMechanism newFailureMechanism)
        {
            if (Math.Abs(oldFailureMechanism.Contribution - newFailureMechanism.Contribution) >= double.Epsilon)
            {
                throw new ArgumentException(Resources.AssessmentSection_ValidateContribution_Contribution_new_FailureMechanism_must_be_equal_to_old_FailureMechanism);
            }
        }

        private void ClearHydraulicBoundaryLocationCalculations()
        {
            waterLevelCalculationsForFactorizedSignalingNorm.Clear();
            waterLevelCalculationsForSignalingNorm.Clear();
            waterLevelCalculationsForLowerLimitNorm.Clear();
            waterLevelCalculationsForFactorizedLowerLimitNorm.Clear();
            waveHeightCalculationsForFactorizedSignalingNorm.Clear();
            waveHeightCalculationsForSignalingNorm.Clear();
            waveHeightCalculationsForLowerLimitNorm.Clear();
            waveHeightCalculationsForFactorizedLowerLimitNorm.Clear();
        }

        private void AddHydraulicBoundaryLocationCalculations(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            waterLevelCalculationsForFactorizedSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waterLevelCalculationsForFactorizedLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForFactorizedSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForSignalingNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
            waveHeightCalculationsForFactorizedLowerLimitNorm.Add(new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation));
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
    }
}