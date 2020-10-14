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
using System.Drawing;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Forms.TreeNodeInfos;
using RiskeerFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class CalculationTypeTestHelperTest
    {
        [Test]
        public void CalculationTypeWithImageCases_Always_ReturnsExpectedCases()
        {
            // Call
            IEnumerable<TestCaseData> testCases = CalculationTypeTestHelper.CalculationTypeWithImageCases.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(CalculationType.Hydraulic, RiskeerFormsResources.HydraulicCalculationIcon),
                new TestCaseData(CalculationType.Probabilistic, RiskeerFormsResources.ProbabilisticCalculationIcon),
                new TestCaseData(CalculationType.SemiProbabilistic, RiskeerFormsResources.SemiProbabilisticCalculationIcon)
            };

            Assert.AreEqual(expectedCases.Length, testCases.Count());
            for (var i = 0; i < testCases.Count(); i++)
            {
                Assert.AreEqual(expectedCases[i].Arguments[0], testCases.ElementAt(i).Arguments[0]);
                TestHelper.AssertImagesAreEqual((Image) expectedCases[i].Arguments[1], (Image) testCases.ElementAt(i).Arguments[1]);
            }
        }
    }
}