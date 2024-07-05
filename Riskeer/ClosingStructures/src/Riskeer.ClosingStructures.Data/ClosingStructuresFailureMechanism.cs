// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.ClosingStructures.Data.Properties;
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.ClosingStructures.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Closing Structures failure mechanism.
    /// </summary>
    public class ClosingStructuresFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>,
                                                     ICalculatableFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClosingStructuresFailureMechanism"/> class.
        /// </summary>
        public ClosingStructuresFailureMechanism()
            : base(Resources.ClosingStructuresFailureMechanism_DisplayName, Resources.ClosingStructuresFailureMechanism_Code)
        {
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName
            };

            GeneralInput = new GeneralClosingStructuresInput();
            ClosingStructures = new StructureCollection<ClosingStructure>();
            ForeshoreProfiles = new ForeshoreProfileCollection();

            CalculationsInputComments = new Comment();
        }

        /// <summary>
        /// Gets the general closing structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralClosingStructuresInput GeneralInput { get; }

        /// <summary>
        /// Gets the available closing structures for this instance.
        /// </summary>
        public StructureCollection<ClosingStructure> ClosingStructures { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; }

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations().Cast<StructuresCalculation<ClosingStructuresInput>>();

        public CalculationGroup CalculationsGroup { get; }

        public Comment CalculationsInputComments { get; }
    }
}