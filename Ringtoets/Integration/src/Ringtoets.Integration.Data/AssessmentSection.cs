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
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;
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

            MacrostabilityInwards = new FailureMechanismPlaceholder(Resources.MacrostabilityInwardFailureMechanism_DisplayName, Resources.MacrostabilityInwardFailureMechanism_Code);
            StabilityStoneCover = new FailureMechanismPlaceholder(Resources.StabilityStoneCoverFailureMechanism_DisplayName, Resources.StabilityStoneCoverFailureMechanism_Code);
            WaveImpactAsphaltCover = new FailureMechanismPlaceholder(Resources.WaveImpactAsphaltCoverFailureMechanism_DisplayName, Resources.WaveImpactAsphaltCoverFailureMechanism_Code);
            GrassCoverErosionOutside = new FailureMechanismPlaceholder(Resources.GrassCoverErosionOutsideFailureMechanism_DisplayName, Resources.GrassCoverErosionOutsideFailureMechanism_Code);
            GrassCoverSlipOffOutside = new FailureMechanismPlaceholder(Resources.GrassCoverSlipOffOutsideFailureMechanism_DisplayName, Resources.GrassCoverSlipOffOutsideFailureMechanism_Code);
            HeightStructure = new FailureMechanismPlaceholder(Resources.HeightStructureFailureMechanism_DisplayName, Resources.HeightStructureFailureMechanism_Code);
            ClosingStructure = new FailureMechanismPlaceholder(Resources.ClosingStructureFailureMechanism_DisplayName, Resources.ClosingStructureFailureMechanism_Code);
            StrengthStabilityPointConstruction = new FailureMechanismPlaceholder(Resources.StrengthStabilityPointConstructionFailureMechanism_DisplayName, Resources.StrengthStabilityPointConstructionFailureMechanism_Code);
            PipingStructure = new FailureMechanismPlaceholder(Resources.PipingStructureFailureMechanism_DisplayName, Resources.PipingStructureFailureMechanism_Code);
            DuneErosion = new FailureMechanismPlaceholder(Resources.DuneErosionFailureMechanism_DisplayName, Resources.DuneErosionFailureMechanism_Code);
            
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
        public FailureMechanismPlaceholder MacrostabilityInwards { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Stabiliteit steenzetting" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder StabilityStoneCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Golfklappen op asfaltbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder WaveImpactAsphaltCover { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding erosie buitentalud" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassCoverErosionOutside { get; private set; }

        /// <summary>
        /// Gets the "Dijken en dammen - Grasbekleding afschuiven buitentalud" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassCoverSlipOffOutside { get; set; }

        /// <summary>
        /// Gets the "Kunstwerken - Hoogte kunstwerk" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder HeightStructure { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Betrouwbaarheid sluiting kunstwerk" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder ClosingStructure { get; private set; }

        /// <summary>
        /// Gets the "Kunstwerken - Piping bij kunstwerk" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder PipingStructure { get; set; }

        /// <summary>
        /// Gets the "Kunstwerken - Sterkte en stabiliteit puntconstructies" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder StrengthStabilityPointConstruction { get; private set; }

        /// <summary>
        /// Gets the "Duinwaterkering - Duinafslag" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder DuneErosion { get; private set; }

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
            yield return GrassCoverErosionOutside;
            yield return GrassCoverSlipOffOutside;
            yield return HeightStructure;
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
                    GrassCoverErosionOutside.Contribution = 5;
                    GrassCoverSlipOffOutside.Contribution = 1;
                    HeightStructure.Contribution = 24;
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
                    GrassCoverErosionOutside.Contribution = 0;
                    GrassCoverSlipOffOutside.Contribution = 0;
                    HeightStructure.Contribution = 0;
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
                    GrassCoverErosionOutside.Contribution = 5;
                    GrassCoverSlipOffOutside.Contribution = 1;
                    HeightStructure.Contribution = 24;
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