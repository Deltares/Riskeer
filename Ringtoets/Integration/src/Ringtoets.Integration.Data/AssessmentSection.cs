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

using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;

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

            Piping = new Piping.Data.Piping();
            GrassErosion = new Placeholder(Resources.GrassErosionFailureMechanism_DisplayName);
            MacrostabilityInward = new Placeholder(Resources.MacrostabilityInwardFailureMechanism_DisplayName);
            Overtopping = new Placeholder(Resources.OvertoppingFailureMechanism_DisplayName);
            Closing = new Placeholder(Resources.ClosingFailureMechanism_DisplayName);
            FailingOfConstruction = new Placeholder(Resources.FailingOfConstructionFailureMechanism_DisplayName);
            StoneRevetment = new Placeholder(Resources.StoneRevetmentFailureMechanism_DisplayName);
            AsphaltRevetment = new Placeholder(Resources.AsphaltRevetmentFailureMechanism_DisplayName);
            GrassRevetment = new Placeholder(Resources.GrassRevetmentFailureMechanism_DisplayName);
            DuneErosion = new Placeholder(Resources.ErosionFailureMechanism_DisplayName);

            FailureMechanismContribution = new FailureMechanismContribution(GetFailureMechanisms(), 30, 30000);
            ChangeComposition(composition);
        }

        /// <summary>
        /// Gets the "Piping" failure mechanism.
        /// </summary>
        public Piping.Data.Piping Piping { get; private set; }

        /// <summary>
        /// Gets the "Graserosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public Placeholder GrassErosion { get; private set; }

        /// <summary>
        /// Gets the "Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public Placeholder MacrostabilityInward { get; private set; }

        /// <summary>
        /// Gets the "Overslag en overloop" failure mechanism.
        /// </summary>
        public Placeholder Overtopping { get; private set; }

        /// <summary>
        /// Gets the "Niet sluiten" failure mechanism.
        /// </summary>
        public Placeholder Closing { get; private set; }

        /// <summary>
        /// Gets the "Constructief falen" failure mechanism.
        /// </summary>
        public Placeholder FailingOfConstruction { get; private set; }

        /// <summary>
        /// Gets the "Steenbekledingen" failure mechanism.
        /// </summary>
        public Placeholder StoneRevetment { get; private set; }

        /// <summary>
        /// Gets the "Asfaltbekledingen" failure mechanism.
        /// </summary>
        public Placeholder AsphaltRevetment { get; private set; }

        /// <summary>
        /// Gets the "Grasbekledingen" failure mechanism.
        /// </summary>
        public Placeholder GrassRevetment { get; private set; }

        /// <summary>
        /// Gets the "Duinerosie" failure mechanism.
        /// </summary>
        public Placeholder DuneErosion { get; private set; }

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
                Piping.SemiProbabilisticInput.SectionLength = value == null ? double.NaN : Math2D.Length(value.Points);
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
                Piping.SemiProbabilisticInput.Norm = value.Norm;
            }
        }

        public HydraulicBoundaryDatabase HydraulicBoundaryDatabase { get; set; }

        public long StorageId { get; set; }

        public IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return Piping;
            yield return GrassErosion;
            yield return MacrostabilityInward;
            yield return Overtopping;
            yield return Closing;
            yield return FailingOfConstruction;
            yield return StoneRevetment;
            yield return AsphaltRevetment;
            yield return GrassRevetment;
            yield return DuneErosion;
        }

        public void ChangeComposition(AssessmentSectionComposition newComposition)
        {
            switch (newComposition)
            {
                case AssessmentSectionComposition.Dike:
                    Piping.Contribution = 24;
                    GrassErosion.Contribution = 24;
                    MacrostabilityInward.Contribution = 4;
                    Overtopping.Contribution = 2;
                    Closing.Contribution = 4;
                    FailingOfConstruction.Contribution = 2;
                    StoneRevetment.Contribution = 4;
                    AsphaltRevetment.Contribution = 3;
                    GrassRevetment.Contribution = 3;
                    DuneErosion.Contribution = 0;
                    FailureMechanismContribution.UpdateContributions(GetFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.Dune:
                    Piping.Contribution = 0;
                    GrassErosion.Contribution = 0;
                    MacrostabilityInward.Contribution = 0;
                    Overtopping.Contribution = 0;
                    Closing.Contribution = 0;
                    FailingOfConstruction.Contribution = 0;
                    StoneRevetment.Contribution = 0;
                    AsphaltRevetment.Contribution = 0;
                    GrassRevetment.Contribution = 0;
                    DuneErosion.Contribution = 70;
                    FailureMechanismContribution.UpdateContributions(GetFailureMechanisms(), 30);
                    break;
                case AssessmentSectionComposition.DikeAndDune:
                    Piping.Contribution = 24;
                    GrassErosion.Contribution = 24;
                    MacrostabilityInward.Contribution = 4;
                    Overtopping.Contribution = 2;
                    Closing.Contribution = 4;
                    FailingOfConstruction.Contribution = 2;
                    StoneRevetment.Contribution = 4;
                    AsphaltRevetment.Contribution = 3;
                    GrassRevetment.Contribution = 3;
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