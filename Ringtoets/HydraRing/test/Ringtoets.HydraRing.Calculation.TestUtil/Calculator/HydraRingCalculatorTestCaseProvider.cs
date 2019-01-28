// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using NUnit.Framework;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// Class that provide test cases which can be used to test objects using hydra ring calculators.
    /// </summary>
    public static class HydraRingCalculatorTestCaseProvider
    {
        /// <summary>
        /// Gets test cases of calculators that will fail when called, including the detailed error message that should be set.
        /// </summary>
        /// <param name="testName">The name of the test.</param>
        /// <returns>The test cases of calculators that will fail when called.</returns>
        /// <example>
        /// <code>
        /// [Test]
        /// [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditionsWithReportDetails), new object[]
        /// {
        ///     nameof(testName)
        /// })]
        /// public void testName(bool endInFailure, string lastErrorFileContent, string detailedReport)
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> GetCalculatorFailingConditionsWithReportDetails(string testName)
        {
            yield return new TestCaseData(false,
                                          "LastErrorFileContent",
                                          $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContent")
                .SetName(CreateTestName(testName, "LastErrorFileContent"));
            yield return new TestCaseData(true,
                                          null,
                                          "Er is geen foutrapport beschikbaar.")
                .SetName(CreateTestName(testName, "EndInFailure"));
            yield return new TestCaseData(true,
                                          "LastErrorFileContentAndEndInFailure",
                                          $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContentAndEndInFailure")
                .SetName(CreateTestName(testName, "LastErrorFileContentAndEndInFailure"));
        }

        /// <summary>
        /// Gets test cases of calculators that will fail when called.
        /// </summary>
        /// <param name="testName">The name of the test.</param>
        /// <returns>The test cases of calculators that will fail when called.</returns>
        /// <example>
        /// <code>
        /// [Test]
        /// [TestCaseSource(typeof(HydraRingCalculatorTestCaseProvider), nameof(HydraRingCalculatorTestCaseProvider.GetCalculatorFailingConditions), new object[]
        /// {
        ///     nameof(testName)
        /// })]
        /// public void testName(bool endInFailure, string lastErrorFileContent)
        /// </code>
        /// </example>
        public static IEnumerable<TestCaseData> GetCalculatorFailingConditions(string testName)
        {
            yield return new TestCaseData(false,
                                          "LastErrorFileContent")
                .SetName(CreateTestName(testName, "LastErrorFileContent"));
            yield return new TestCaseData(true,
                                          null)
                .SetName(CreateTestName(testName, "EndInFailure"));
            yield return new TestCaseData(true,
                                          "LastErrorFileContentAndEndInFailure")
                .SetName(CreateTestName(testName, "LastErrorFileContentAndEndInFailure"));
        }

        private static string CreateTestName(string testName, string parameter)
        {
            string parameters = $"({parameter})";
            int newLength = parameters.Length < testName.Length
                                ? testName.Length - parameters.Length
                                : testName.Length;
            string shortenedTestName = testName.Substring(0, newLength);
            return string.Concat(shortenedTestName, parameters);
        }
    }
}