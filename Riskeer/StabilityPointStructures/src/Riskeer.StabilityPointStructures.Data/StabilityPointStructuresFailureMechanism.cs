﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using Riskeer.Common.Data;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.StabilityPointStructures.Data.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.StabilityPointStructures.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Strength and Stability of Point Constructions failure mechanism.
    /// </summary>
    public class StabilityPointStructuresFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>,
                                                            ICalculatableFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StabilityPointStructuresFailureMechanism"/> class.
        /// </summary>
        public StabilityPointStructuresFailureMechanism()
            : base(Resources.StabilityPointStructuresFailureMechanism_DisplayName, Resources.StabilityPointStructuresFailureMechanism_Code)
        {
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName
            };
            GeneralInput = new GeneralStabilityPointStructuresInput();
            StabilityPointStructures = new StructureCollection<StabilityPointStructure>();
            ForeshoreProfiles = new ForeshoreProfileCollection();
            CalculationsInputComments = new Comment();
        }

        /// <summary>
        /// Gets the general stability point structures calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralStabilityPointStructuresInput GeneralInput { get; }

        /// <summary>
        /// Gets the available stability point structures for this instance.
        /// </summary>
        public StructureCollection<StabilityPointStructure> StabilityPointStructures { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; }

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations().Cast<StructuresCalculation<StabilityPointStructuresInput>>();

        public CalculationGroup CalculationsGroup { get; }

        public Comment CalculationsInputComments { get; }
    }
}