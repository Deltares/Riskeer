// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Collections.Generic;
using NUnit.Framework;

namespace Riskeer.Common.Data.TestUtil.Probability
{
    /// <summary>
    /// Helper class for getting probability test cases.
    /// </summary>
    public static class ProbabilityTestHelper
    {
        /// <summary>
        /// Gets a set of valid probabilities.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TestCaseData"/>
        /// containing valid probabilities.</returns>
        public static IEnumerable<TestCaseData> GetValidProbabilities()
        {
            yield return new TestCaseData(0);
            yield return new TestCaseData(1);
            yield return new TestCaseData(0.5);
            yield return new TestCaseData(1e-6);
            yield return new TestCaseData(double.NaN);
        }

        /// <summary>
        /// Gets a set of invalid probabilities.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="TestCaseData"/>
        /// containing invalid probabilities.</returns>
        public static IEnumerable<TestCaseData> GetInvalidProbabilities()
        {
            yield return new TestCaseData(-20);
            yield return new TestCaseData(-1e-6);
            yield return new TestCaseData(1 + 1e-6);
            yield return new TestCaseData(12);
        }
    }
}