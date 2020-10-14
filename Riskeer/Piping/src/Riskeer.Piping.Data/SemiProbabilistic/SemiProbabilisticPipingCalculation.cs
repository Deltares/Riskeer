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

namespace Riskeer.Piping.Data.SemiProbabilistic
{
    /// <summary>
    /// This class holds information about a semi-probabilistic calculation for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class SemiProbabilisticPipingCalculation : PipingCalculation<SemiProbabilisticPipingInput, PipingOutput>
    {
        /// <summary>
        /// Creates a new instance of <see cref="SemiProbabilisticPipingCalculation"/>.
        /// </summary>
        /// <param name="generalInputParameters">General piping calculation parameters that are the same across all
        /// piping calculations.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="generalInputParameters"/>
        /// is <c>null</c>.</exception>
        public SemiProbabilisticPipingCalculation(GeneralPipingInput generalInputParameters) : base(new SemiProbabilisticPipingInput(generalInputParameters)) {}

        public override bool HasOutput => Output != null;

        /// <summary>
        /// Gets or sets the results of the piping calculation.
        /// </summary>
        public PipingOutput Output { get; set; }

        public override void ClearOutput()
        {
            Output = null;
        }

        public override object Clone()
        {
            var clone = (SemiProbabilisticPipingCalculation) base.Clone();

            if (Output != null)
            {
                clone.Output = (PipingOutput) Output.Clone();
            }

            return clone;
        }
    }
}