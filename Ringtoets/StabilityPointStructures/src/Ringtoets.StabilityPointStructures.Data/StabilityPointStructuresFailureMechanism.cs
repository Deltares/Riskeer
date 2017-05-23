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
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.StabilityPointStructures.Data.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Strength and Stability of Point Constructions failure mechanism.
    /// </summary>
    public class StabilityPointStructuresFailureMechanism : FailureMechanismBase, ICalculatableFailureMechanism, IHasSectionResults<StabilityPointStructuresFailureMechanismSectionResult>
    {
        private readonly IList<StabilityPointStructuresFailureMechanismSectionResult> sectionResults;

        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityPointStructuresFailureMechanism"/> class.
        /// </summary>
        public StabilityPointStructuresFailureMechanism()
            : base(Resources.StabilityPointStructuresFailureMechanism_DisplayName, Resources.StabilityPointStructuresFailureMechanism_Code)
        {
            CalculationsGroup = new CalculationGroup(RingtoetsCommonDataResources.FailureMechanism_Calculations_DisplayName, false);
            GeneralInput = new GeneralStabilityPointStructuresInput();
            StabilityPointStructures = new StructureCollection<StabilityPointStructure>();
            sectionResults = new List<StabilityPointStructuresFailureMechanismSectionResult>();
            ForeshoreProfiles = new ForeshoreProfileCollection();
        }

        public override IEnumerable<ICalculation> Calculations
        {
            get
            {
                return CalculationsGroup.GetCalculations().Cast<StructuresCalculation<StabilityPointStructuresInput>>();
            }
        }

        /// <summary>
        /// Gets the general stability point structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralStabilityPointStructuresInput GeneralInput { get; private set; }

        /// <summary>
        /// Gets the available stability point structures for this instance.
        /// </summary>
        public StructureCollection<StabilityPointStructure> StabilityPointStructures { get; private set; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; private set; }

        public CalculationGroup CalculationsGroup { get; }

        public IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> SectionResults
        {
            get
            {
                return sectionResults;
            }
        }

        public override void AddSection(FailureMechanismSection section)
        {
            base.AddSection(section);

            sectionResults.Add(new StabilityPointStructuresFailureMechanismSectionResult(section));
        }

        public override void ClearAllSections()
        {
            base.ClearAllSections();
            sectionResults.Clear();
        }
    }
}