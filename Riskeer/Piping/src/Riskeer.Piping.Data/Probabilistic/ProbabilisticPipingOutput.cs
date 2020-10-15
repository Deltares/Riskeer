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

using Core.Common.Base;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Piping.Data.Probabilistic
{
    /// <summary>
    /// Class containing the results of a probabilistic piping calculation.
    /// </summary>
    public class ProbabilisticPipingOutput : CloneableObservable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="ProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="resultWithLength">The result of the sub-calculation that takes into account the section length.</param>
        /// <param name="resultWithoutLength">The result of the sub-calculation that doesn't take into account the section length.</param>
        /// <remarks>The input parameters are not checked for being <c>null</c> as (one of the) the sub-calculation results might
        /// be missing due to calculation failure.</remarks>
        public ProbabilisticPipingOutput(PartialProbabilisticPipingOutput resultWithLength, PartialProbabilisticPipingOutput resultWithoutLength)
        {
            ResultWithLength = resultWithLength;
            ResultWithoutLength = resultWithoutLength;
        }

        /// <summary>
        /// Gets the result of the sub-calculation that takes into account the section length.
        /// </summary>
        public PartialProbabilisticPipingOutput ResultWithLength { get; private set; }

        /// <summary>
        /// Gets the result of the sub-calculation that doesn't take into account the section length.
        /// </summary>
        public PartialProbabilisticPipingOutput ResultWithoutLength { get; private set; }

        public override object Clone()
        {
            var clone = (ProbabilisticPipingOutput) base.Clone();

            if (ResultWithLength != null)
            {
                clone.ResultWithLength = (PartialProbabilisticPipingOutput) ResultWithLength.Clone();
            }

            if (ResultWithoutLength != null)
            {
                clone.ResultWithoutLength = (PartialProbabilisticPipingOutput) ResultWithoutLength.Clone();
            }

            return clone;
        }
    }
}