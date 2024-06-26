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

namespace Riskeer.Piping.Data.Probabilistic
{
    /// <summary>
    /// Base class that holds information about a probabilistic calculation for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public abstract class ProbabilisticPipingCalculation : PipingCalculation<ProbabilisticPipingInput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingCalculation"/>.
        /// </summary>
        protected ProbabilisticPipingCalculation() : base(new ProbabilisticPipingInput()) {}

        public override bool ShouldCalculate =>
            !HasOutput || InputParameters.ShouldProfileSpecificIllustrationPointsBeCalculated != Output.ProfileSpecificOutput.HasGeneralResult
                       || InputParameters.ShouldSectionSpecificIllustrationPointsBeCalculated != Output.SectionSpecificOutput.HasGeneralResult;

        public override bool HasOutput => Output != null;

        /// <summary>
        /// Gets or sets the results of the piping calculation.
        /// </summary>
        public ProbabilisticPipingOutput Output { get; set; }

        /// <summary>
        /// Clears the calculated illustration points.
        /// </summary>
        public void ClearIllustrationPoints()
        {
            Output?.ClearIllustrationPoints();
        }

        public override void ClearOutput()
        {
            Output = null;
        }

        public override object Clone()
        {
            var clone = (ProbabilisticPipingCalculation) base.Clone();

            if (Output != null)
            {
                clone.Output = (ProbabilisticPipingOutput) Output.Clone();
            }

            return clone;
        }
    }
}