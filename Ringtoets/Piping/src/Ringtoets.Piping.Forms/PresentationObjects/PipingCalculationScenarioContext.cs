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

using System;
using System.Collections.Generic;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="PipingCalculation"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class PipingCalculationScenarioContext : PipingContext<PipingCalculationScenario>, ICalculationContext<PipingCalculationScenario, PipingFailureMechanism>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationScenarioContext"/> class.
        /// </summary>
        /// <param name="calculation">The <see cref="PipingCalculation"/> instance wrapped by this context object.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the piping context.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism which the piping context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public PipingCalculationScenarioContext(PipingCalculationScenario calculation,
                                                IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines,
                                                IEnumerable<StochasticSoilModel> stochasticSoilModels,
                                                PipingFailureMechanism pipingFailureMechanism,
                                                IAssessmentSection assessmentSection)
            : base(calculation, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
    }
}