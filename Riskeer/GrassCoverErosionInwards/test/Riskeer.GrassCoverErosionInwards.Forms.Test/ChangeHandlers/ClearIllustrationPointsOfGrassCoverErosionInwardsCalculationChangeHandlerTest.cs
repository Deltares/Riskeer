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
using System.Collections.Generic;
using Core.Common.Gui;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandlerTest
    {
        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(inquiryHelper, calculation);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<GrassCoverErosionInwardsCalculation>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        [TestCaseSource(nameof(GetCalculationConfigurations))]
        public void ClearIllustrationPoints_WithVariousCalculationConfigurations_ClearsIllustrationPointsAndReturnsExpectedResult(
            GrassCoverErosionInwardsCalculation calculation,
            bool expectedResult)
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(inquiryHelper, calculation);

            // Call
            bool result = handler.ClearIllustrationPoints();

            // Assert
            Assert.AreEqual(expectedResult, result);

            Assert.IsNull(calculation.Output?.OvertoppingOutput.GeneralResult);
            Assert.IsNull(calculation.Output?.OvertoppingRateOutput?.GeneralResult);
            Assert.IsNull(calculation.Output?.DikeHeightOutput?.GeneralResult);
            mocks.VerifyAll();
        }

        private static IEnumerable<TestCaseData> GetCalculationConfigurations()
        {
            var random = new Random(21);
            var calculationWithOverToppingOutputWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                            null,
                                                            null)
            };

            var calculationWithDikeHeightRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                            new TestDikeHeightOutput(new TestGeneralResultFaultTreeIllustrationPoint()),
                                                            null)
            };

            var calculationWithOvertoppingRateWithIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                            null,
                                                            new TestOvertoppingRateOutput(new TestGeneralResultFaultTreeIllustrationPoint()))
            };

            var calculationWitNoIllustrationPoints = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(new TestOvertoppingOutput(random.NextDouble()),
                                                            null,
                                                            null)
            };

            yield return new TestCaseData(calculationWithOverToppingOutputWithIllustrationPoints, true);
            yield return new TestCaseData(calculationWithDikeHeightRateWithIllustrationPoints, true);
            yield return new TestCaseData(calculationWithOvertoppingRateWithIllustrationPoints, true);
            yield return new TestCaseData(calculationWitNoIllustrationPoints, false);
            yield return new TestCaseData(new GrassCoverErosionInwardsCalculation(), false);
        }
    }
}