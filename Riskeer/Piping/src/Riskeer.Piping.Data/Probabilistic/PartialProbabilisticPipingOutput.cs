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
using Riskeer.Common.Data.IllustrationPoints;

namespace Riskeer.Piping.Data.Probabilistic
{
    /// <summary>
    /// This class contains the result of a sub-calculation for a <see cref="ProbabilisticPipingCalculation"/>.
    /// </summary>
    public class PartialProbabilisticPipingOutput : IPartialProbabilisticPipingOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="PartialProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="reliability">The reliability of the calculation.</param>
        /// <param name="generalResult">The general result of this output with the fault tree illustration points.</param>
        public PartialProbabilisticPipingOutput(double reliability,
                                                GeneralResult<TopLevelFaultTreeIllustrationPoint> generalResult)
        {
            Reliability = reliability;
            GeneralResult = generalResult;
        }

        /// <summary>
        /// Gets the reliability of the calculation.
        /// </summary>
        public double Reliability { get; }

        /// <summary>
        /// Gets the value indicating whether the output contains a general result with illustration points.
        /// </summary>
        public bool HasGeneralResult => GeneralResult != null;

        /// <summary>
        /// Gets the general result with the fault tree illustration points.
        /// </summary>
        public GeneralResult<TopLevelFaultTreeIllustrationPoint> GeneralResult { get; private set; }

        /// <summary>
        /// Clears the illustration points of the output.
        /// </summary>
        public void ClearIllustrationPoints()
        {
            GeneralResult = null;
        }

        public object Clone()
        {
            var clone = (PartialProbabilisticPipingOutput) MemberwiseClone();

            if (GeneralResult != null)
            {
                clone.GeneralResult = (GeneralResult<TopLevelFaultTreeIllustrationPoint>) GeneralResult.Clone();
            }

            return clone;
        }
    }
}