﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="CalculationGroup"/>
    /// in order be able to create configurable macro stability inwards calculations.
    /// </summary>
    public class MacroStabilityInwardsCalculationGroupContext : MacroStabilityInwardsContext<CalculationGroup>,
                                                                ICalculationContext<CalculationGroup, MacroStabilityInwardsFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MacroStabilityInwardsCalculationGroupContext"/> class.
        /// </summary>
        /// <param name="calculationGroup">The <see cref="CalculationGroup"/> instance wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped calculation group.</param>
        /// <param name="surfaceLines">The surface lines available within the context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the context.</param>
        /// <param name="macroStabilityInwardsFailureMechanism">The macro stability inwards failure mechanism which the context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>, except for <paramref name="parent"/>.</exception>
        public MacroStabilityInwardsCalculationGroupContext(CalculationGroup calculationGroup,
                                                            CalculationGroup parent,
                                                            IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
                                                            IEnumerable<StochasticSoilModel> stochasticSoilModels,
                                                            MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
                                                            IAssessmentSection assessmentSection)
            : base(calculationGroup, surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection)
        {
            Parent = parent;
        }

        public CalculationGroup Parent { get; }
    }
}