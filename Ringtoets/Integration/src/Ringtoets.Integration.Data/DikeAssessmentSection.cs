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

using System.Collections.Generic;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Piping.Data;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The dike-based section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class DikeAssessmentSection : AssessmentSectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeAssessmentSection"/> class.
        /// </summary>
        public DikeAssessmentSection()
        {
            Name = Resources.DikeAssessmentSection_DisplayName;

            PipingFailureMechanism = new PipingFailureMechanism
            {
                Contribution = 24
            };
            GrassErosionFailureMechanism = new FailureMechanismPlaceholder(Resources.GrassErosionFailureMechanism_DisplayName)
            {
                Contribution = 24
            };
            MacrostabilityInwardFailureMechanism = new FailureMechanismPlaceholder(Resources.MacrostabilityInwardFailureMechanism_DisplayName)
            {
                Contribution = 4
            };
            OvertoppingFailureMechanism = new FailureMechanismPlaceholder(Resources.OvertoppingFailureMechanism_DisplayName)
            {
                Contribution = 2
            };
            ClosingFailureMechanism = new FailureMechanismPlaceholder(Resources.ClosingFailureMechanism_DisplayName)
            {
                Contribution = 4
            };
            FailingOfConstructionFailureMechanism = new FailureMechanismPlaceholder(Resources.FailingOfConstructionFailureMechanism_DisplayName)
            {
                Contribution = 2
            };
            StoneRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.StoneRevetmentFailureMechanism_DisplayName)
            {
                Contribution = 4
            };
            AsphaltRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.AsphaltRevetmentFailureMechanism_DisplayName)
            {
                Contribution = 3
            };
            GrassRevetmentFailureMechanism = new FailureMechanismPlaceholder(Resources.GrassRevetmentFailureMechanism_DisplayName)
            {
                Contribution = 3
            };

            FailureMechanismContribution = new FailureMechanismContribution(GetFailureMechanisms(), 30, 30000);
        }

        /// <summary>
        /// Gets the "Piping" failure mechanism.
        /// </summary>
        public PipingFailureMechanism PipingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Graserosie kruin en binnentalud" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassErosionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Macrostabiliteit binnenwaarts" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder MacrostabilityInwardFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Overslag en overloop" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder OvertoppingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Niet sluiten" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder ClosingFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Constructief falen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder FailingOfConstructionFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Steenbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder StoneRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Asfaltbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder AsphaltRevetmentFailureMechanism { get; private set; }

        /// <summary>
        /// Gets the "Grasbekledingen" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder GrassRevetmentFailureMechanism { get; private set; }

        public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return PipingFailureMechanism;
            yield return GrassErosionFailureMechanism;
            yield return MacrostabilityInwardFailureMechanism;
            yield return OvertoppingFailureMechanism;
            yield return ClosingFailureMechanism;
            yield return FailingOfConstructionFailureMechanism;
            yield return StoneRevetmentFailureMechanism;
            yield return AsphaltRevetmentFailureMechanism; 
            yield return GrassRevetmentFailureMechanism;
        }
    }
}