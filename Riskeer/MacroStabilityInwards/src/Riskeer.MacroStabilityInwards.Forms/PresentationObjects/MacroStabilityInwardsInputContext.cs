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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="MacroStabilityInwardsInput"/>
    /// and allowing for selecting a surface line or soil profile based on data available
    /// in a macro stability inwards failure mechanism.
    /// </summary>
    public class MacroStabilityInwardsInputContext : MacroStabilityInwardsContext<MacroStabilityInwardsInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsInputContext"/>
        /// </summary>
        /// <param name="macroStabilityInwardsInput">The macro stability inwards input instance wrapped by this context object.</param>
        /// <param name="calculation">The calculation scenario the <paramref name="macroStabilityInwardsInput"/> belongs to.</param>
        /// <param name="surfaceLines">The surface lines available within the macro stability inwards context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the macro stability inwards context.</param>
        /// <param name="macroStabilityInwardsFailureMechanism">The failure mechanism which the macro stability inwards context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the macro stability inwards context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public MacroStabilityInwardsInputContext(MacroStabilityInwardsInput macroStabilityInwardsInput,
                                                 MacroStabilityInwardsCalculationScenario calculation,
                                                 IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines,
                                                 IEnumerable<MacroStabilityInwardsStochasticSoilModel> stochasticSoilModels,
                                                 MacroStabilityInwardsFailureMechanism macroStabilityInwardsFailureMechanism,
                                                 IAssessmentSection assessmentSection)
            : base(macroStabilityInwardsInput, surfaceLines, stochasticSoilModels, macroStabilityInwardsFailureMechanism, assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            MacroStabilityInwardsCalculation = calculation;
        }

        /// <summary>
        /// Gets the calculation scenario which the macro stability inwards context belongs to.
        /// </summary>
        public MacroStabilityInwardsCalculationScenario MacroStabilityInwardsCalculation { get; }
    }
}