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
using System.Linq;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.StabilityStoneCover.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Stability of Stone Cover failure mechanism.
    /// </summary>
    public class StabilityStoneCoverFailureMechanism : FailureMechanismBase, IHasSectionResults<StabilityStoneCoverFailureMechanismSectionResult>
    {
        private readonly IList<StabilityStoneCoverFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityStoneCoverFailureMechanism"/> class.
        /// </summary>
        public StabilityStoneCoverFailureMechanism()
            : base(Resources.StabilityStoneCoverFailureMechanism_DisplayName, Resources.StabilityStoneCoverFailureMechanism_Code)
        {
            sectionResults = new List<StabilityStoneCoverFailureMechanismSectionResult>();
            WaveConditionsCalculationGroup = new CalculationGroup(RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName, false);
            GeneralInput = new GeneralStabilityStoneCoverWaveConditionsInput();
            ForeshoreProfiles = new ObservableList<ForeshoreProfile>();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return WaveConditionsCalculationGroup.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>();
            }
        }

        /// <summary>
        /// Gets the general stability stone cover wave conditions input parameters that apply to each calculation.
        /// </summary>
        public GeneralStabilityStoneCoverWaveConditionsInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the container of all wave conditions calculations.
        /// </summary>
        public CalculationGroup WaveConditionsCalculationGroup { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ObservableList<ForeshoreProfile> ForeshoreProfiles { get; private set; }

        public IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new StabilityStoneCoverFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}