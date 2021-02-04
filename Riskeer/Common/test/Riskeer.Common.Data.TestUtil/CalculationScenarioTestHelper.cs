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

using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Helper class for testing <see cref="ICalculationScenario"/>.
    /// </summary>
    public static class CalculationScenarioTestHelper
    {
        /// <summary>
        /// Gets the valid scenario contribution values.
        /// </summary>
        /// <returns>A collection of valid scenario contribution values.</returns>
        public static IEnumerable<TestCaseData> GetValidScenarioContributionValues()
        {
            return new[]
            {
                new TestCaseData(-0.00001),
                new TestCaseData(0.0),
                new TestCaseData(1.0),
                new TestCaseData(1.00001)
            };
        }

        /// <summary>
        /// Gets the invalid scenario contribution values.
        /// </summary>
        /// <returns>A collection of invalid scenario contribution values.</returns>
        public static IEnumerable<TestCaseData> GetInvalidScenarioContributionValues()
        {
            return new[]
            {
                new TestCaseData(double.NaN),
                new TestCaseData(double.PositiveInfinity),
                new TestCaseData(double.NegativeInfinity),
                new TestCaseData(-0.0001),
                new TestCaseData(1.0001)
            };
        }
    }
}