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
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.Properties;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Ringtoets.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Grass Cover Slip Off Outwards failure mechanism.
    /// </summary>
    public class GrassCoverSlipOffOutwardsFailureMechanism : FailureMechanismBase,
                                                             IHasSectionResults<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>
    {
        private readonly ObservableList<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverSlipOffOutwardsFailureMechanism()
            : base(Resources.GrassCoverSlipOffOutwardsFailureMechanism_DisplayName, Resources.GrassCoverSlipOffOutwardsFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new GrassCoverSlipOffOutwardsFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}