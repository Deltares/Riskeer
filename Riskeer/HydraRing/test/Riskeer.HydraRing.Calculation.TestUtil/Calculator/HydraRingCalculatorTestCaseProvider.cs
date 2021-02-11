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

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Riskeer.HydraRing.Calculation.TestUtil.Calculator
{
    /// <summary>
    /// Class that provide test cases which can be used to test objects using Hydra-Ring calculators.
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
                                          $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContent");
            yield return new TestCaseData(true,
                                          null,
                                          "Er is geen foutrapport beschikbaar.");
            yield return new TestCaseData(true,
                                          "LastErrorFileContentAndEndInFailure",
                                          $"Bekijk het foutrapport door op details te klikken.{Environment.NewLine}LastErrorFileContentAndEndInFailure");
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
                                          "LastErrorFileContent");
            yield return new TestCaseData(true,
                                          null);
            yield return new TestCaseData(true,
                                          "LastErrorFileContentAndEndInFailure");
        }
    }
}