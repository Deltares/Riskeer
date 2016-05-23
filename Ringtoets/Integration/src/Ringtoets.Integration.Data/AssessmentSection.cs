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
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class AssessmentSection : Observable, IAssessmentSection
    {
        private ReferenceLine referenceLine;

        private FailureMechanismContribution contritbution;

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

            MacrostabilityInwards = new MacroStabilityInwardsFailureMechanism();
            StabilityStoneCover = new StabilityStoneCoverFailureMechanism();
            WaveImpactAsphaltCover = new WaveImpactAsphaltCoverFailureMechanism();
            GrassCoverErosionOutwards = new GrassCoverErosionOutwardsFailureMechanism();
            GrassCoverSlipOffOutwards = new GrassCoverSlipOffOutwardsFailureMechanism();
            HeightStructures = new HeightStructuresFailureMechanism();
            ClosingStructure = new ClosingStructureFailureMechanism();
            StrengthStabilityPointConstruction = new StrengthStabilityPointConstructionFailureMechanism();
            PipingStructure = new PipingStructureFailureMechanism();
            DuneErosion = new DuneErosionFailureMechanism();
            
            FailureMechanismContribution = new FailureMechanismContribution(GetFailureMechanisms(), 30, 30000);
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
        public MacroStabilityInwardsFailureMechanism MacrostabilityInwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        public StabilityStoneCoverFailureMechanism StabilityStoneCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Golfklappen op asfaltbekledingen" failure mechanism.
        /// </summary>
        public WaveImpactAsphaltCoverFailureMechanism WaveImpactAsphaltCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism GrassCoverErosionOutwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism GrassCoverSlipOffOutwards { get; set; }

        /// <summary>
        /// Gets the "Kunstwerken - Hoogte kunstwerk" failure mechanism.
        /// </summary>
        public HeightStructuresFailureMechanism HeightStructures { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        public ClosingStructureFailureMechanism ClosingStructure { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        public PipingStructureFailureMechanism PipingStructure { get; set; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        public StrengthStabilityPointConstructionFailureMechanism StrengthStabilityPointConstruction { get; private set; }

        /// <summary>
        /// Gets the "Duinwaterkering - Duinafslag" failure mechanism.
        /// </summary>
        public DuneErosionFailureMechanism DuneErosion { get; private set; }

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
                PipingFailureMechanism.NormProbabilityInput.SectionLength = value == null ? double.NaN : Math2D.Length(value.Points);
            }
        }

        public FailureMechanismContribution FailureMechanismContribution
        {
            get
            {
                return contritbution;
            }
            private set
            {
                contritbution = value;
                PipingFailureMechanism.NormProbabilityInput.Norm = value.Norm;
            }
        }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

        public long StorageId { get; set; }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return PipingFailureMechanism;
            yield return GrassCoverErosionInwards;
            yield return MacrostabilityInwards;
            yield return StabilityStoneCover;
            yield return WaveImpactAsphaltCover;
            yield return GrassCoverErosionOutwards;
            yield return GrassCoverSlipOffOutwards;
            yield return HeightStructures;
            yield return ClosingStructure;
            yield return PipingStructure;
            yield return StrengthStabilityPointConstruction;
            yield return DuneErosion;
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
                    ClosingStructure.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StrengthStabilityPointConstruction.Contribution = 2;
                    DuneErosion.Contribution = 0;
                    FailureMechanismContribution.UpdateContributions(GetFailureMechanisms(), 30);
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
                    ClosingStructure.Contribution = 0;
                    PipingStructure.Contribution = 0;
                    StrengthStabilityPointConstruction.Contribution = 0;
                    DuneErosion.Contribution = 70;
                    FailureMechanismContribution.UpdateContributions(GetFailureMechanisms(), 30);
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
                    ClosingStructure.Contribution = 4;
                    PipingStructure.Contribution = 2;
                    StrengthStabilityPointConstruction.Contribution = 2;
                    DuneErosion.Contribution = 10;
                    FailureMechanismContribution.UpdateContributions(GetFailureMechanisms(), 20);
                    break;
                default:
                    throw new NotImplementedException();
            }
            Composition = newComposition;
        }
    }
}