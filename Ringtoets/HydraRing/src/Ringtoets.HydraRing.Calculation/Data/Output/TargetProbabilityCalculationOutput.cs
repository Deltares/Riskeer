﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Data.Output
{
    /// <summary>
    /// Container of all relevant output generated by a type 2 calculation via Hydra-Ring ("iterate towards a target probability, provided as reliability index").
    /// </summary>
    public class TargetProbabilityCalculationOutput
    {
        private readonly double result;
        private readonly double actualTargetProbability;

        /// <summary>
        /// Creates a new instance of the <see cref="TargetProbabilityCalculationOutput"/> class.
        /// </summary>
        /// <param name="result">The result corresponding to <paramref name="actualTargetProbability"/>.</param>
        /// <param name="actualTargetProbability">The actual target probability that was reached.</param>
        public TargetProbabilityCalculationOutput(double result, double actualTargetProbability)
        {
            this.result = result;
            this.actualTargetProbability = actualTargetProbability;
        }

        /// <summary>
        /// Gets the result corresponding to <see cref="ActualTargetProbability"/>.
        /// </summary>
        public double Result
        {
            get
            {
                return result;
            }
        }

        /// <summary>
        /// Gets the actual target probability that was reached.
        /// </summary>
        public double ActualTargetProbability
        {
            get
            {
                return actualTargetProbability;
            }
        }
    }
}
