// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Piping.Data.Probabilistic;

namespace Riskeer.Piping.Data.TestUtil
{
    /// <summary>
    /// Partial probabilistic piping output for testing purposes.
    /// </summary>
    public class TestPartialProbabilisticPipingOutput : PartialProbabilisticPipingOutput<TestTopLevelIllustrationPoint>
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestPartialProbabilisticPipingOutput"/>.
        /// </summary>
        /// <param name="reliability">The reliability of the calculation.</param>
        /// <param name="generalResult">The general result of this output with the fault tree illustration points.</param>
        public TestPartialProbabilisticPipingOutput(double reliability, GeneralResult<TestTopLevelIllustrationPoint> generalResult)
            : base(reliability, generalResult) {}
    }
}