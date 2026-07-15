// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
using Core.Gui.Helpers;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;

namespace Riskeer.Common.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfStructuresCalculationHandlerTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var calculation = new TestStructuresCalculation();

            // Call
            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler(inquiryHelper, calculation);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<IStructuresCalculation>>(handler);
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void ClearIllustrationPoints_WithVariousCalculationConfigurations_ClearsIllustrationPointsAndReturnsExpectedResult(
            TestStructuresCalculation calculation, bool expectedResult)
        {
            // Setup
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            var handler = new ClearIllustrationPointsOfStructuresCalculationHandler(inquiryHelper, calculation);

            bool hasOutput = calculation.HasOutput;

            // Call
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(expectedResult, isCalculationAffected);
            Assert.AreEqual(hasOutput, calculation.HasOutput);

            Assert.IsNull(calculation.Output?.GeneralResult);
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurations()
        {
            yield return new TestCaseData(new TestStructuresCalculation(), false);
            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput()
            }, false);
            yield return new TestCaseData(new TestStructuresCalculation
            {
                Output = new TestStructuresOutput(new TestGeneralResultFaultTreeIllustrationPoint())
            }, true);
        }
    }
}