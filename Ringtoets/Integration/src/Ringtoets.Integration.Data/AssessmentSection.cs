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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class AssessmentSection : Observable, IAssessmentSection
    {
        private ReferenceLine referenceLine;

        /// <summary>
        /// Initializes a new instance of the <see cref="AssessmentSection"/> class.
        /// </summary>
        /// <param name="composition">The composition of the assessment section, e.g. what
        /// type of elements can be found within the assessment section.</param>
        public AssessmentSection(AssessmentSectionComposition composition)
        {
            Name = Resources.AssessmentSection_DisplayName;

            PipingFailureMechanism = new PipingFailureMechanism();
            GrassCoverErosionInwards = new GrassCoverErosionInwardsFailureMechanism();

            MacrostabilityInwards = new MacrostabilityInwardsFailureMechanism();
            MacrostabilityOutwards = new MacrostabilityOutwardsFailureMechanism();
            Microstability = new MicrostabilityFailureMechanism();
            StabilityStoneCover = new StabilityStoneCoverFailureMechanism();
            WaveImpactAsphaltCover = new WaveImpactAsphaltCoverFailureMechanism();
            WaterPressureAsphaltCover = new WaterPressureAsphaltCoverFailureMechanism();
            GrassCoverErosionOutwards = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverSlipOffOutwards = new GrassCoverSlipOffOutwardsFailureMechanism();
            GrassCoverSlipOffInwards = new GrassCoverSlipOffInwardsFailureMechanism();
            HeightStructures = new HeightStructuresFailureMechanism();
            ClosingStructures = new ClosingStructuresFailureMechanism();
            StrengthStabilityPointConstruction = new StrengthStabilityPointConstructionFailureMechanism();
            StrengthStabilityLengthwiseConstruction = new StrengthStabilityLengthwiseConstructionFailureMechanism();
            PipingStructure = new PipingStructureFailureMechanism();
            DuneErosion = new DuneErosionFailureMechanism();
            TechnicalInnovation = new TechnicalInnovationFailureMechanism();

            FailureMechanismContribution = new FailureMechanismContribution(GetContributingFailureMechanisms(), 30, 30000);
            ChangeComposition(composition);
        }

        /// <summary>
        /// Gets the "Dijken en dammen - Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism GrassCoverErosionInwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public MacrostabilityInwardsFailureMechanism MacrostabilityInwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Macrostabiliteit buitenwaarts" failure mechanism.
        /// </summary>
        public MacrostabilityOutwardsFailureMechanism MacrostabilityOutwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Microstabiliteit" failure mechanism.
        /// </summary>
        public MicrostabilityFailureMechanism Microstability { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        public StabilityStoneCoverFailureMechanism StabilityStoneCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Golfklappen op asfaltbekledingen" failure mechanism.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanism WaveImpactAsphaltCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Wateroverdruk bij asfaltbekleding" failure mechanism.
        /// </summary>
        public WaterPressureAsphaltCoverFailureMechanism WaterPressureAsphaltCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism GrassCoverErosionOutwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven binnentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffInwardsFailureMechanism GrassCoverSlipOffInwards { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Hoogte kunstwerk" failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism HeightStructures { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        public ClosingStructuresFailureMechanism ClosingStructures { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        public PipingStructureFailureMechanism PipingStructure { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        public StrengthStabilityPointConstructionFailureMechanism StrengthStabilityPointConstruction { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit langsconstructies" failure mechanism.
        /// </summary>
        public StrengthStabilityLengthwiseConstructionFailureMechanism StrengthStabilityLengthwiseConstruction { get; private set; }

        /// <summary>
        /// Gets the "Duinwaterkering - Duinafslag" failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism DuneErosion { get; private set; }

        /// <summary>
        /// Gets the "Technische innovaties - Technische innovaties" failure mechanism.
        /// </summary>
        public TechnicalInnovationFailureMechanism TechnicalInnovation { get; private set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public string Comments { get; set; }

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
                PipingFailureMechanism.PipingProbabilityAssessmentInput.SectionLength = value == null ? double.NaN : Math2D.Length(value.Points);
            }
        }

        public FailureMechanismContribution FailureMechanismContribution { get; private set; }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return PipingFailureMechanism;
            yield return GrassCoverErosionInwards;
            yield return MacrostabilityInwards;
            yield return MacrostabilityOutwards;
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
            yield return StrengthStabilityPointConstruction;
            yield return StrengthStabilityLengthwiseConstruction;
            yield return DuneErosion;
            yield return TechnicalInnovation;
        }

        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            switch (newComposition)
            {
                case AssessmentSectionComposition.Dike:
                    PipingFailureMechanism.Contribution = 24;
                    GrassCoverErosionInwards.Contribution = 24;
                    MacrostabilityInwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 3;
                    WaveImpactAsphaltCover.Contribution = 1;
                    GrassCoverErosionOutwards.Contribution = 5;
                    GrassCoverSlipOffOutwards.Contribution = 1;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StrengthStabilityPointConstruction.Contribution = 2;
                    DuneErosion.Contribution = 0;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.Dune:
                    PipingFailureMechanism.Contribution = 0;
                    GrassCoverErosionInwards.Contribution = 0;
                    MacrostabilityInwards.Contribution = 0;
                    StabilityStoneCover.Contribution = 0;
                    WaveImpactAsphaltCover.Contribution = 0;
                    GrassCoverErosionOutwards.Contribution = 0;
                    GrassCoverSlipOffOutwards.Contribution = 0;
                    HeightStructures.Contribution = 0;
                    ClosingStructures.Contribution = 0;
                    PipingStructure.Contribution = 0;
                    StrengthStabilityPointConstruction.Contribution = 0;
                    DuneErosion.Contribution = 70;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    PipingFailureMechanism.Contribution = 24;
                    GrassCoverErosionInwards.Contribution = 24;
                    MacrostabilityInwards.Contribution = 4;
                    StabilityStoneCover.Contribution = 3;
                    WaveImpactAsphaltCover.Contribution = 1;
                    GrassCoverErosionOutwards.Contribution = 5;
                    GrassCoverSlipOffOutwards.Contribution = 1;
                    HeightStructures.Contribution = 24;
                    ClosingStructures.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StrengthStabilityPointConstruction.Contribution = 2;
                    DuneErosion.Contribution = 10;
                    FailureMechanismContribution.UpdateContributions(GetContributingFailureMechanisms(), 20);
                    break;
                default:
                    throw new NotImplementedException();
            }
            Composition = newComposition;
        }

        private IEnumerable<IFailureMechanism> GetContributingFailureMechanisms()
        {
            yield return PipingFailureMechanism;
            yield return GrassCoverErosionInwards;
            yield return MacrostabilityInwards;
            yield return StabilityStoneCover;
            yield return WaveImpactAsphaltCover;
            yield return GrassCoverErosionOutwards;
            yield return GrassCoverSlipOffOutwards;
            yield return HeightStructures;
            yield return ClosingStructures;
            yield return PipingStructure;
            yield return StrengthStabilityPointConstruction;
            yield return DuneErosion;
        }
    }
}