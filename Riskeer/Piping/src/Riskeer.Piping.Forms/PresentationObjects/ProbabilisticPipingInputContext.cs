// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System;
using System.Collections.Generic;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.PresentationObjects
{
    /// <summary>
    /// A presentation layer object wrapping an instance of <see cref="ProbabilisticPipingInput"/>
    /// and allowing for selecting a surface line or soil profile based on data available
    /// in a piping failure mechanism.
    /// </summary>
    public class ProbabilisticPipingInputContext : PipingContext<ProbabilisticPipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContext"/>
        /// </summary>
        /// <param name="pipingInput">The piping input instance wrapped by this context object.</param>
        /// <param name="calculation">The calculation the <paramref name="pipingInput"/> belongs to.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the piping context.</param>
        /// <param name="pipingFailureMechanism">The failure mechanism which the piping context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public ProbabilisticPipingInputContext(ProbabilisticPipingInput pipingInput,
                                               ProbabilisticPipingCalculation calculation,
                                               IEnumerable<PipingSurfaceLine> surfaceLines,
                                               IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                               PipingFailureMechanism pipingFailureMechanism,
                                               IAssessmentSection assessmentSection)
            : base(pipingInput, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection)
        {
            if (calculation == null)
            {
                throw new ArgumentNullException(nameof(calculation));
            }

            PipingCalculation = calculation;
        }

        /// <summary>
        /// Gets the calculation which the piping context belongs to.
        /// </summary>
        public ProbabilisticPipingCalculation PipingCalculation { get; }
    }
}