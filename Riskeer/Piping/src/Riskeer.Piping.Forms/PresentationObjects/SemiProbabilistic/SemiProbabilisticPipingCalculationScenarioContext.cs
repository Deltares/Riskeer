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

using System;
using System.Collections.Generic;
using Core.Common.Controls.PresentationObjects;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic
{
    /// <summary>
    /// Presentation object for all data required to configure an instance of <see cref="SemiProbabilisticPipingCalculationScenario"/>
    /// in order to prepare it for performing a calculation.
    /// </summary>
    public class SemiProbabilisticPipingCalculationScenarioContext : PipingContext<SemiProbabilisticPipingCalculationScenario>,
                                                                     ICalculationContext<SemiProbabilisticPipingCalculationScenario, PipingFailureMechanism>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingCalculationScenarioContext"/>.
        /// </summary>
        /// <param name="calculation">The <see cref="SemiProbabilisticPipingCalculationScenario"/> instance wrapped by this context object.</param>
        /// <param name="parent">The <see cref="CalculationGroup"/> that owns the wrapped scenario.</param>
        /// <param name="surfaceLines">The surface lines available within the piping context.</param>
        /// <param name="stochasticSoilModels">The stochastic soil models available within the piping context.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism which the piping context belongs to.</param>
        /// <param name="assessmentSection">The assessment section which the piping context belongs to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        public SemiProbabilisticPipingCalculationScenarioContext(SemiProbabilisticPipingCalculationScenario calculation,
                                                                 CalculationGroup parent,
                                                                 IEnumerable<PipingSurfaceLine> surfaceLines,
                                                                 IEnumerable<PipingStochasticSoilModel> stochasticSoilModels,
                                                                 PipingFailureMechanism pipingFailureMechanism,
                                                                 IAssessmentSection assessmentSection)
            : base(calculation, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection)
        {
            if (parent == null)
            {
                throw new ArgumentNullException(nameof(parent));
            }

            Parent = parent;
        }

        public CalculationGroup Parent { get; }

        public override bool Equals(WrappedObjectContextBase<SemiProbabilisticPipingCalculationScenario> other)
        {
            return base.Equals(other)
                   && other is SemiProbabilisticPipingCalculationScenarioContext context
                   && ReferenceEquals(Parent, context.Parent);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SemiProbabilisticPipingCalculationScenarioContext);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Parent.GetHashCode();
        }
    }
}