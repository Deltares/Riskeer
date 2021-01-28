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

using Core.Common.Gui.Commands;
using Core.Common.Gui.Helpers;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.TestUtil.IllustrationPoints;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Data.TestUtil;
using Riskeer.GrassCoverErosionInwards.Forms.ChangeHandlers;
using Riskeer.GrassCoverErosionInwards.Util;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.ChangeHandlers
{
    [TestFixture]
    public class ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandlerTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(
                calculation, inquiryHelper, viewCommands);

            // Assert
            Assert.IsInstanceOf<ClearIllustrationPointsOfCalculationChangeHandlerBase<GrassCoverErosionInwardsCalculation>>(handler);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationWithoutOutput_WhenClearIllustrationPoints_ThenNothingHappensAndReturnFalse()
        {
            // Given
            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();

            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(
                calculation, inquiryHelper, viewCommands);

            // When
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Then
            Assert.IsFalse(isCalculationAffected);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsFalse(GrassCoverErosionInwardsIllustrationPointsHelper.HasIllustrationPoints(calculation));
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void ClearIllustrationPoints_WithVariousCalculationConfigurations_ClearsIllustrationPointsAndReturnsExpectedResult(
            [Values(true, false)] bool hasOvertoppingIllustrationPoints,
            [Values(true, false)] bool hasDikeHeightIllustrationPoints,
            [Values(true, false)] bool hasOvertoppingRateIllustrationPoints)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(
                    new TestOvertoppingOutput(hasOvertoppingIllustrationPoints
                                                  ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                  : null),
                    new TestDikeHeightOutput(hasDikeHeightIllustrationPoints
                                                 ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                 : null),
                    new TestOvertoppingRateOutput(hasOvertoppingRateIllustrationPoints
                                                      ? new TestGeneralResultFaultTreeIllustrationPoint()
                                                      : null))
            };

            var mocks = new MockRepository();
            var inquiryHelper = mocks.Stub<IInquiryHelper>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            if (hasOvertoppingIllustrationPoints)
            {
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculation.Output.OvertoppingOutput.GeneralResult));
            }

            if (hasDikeHeightIllustrationPoints)
            {
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculation.Output.DikeHeightOutput.GeneralResult));
            }

            if (hasOvertoppingRateIllustrationPoints)
            {
                viewCommands.Expect(vc => vc.RemoveAllViewsForItem(calculation.Output.OvertoppingRateOutput.GeneralResult));
            }
            mocks.ReplayAll();

            var handler = new ClearIllustrationPointsOfGrassCoverErosionInwardsCalculationChangeHandler(calculation, inquiryHelper, viewCommands);

            // Call
            bool isCalculationAffected = handler.ClearIllustrationPoints();

            // Assert
            bool expectedResult = hasOvertoppingIllustrationPoints || hasDikeHeightIllustrationPoints || hasOvertoppingRateIllustrationPoints;
            Assert.AreEqual(expectedResult, isCalculationAffected);
            Assert.IsTrue(calculation.HasOutput);
            Assert.IsNull(calculation.Output.OvertoppingOutput.GeneralResult);
            Assert.IsNull(calculation.Output.OvertoppingRateOutput.GeneralResult);
            Assert.IsNull(calculation.Output.DikeHeightOutput.GeneralResult);
            mocks.VerifyAll();
        }
    }
}