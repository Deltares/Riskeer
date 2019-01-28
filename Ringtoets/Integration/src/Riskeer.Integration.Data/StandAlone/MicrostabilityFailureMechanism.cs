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

using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.Properties;
using Riskeer.Integration.Data.StandAlone.SectionResults;

namespace Riskeer.Integration.Data.StandAlone
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Microstability failure mechanism.
    /// </summary>
    public class MicrostabilityFailureMechanism : FailureMechanismBase, IHasSectionResults<MicrostabilityFailureMechanismSectionResult>
    {
        private readonly ObservableList<MicrostabilityFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="MicrostabilityFailureMechanism"/> class.
        /// </summary>
        public MicrostabilityFailureMechanism()
            : base(Resources.MicrostabilityFailureMechanism_DisplayName, Resources.MicrostabilityFailureMechanism_Code, 4)
        {
            sectionResults = new ObservableList<MicrostabilityFailureMechanismSectionResult>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                yield break;
            }
        }

        public IObservableEnumerable<MicrostabilityFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        protected override void AddSectionResult(FailureMechanismSection section)
        {
            base.AddSectionResult(section);
            sectionResults.Add(new MicrostabilityFailureMechanismSectionResult(section));
        }

        protected override void ClearSectionResults()
        {
            sectionResults.Clear();
        }
    }
}