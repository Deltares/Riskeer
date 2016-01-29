// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Collections.Generic;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data
{
    /// <summary>
    /// The dune-based section to be assessed by the user for safety in regards of various failure mechanisms.
    /// </summary>
    public sealed class DuneAssessmentSection : AssessmentSectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuneAssessmentSection"/> class.
        /// </summary>
        public DuneAssessmentSection()
        {
            Name = Resources.DuneAssessmentSection_DisplayName;

            DuneErosionFailureMechanism = new FailureMechanismPlaceholder(Resources.DuneErosionFailureMechanism_DisplayName)
            {
                Contribution = 70
            };

            FailureMechanismContribution = new FailureMechanismContribution(GetFailureMechanisms(), 30, 30000);
        }

        /// <summary>
        /// Gets the "Duin erosie" failure mechanism.
        /// </summary>
        public FailureMechanismPlaceholder DuneErosionFailureMechanism { get; private set; }

        public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            yield return DuneErosionFailureMechanism;
        }
    }
}