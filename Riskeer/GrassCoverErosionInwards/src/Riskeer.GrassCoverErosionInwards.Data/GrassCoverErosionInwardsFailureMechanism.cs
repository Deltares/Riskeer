﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.GrassCoverErosionInwards.Data.Properties;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Data
{
    /// <summary>
    /// Model for performing grass cover erosion inwards calculations.
    /// </summary>
    public class GrassCoverErosionInwardsFailureMechanism : FailureMechanismBase<AdoptableFailureMechanismSectionResult>,
                                                            ICalculatableFailureMechanism
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrassCoverErosionInwardsFailureMechanism"/> class.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism()
            : base(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode)
        {
            CalculationsGroup = new CalculationGroup
            {
                Name = RiskeerCommonDataResources.FailureMechanism_Calculations_DisplayName
            };
            GeneralInput = new GeneralGrassCoverErosionInwardsInput();
            DikeProfiles = new DikeProfileCollection();
            CalculationsInputComments = new Comment();
        }

        /// <summary>
        /// Gets the general grass cover erosion inwards calculation input parameters that apply to each calculation.
        /// </summary>
        public GeneralGrassCoverErosionInwardsInput GeneralInput { get; }

        /// <summary>
        /// Gets the available dike profiles for this instance.
        /// </summary>
        public DikeProfileCollection DikeProfiles { get; }

        public IEnumerable<ICalculation> Calculations => CalculationsGroup.GetCalculations().OfType<GrassCoverErosionInwardsCalculation>();

        public CalculationGroup CalculationsGroup { get; }

        public Comment CalculationsInputComments { get; }
    }
}