﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionOutwards.Data.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Data
{
    /// <summary>
    /// Model containing input and output needed to perform different levels of the
    /// Grass Cover Erosion Outwards failure mechanism.
    /// </summary>
    public class GrassCoverErosionOutwardsFailureMechanism : FailureMechanismBase<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>,
                                                             ICalculatableFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionOutwardsFailureMechanism()
            : base(Resources.GrassCoverErosionOutwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionOutwardsFailureMechanism_Code)
        {
            GeneralInput = new GeneralGrassCoverErosionOutwardsInput();
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName
            };
            ForeshoreProfiles = new ForeshoreProfileCollection();
        }

        /// <summary>
        /// Gets the general grass cover erosion outwards calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralGrassCoverErosionOutwardsInput GeneralInput { get; }

        /// <summary>
        /// Gets the available foreshore profiles for this instance.
        /// </summary>
        public ForeshoreProfileCollection ForeshoreProfiles { get; }

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>();

        public CalculationGroup CalculationsGroup { get; }
    }
}