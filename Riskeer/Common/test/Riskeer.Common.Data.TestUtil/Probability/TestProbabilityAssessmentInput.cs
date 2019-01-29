﻿// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Riskeer.Common.Data.Probability;

namespace Riskeer.Common.Data.TestUtil.Probability
{
    /// <summary>
    /// A <see cref="ProbabilityAssessmentInput"/> that can be used in tests.
    /// </summary>
    public class TestProbabilityAssessmentInput : ProbabilityAssessmentInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestProbabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="a">The default value for the parameter 'a' to be used to factor in the 'length effect'
        /// when determining the maximum tolerated probability of failure.</param>
        /// <param name="b">The default value for the parameter 'b' to be used to factor in the 'length effect'
        /// when determining the maximum tolerated probability of failure.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="a"/> is not in the range [0, 1].</exception>
        public TestProbabilityAssessmentInput(double a, double b) : base(a, b) {}
    }
}