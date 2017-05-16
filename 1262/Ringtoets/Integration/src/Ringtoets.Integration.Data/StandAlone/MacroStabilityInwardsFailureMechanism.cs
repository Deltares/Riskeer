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

using System.Collections.Generic;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Macrostability Inwards failure mechanism.
    /// </summary>
    public class MacrostabilityInwardsFailureMechanism : FailureMechanismBase, IHasSectionResults<MacrostabilityInwardsFailureMechanismSectionResult>
    {
        private readonly IList<MacrostabilityInwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MacrostabilityInwardsFailureMechanism"/> class.
        /// </summary>
        public MacrostabilityInwardsFailureMechanism()
            : base(Resources.MacrostabilityInwardFailureMechanism_DisplayName, Resources.MacrostabilityInwardFailureMechanism_Code)
        {
            sectionResults = new List<MacrostabilityInwardsFailureMechanismSectionResult>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new MacrostabilityInwardsFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}